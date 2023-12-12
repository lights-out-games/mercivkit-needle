using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MercivKit
{
    public class MercivBridge : MonoBehaviour
    {
        private struct Message
        {
            [JsonProperty("event")]
            public string Event { get; set; }

            [JsonProperty("args")]
            public JToken[] Args { get; set; }

            [JsonProperty("rpc-id")]
            public int? RpcId { get; set; }

            [JsonProperty("response-id")]
            public int? ResponseId { get; set; }
        }

        private static MercivBridge _instance;
        public static MercivBridge Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MercivBridge>();
                }

                return _instance;
            }
        }

        private MercivBridgeTransport _bridge;

        private Dictionary<string, List<Delegate>> _subscriptions = new Dictionary<string, List<Delegate>>();

        private Dictionary<int, Action<JToken>> _rpcs = new Dictionary<int, Action<JToken>>();

        private int _rpcId = 1;

        private void Awake()
        {
            _instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

#if UNITY_WEBGL
            if (Application.isEditor)
            {
                _bridge = new MercivEditorBridge();
            }
            else
            {
                _bridge = new MercivJavaScriptBridge();
            }
#else
            _bridge = new MercivEditorBridge();
#endif
        }

        public Task<T> CallAsync<T>(string evt, params object[] args)
        {
            Log.Verbose("Calling RPC: " + evt);
            var message = new Message
            {
                Event = evt,
                Args = args.Select(JToken.FromObject).ToArray(),
                RpcId = _rpcId++
            };

            var tcs = new TaskCompletionSource<T>();
            _rpcs.Add(message.RpcId.Value, (result) => {
                tcs.SetResult(result.ToObject<T>());
            });

            var json = JsonConvert.SerializeObject(message);
            _bridge.SendMessage(json);
            return tcs.Task;
        }

        public void RaiseEvent(string evt, params object[] args)
        {
            Log.Verbose("Raising event: " + evt);
            var message = new Message
            {
                Event = evt,
                Args = args.Select(JToken.FromObject).ToArray()
            };

            var json = JsonConvert.SerializeObject(message);
            _bridge.SendMessage(json);
        }

        public void SubscribeDelegate(string evt, Delegate callback)
        {
            Log.Verbose("Subscribing to event: " + evt);
            if (!_subscriptions.TryGetValue(evt, out var callbacks))
            {
                callbacks = new List<Delegate>();
                _subscriptions.Add(evt, callbacks);
            }

            callbacks.Add(callback);
        }

        public void Subscribe<T>(string evt, Action<T> callback)
        {
            SubscribeDelegate(evt, (Delegate)callback);
        }

        public void Subscribe<T1, T2>(string evt, Action<T1, T2> callback)
        {
            SubscribeDelegate(evt, (Delegate)callback);
        }

        public void Subscribe<T1, T2, T3>(string evt, Action<T1, T2, T3> callback)
        {
            SubscribeDelegate(evt, (Delegate)callback);
        }

        public void CreateRpc<T, R>(string evt, Func<T, R> callback)
        {
            SubscribeDelegate(evt, (Delegate)callback);
        }

        public void CreateRpc<T1, T2, R>(string evt, Func<T1, T2, R> callback)
        {
            SubscribeDelegate(evt, (Delegate)callback);
        }

        public void CreateRpc<T1, T2, T3, R>(string evt, Func<T1, T2, T3, R> callback)
        {
            SubscribeDelegate(evt, (Delegate)callback);
        }

        public void UnsubscribeDelegate(string evt, Delegate callback)
        {
            Log.Verbose("Unsubscribing from event: " + evt);
            if (_subscriptions == null)
            {
                return;
            }

            if (!_subscriptions.TryGetValue(evt, out var callbacks))
            {
                return;
            }

            callbacks.Remove(callback);
        }

        public void Unsubscribe<T>(string evt, Action<T> callback)
        {
            UnsubscribeDelegate(evt, (Delegate)callback);
        }

        public void Unsubscribe<T1, T2>(string evt, Action<T1, T2> callback)
        {
            UnsubscribeDelegate(evt, (Delegate)callback);
        }

        public void Unsubscribe<T1, T2, T3>(string evt, Action<T1, T2, T3> callback)
        {
            UnsubscribeDelegate(evt, (Delegate)callback);
        }

        public void RemoveRpc<T, R>(string evt, Func<T, R> callback)
        {
            UnsubscribeDelegate(evt, (Delegate)callback);
        }

        public void RemoveRpc<T1, T2, R>(string evt, Func<T1, T2, R> callback)
        {
            UnsubscribeDelegate(evt, (Delegate)callback);
        }

        public void RemoveRpc<T1, T2, T3, R>(string evt, Func<T1, T2, T3, R> callback)
        {
            UnsubscribeDelegate(evt, (Delegate)callback);
        }

        private void Update()
        {
            while (_bridge.ReceiveQueue.TryDequeue(out var message))
            {
                var parsed = JsonConvert.DeserializeObject<Message>(message);

                if (parsed.ResponseId.HasValue)
                {
                    if (_rpcs == null)
                    {
                        Log.Verbose("No RPCs for response: " + parsed.Event + " " + parsed.ResponseId);
                        continue;
                    }

                    if (!_rpcs.TryGetValue(parsed.ResponseId.Value, out var callback))
                    {
                        Log.Verbose("No RPC for response: " + parsed.Event + " " + parsed.ResponseId);
                        continue;
                    }

                    Log.Verbose("Received response for RPC: " + parsed.Event + " " + parsed.ResponseId);
                    _rpcs.Remove(parsed.ResponseId.Value);
                    callback(parsed.Args[0]);
                    continue;
                }

                if (_subscriptions == null)
                {
                    Log.Verbose("No subscriptions for event: " + parsed.Event);
                    continue;
                }

                if (!_subscriptions.TryGetValue(parsed.Event, out var callbacks))
                {
                    Log.Verbose("No subscribers for event: " + parsed.Event);
                    continue;
                }

                foreach (var callback in callbacks)
                {
                    Log.Verbose("Invoking callback for event: " + parsed.Event);
                    var method = callback.Method;
                    var parameters = method.GetParameters();
                    var args = new object[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        var arg = parsed.Args[i];
                        args[i] = arg.ToObject(parameter.ParameterType);
                    }

                    var result = callback.DynamicInvoke(args);
                    if (result != null)
                    {
                        Log.Verbose("Sending response from event handler for: " + parsed.Event);
                        var response = new Message
                        {
                            Event = parsed.Event,
                            Args = new[] { JToken.FromObject(result) },
                            ResponseId = parsed.RpcId
                        };

                        var json = JsonConvert.SerializeObject(response);
                        _bridge.SendMessage(json);
                    }
                }
            }
        }
    }
}
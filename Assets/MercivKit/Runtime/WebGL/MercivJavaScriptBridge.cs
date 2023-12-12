#if UNITY_WEBGL

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace MercivKit
{
    internal class MercivJavaScriptBridge : MercivBridgeTransport
    {
        private static MercivJavaScriptBridge _instance;

        private ConcurrentQueue<string> _receiveQueue;
        public ConcurrentQueue<string> ReceiveQueue => _receiveQueue;

        [DllImport("__Internal")]
        private static extern void MercivJavaScriptBridgeCreate(Action<string> OnMessage);

        [DllImport("__Internal")]
        private static extern void MercivJavaScriptBridgeSend(string message);

        public MercivJavaScriptBridge()
        {
            _receiveQueue = new ConcurrentQueue<string>();
            _instance = this;
            MercivJavaScriptBridgeCreate(OnMessage);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        public static void OnMessage(string message)
        {
            _instance.OnMessageInstance(message);
        }

        public void SendMessage(string message)
        {
            Log.Verbose("Sending: " + message);
            MercivJavaScriptBridgeSend(message);
        }

        private void OnMessageInstance(string message)
        {
            Log.Verbose("Received: " + message);
            _receiveQueue.Enqueue(message);
        }
    }
}

#endif
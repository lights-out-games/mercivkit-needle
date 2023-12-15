
using System.Collections.Concurrent;
using System;
using System.IO;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

//using System.Net.WebSockets;
using WebSocketSharp;

namespace MercivKit
{
    //WebSocket websocket;
    internal class MercivEditorBridge : MercivBridgeTransport
    {
        private static MercivEditorBridge _instance;

        private ConcurrentQueue<string> _receiveQueue;
        public ConcurrentQueue<string> ReceiveQueue => _receiveQueue;

        //[DllImport("__Internal")]
        //private static extern void MercivEditorBridgeCreate(Action<string> OnMessage);

        public MercivEditorBridge()
        {
            _receiveQueue = new ConcurrentQueue<string>();
            _instance = this;

            var ws = new WebSocket("ws://localhost:3000");
            //using (var ws = new WebSocket ("wss://localhost:5963/Echo"))
            //using (var ws = new WebSocket ("ws://localhost:4649/Chat"))
            //using (var ws = new WebSocket ("wss://localhost:5963/Chat"))
            //using (var ws = new WebSocket ("ws://localhost:4649/Chat?name=nobita"))
            //using (var ws = new WebSocket ("wss://localhost:5963/Chat?name=nobita"))
            
                // Set the WebSocket events.

                ws.OnOpen += (sender, e) => ws.Send("Hi, there!");

                ws.OnMessage += (sender, e) => {
                    var fmt = "[WebSocket Message] {0}";
                    var body = !e.IsPing ? e.Data : "A ping was received.";

                    Console.WriteLine(fmt, body);
                };

                ws.OnError += (sender, e) => {
                    var fmt = "[WebSocket Error] {0}";

                    Console.WriteLine(fmt, e.Message);
                };

                ws.OnClose += (sender, e) => {
                    var fmt = "[WebSocket Close ({0})] {1}";

                    Console.WriteLine(fmt, e.Code, e.Reason);
                };

                // websocket = new WebSocket("ws://echo.websocket.org");
                //websocket = new WebSocket("ws://localhost:3000");

                //websocket.OnOpen += () =>
                //{
                //    Debug.Log("Connection open!");
                //};

                //websocket.OnError += (e) =>
                //{
                //    Debug.Log("Error! " + e);
                //};

                //websocket.OnClose += (e) =>
                //{
                //    Debug.Log("Connection closed!");
                //};

                //websocket.OnMessage += (bytes) =>
                //{
                //    // Reading a plain text message
                //    var message = System.Text.Encoding.UTF8.GetString(bytes);
                //    Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
                //};

                //// Keep sending messages at every 0.3s
                //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

                //await websocket.Connect();
        }


        [MonoPInvokeCallback(typeof(Action<string>))]
        public static void OnMessage(string message)
        {
            _instance.OnMessageInstance(message);
        }

        public void SendMessage(string message)
        {
            // TODO
            Log.Verbose("Sending: " + message);
        }

        private void OnMessageInstance(string message)
        {
            // TODO
            Log.Verbose("Received: " + message);
        }
    }
}
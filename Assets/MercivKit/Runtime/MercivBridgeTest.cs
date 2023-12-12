using System;
using UnityEngine;

namespace MercivKit
{
    public class MercivBridgeTest : MonoBehaviour
    {
        async void Start()
        {
            Log.Info("Calling add(1, 2, 3)...");

            MercivBridge.Instance.CreateRpc<string, string>("hello", (string message) => {
                Log.Info("From JS: " + message);
                return "Hello from C#!";
            });

            MercivBridge.Instance.RaiseEvent("ready");

            var result = await MercivBridge.Instance.CallAsync<int>("add", 1, 2, 3);
            Log.Info("Add Result: " + result);
        }

        void OnEnable()
        {
            Log.Info("Subscribing to 'clock'...");
            MercivBridge.Instance.Subscribe<string>("clock", OnClock);
        }

        private void OnClock(string time)
        {
            Log.Info("Clock: " + time);
        }

        void OnDisable()
        {
            Log.Info("Unsubscribing from 'clock'...");
            MercivBridge.Instance.Unsubscribe<string>("clock", OnClock);
        }
    }
}
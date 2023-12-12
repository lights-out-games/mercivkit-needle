
using System.Collections.Concurrent;

namespace MercivKit
{
    internal class MercivEditorBridge : MercivBridgeTransport
    {
        private ConcurrentQueue<string> _receiveQueue;
        public ConcurrentQueue<string> ReceiveQueue => _receiveQueue;

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
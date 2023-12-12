using System.Collections.Concurrent;

namespace MercivKit
{
    internal interface MercivBridgeTransport
    {
        public ConcurrentQueue<string> ReceiveQueue { get; }
        void SendMessage(string message);
    }
}
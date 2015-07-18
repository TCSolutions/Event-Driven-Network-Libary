using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace NetworkLib.Networking.StateObjects
{
    internal class ServerAcceptStateObject
    {
        public Server Parent;
        public Socket Listener;
        public ManualResetEvent mReset;
        public bool Listening;
        public bool HasEnded;
    }
}

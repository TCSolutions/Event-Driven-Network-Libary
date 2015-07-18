using System;
using System.Net.Sockets;
using NetworkLib.Networking.StateObjects;

namespace NetworkLib.Networking
{
    public delegate void ClientConnectedEventHandler(object clientStateObject);
    public delegate void ClientDisconnectedEventHandler(object clientStateObject);

    public abstract class CommunicationBase
    {
        public Socket sSocket;
        public ClientConnectedEventHandler OnClientConnected;
        public ClientDisconnectedEventHandler OnClientDisconnected;
    }
}

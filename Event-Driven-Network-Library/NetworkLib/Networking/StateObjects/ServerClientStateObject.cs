using System;
using NetworkLib.Packets;
using System.Net.Sockets;

namespace NetworkLib.Networking.StateObjects
{
    public class ServerClientStateObject : BaseClientStateObject
    {
        public int ClientID;

        public ServerClientStateObject()
        {
            StateType = StateObjectType.Server;
        }
    }
}

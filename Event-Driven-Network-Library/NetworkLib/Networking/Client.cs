using System;

using System.Net;
using System.Threading;
using System.Net.Sockets;

using NetworkLib.Packets;
using NetworkLib.Extensions;
using System.Runtime.InteropServices;
using NetworkLib.Networking.StateObjects;

namespace NetworkLib.Networking
{
    public class Client : CommunicationBase
    {
        public ClientStateObject cStateObject;
        
        public void Connect(IPEndPoint epConnect)
        {
            sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sSocket.SendTimeout = 5000;
            sSocket.ReceiveTimeout = 5000;

            sSocket.Connect(epConnect);

            cStateObject = new ClientStateObject();
            cStateObject.Parent = this;
            cStateObject.StateType = StateObjectType.Client;
            cStateObject.ClientSocket = sSocket;

            OnClientConnected(cStateObject);

            try
            {
                sSocket.BeginReceive(cStateObject.headerBuffer, 0, cStateObject.headerBuffer.Length, 0,
                new AsyncCallback(Core.ReadCallback), cStateObject);
            }
            catch { OnClientDisconnected(cStateObject); }
            
        }

        public void Send(byte[] Data)
        {
            cStateObject.Send(Data);
        }

        public void Disconnect()
        {
            sSocket.Close();
        }
    }
}

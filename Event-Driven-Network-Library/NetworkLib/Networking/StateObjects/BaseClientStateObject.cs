using System;
using System.Net;
using System.Net.Sockets;
using NetworkLib.Packets;
using System.Runtime.InteropServices;

namespace NetworkLib.Networking.StateObjects
{
    public abstract class BaseClientStateObject
    {
        public StateObjectType StateType;

        public object Parent;
        public Socket ClientSocket;

        internal int headerIndex = 0;
        internal int dataIndex = 0;

        internal ReadMode rMode = ReadMode.Header;
        internal PacketHeader pHeader = default(PacketHeader);
        internal byte[] headerBuffer = new byte[Marshal.SizeOf(typeof(PacketHeader))];
        internal byte[] dataBuffer = null;

        public object ClientData;

        public delegate void PacketReceivedEventHandler(object ClientStateObject, BytePacket bPacket);
        public PacketReceivedEventHandler[] OnPacketReceived = new PacketReceivedEventHandler[Core.MaxEventHandlers];

        public bool IsConnected { get { return !(ClientSocket.Available == 0 && ClientSocket.Poll(1, SelectMode.SelectRead)); } }

        public void Send(byte[] bData)
        {
            if(ClientSocket.Connected) ClientSocket.BeginSend(bData, 0, bData.Length, SocketFlags.None, new AsyncCallback(Core.SendCallback), this);
        }

        public void Disconnect()
        {
            try { ClientSocket.Disconnect(false); }
            catch { }
            try { ClientSocket.Close(); }
            catch { }

            if (this.StateType == StateObjectType.Server)
            {
                lock (((Server)this.Parent).listLock)
                {
                    if(((Server)this.Parent).lClientList.Remove((ServerClientStateObject)this))
                    {
                        ((CommunicationBase)this.Parent).OnClientDisconnected(this);
                    }
                }
            }
            else
            {
                try { ((CommunicationBase)this.Parent).OnClientDisconnected(this); }
                catch { }
            }
        }
    }
}

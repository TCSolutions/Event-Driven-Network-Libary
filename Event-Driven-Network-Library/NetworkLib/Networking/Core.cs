using System;

using System.Net;
using System.Threading;
using System.Net.Sockets;

using NetworkLib.Packets;
using NetworkLib.Encryption;
using NetworkLib.Extensions;
using System.Runtime.InteropServices;
using NetworkLib.Networking.StateObjects;

namespace NetworkLib.Networking
{
    public static class Core
    {
        public static int MaxEventHandlers = 100;
        public static int MaxAllowedDataSize = 10485760;
        public static EncryptionType DefaultEncryption = EncryptionType.None;

        internal static void DoListen(object acceptObject)
        {
            ServerAcceptStateObject sAcceptObject = (ServerAcceptStateObject)acceptObject;
           
            while (sAcceptObject.Listening)
            {

                sAcceptObject.mReset.Reset();
                sAcceptObject.Listener.BeginAccept(AcceptCallback, sAcceptObject);
                
                while (sAcceptObject.Listening)
                {
                    if (sAcceptObject.mReset.WaitOne(1000))
                        break;
                }
                
            }

            sAcceptObject.HasEnded = true;
        }

        internal static void AcceptCallback(IAsyncResult ar)
        {
            ServerAcceptStateObject sAcceptObject = (ServerAcceptStateObject)ar.AsyncState;
            sAcceptObject.mReset.Set();

            Socket sListener = sAcceptObject.Listener;
            Socket sClient;
            
            try
            {
                sClient = sListener.EndAccept(ar);
            }
            catch { return; }
            

            ServerClientStateObject sStateObject = new ServerClientStateObject();

            sStateObject.ClientSocket = sClient;
            sStateObject.ClientID = sAcceptObject.Parent.nClientID++;
            sStateObject.StateType = StateObjectType.Server;
            sStateObject.Parent = sAcceptObject.Parent;

            if (sAcceptObject.Parent.OnClientConnected != null) sAcceptObject.Parent.OnClientConnected(sStateObject);

            lock (((Server)sStateObject.Parent).listLock)
            {
                ((Server)sStateObject.Parent).lClientList.Add(sStateObject);
            }
            
            try
            {
                sClient.BeginReceive(sStateObject.headerBuffer, 0, sStateObject.headerBuffer.Length, 0,
                 new AsyncCallback(ReadCallback), sStateObject);
            }
            catch
            {
                ((CommunicationBase)sStateObject.Parent).OnClientDisconnected(sStateObject);
                if (sStateObject.StateType == StateObjectType.Server)
                {

                    lock (((Server)sStateObject.Parent).listLock)
                    {
                        ((Server)sStateObject.Parent).lClientList.Remove((ServerClientStateObject)sStateObject);
                    }
                }
                return;
            }
            
        }

        internal static void ReadCallback(IAsyncResult ar)
        {
            BaseClientStateObject sStateObject = (BaseClientStateObject)ar.AsyncState;

            int nRead = 0;

            try
            {
                nRead = sStateObject.ClientSocket.EndSend(ar);
                if (nRead == 0)
                {
                    sStateObject.Disconnect();
                    return;
                }
                switch (sStateObject.rMode)
                {
                    case ReadMode.Header:
                        sStateObject.headerIndex += nRead;

                        if (sStateObject.headerIndex != sStateObject.headerBuffer.Length)
                        {
                            sStateObject.ClientSocket.BeginReceive(
                                    sStateObject.headerBuffer,
                                    sStateObject.headerIndex,
                                    sStateObject.headerBuffer.Length - sStateObject.headerIndex,
                                    SocketFlags.None,
                                    new AsyncCallback(ReadCallback), sStateObject
                                );

                            return;
                        }

                        sStateObject.pHeader = MarshalExtensions.ByteArrayToStructure<PacketHeader>(sStateObject.headerBuffer);
                        sStateObject.headerBuffer = new byte[Marshal.SizeOf(typeof(PacketHeader))];
                        sStateObject.headerIndex = 0;

                        if (sStateObject.pHeader.DataLength > 0)
                        {
                            if (sStateObject.pHeader.DataLength > MaxAllowedDataSize)
                            {
                                sStateObject.Disconnect();
                                return;
                            }
                            sStateObject.rMode = ReadMode.Data;
                            sStateObject.dataIndex = 0;
                            sStateObject.dataBuffer = new byte[sStateObject.pHeader.DataLength];
                            sStateObject.ClientSocket.BeginReceive(
                                    sStateObject.dataBuffer,
                                    sStateObject.dataIndex,
                                    sStateObject.dataBuffer.Length - sStateObject.dataIndex,
                                    SocketFlags.None,
                                    new AsyncCallback(ReadCallback), sStateObject
                                );
                            return;
                        }

                        break;

                    case ReadMode.Data:
                        sStateObject.dataIndex += nRead;

                        if (sStateObject.dataIndex != sStateObject.dataBuffer.Length)
                        {
                            sStateObject.ClientSocket.BeginReceive(
                                    sStateObject.dataBuffer,
                                    sStateObject.dataIndex,
                                    sStateObject.dataBuffer.Length - sStateObject.dataIndex,
                                    SocketFlags.None,
                                    new AsyncCallback(ReadCallback), sStateObject
                                );

                            return;
                        }

                        BytePacket bPacket = new BytePacket();
                        bPacket.FromExisting(sStateObject.pHeader, sStateObject.dataBuffer);

                        if (sStateObject.OnPacketReceived[bPacket.Header.PacketType] != null)
                        {
                            Thread tOnPacketReceived = new Thread(
                                () => {
                                    try 
                                    { 
                                        sStateObject.OnPacketReceived[bPacket.Header.PacketType](sStateObject, bPacket);
                                    }
                                    catch { sStateObject.Disconnect(); }
                                });

                            tOnPacketReceived.Start();
                        }
                            
                        sStateObject.rMode = ReadMode.Header;
                        sStateObject.ClientSocket.BeginReceive(
                            sStateObject.headerBuffer,
                            sStateObject.headerIndex,
                            sStateObject.headerBuffer.Length - sStateObject.headerIndex,
                            SocketFlags.None,
                            new AsyncCallback(ReadCallback), sStateObject
                        );

                        break;
                }
            }
            catch
            {
                sStateObject.Disconnect();
                return;
            }
        }

        internal static void SendCallback(IAsyncResult ar)
        {
            BaseClientStateObject sStateObject = (BaseClientStateObject)ar.AsyncState;
            try
            {
                sStateObject.ClientSocket.EndSend(ar);
            }
            catch
            {
                sStateObject.Disconnect();
                return;
            }
        }
    }
}

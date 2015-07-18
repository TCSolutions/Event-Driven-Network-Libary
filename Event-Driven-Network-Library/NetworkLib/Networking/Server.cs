using System;

using System.Net;
using System.Threading;
using System.Net.Sockets;

using NetworkLib.Packets;
using NetworkLib.Extensions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetworkLib.Networking.StateObjects;

namespace NetworkLib.Networking
{
    public class Server : CommunicationBase
    {
        public int SocketBacklog = 1000;
        
        internal int nClientID;
        private bool bListening = false;

        public ServerClientStateObject[] ConnectedClients
        {
            get
            {
                ServerClientStateObject[] cList;
                
                lock (listLock)
                {
                     cList = (ServerClientStateObject[])lClientList.ToArray().Clone();
                }

                return cList;
            }
        }

        internal object listLock = new object();
        internal List<ServerClientStateObject> lClientList = new List<ServerClientStateObject>();
        
        public int ClientCount
        {
            get
            {
                int nCount = 0;
                lock (listLock)
                {
                    nCount = lClientList.Count;
                }

                return nCount;
            }
        }

        public bool Listening { get { return bListening; } }

        internal ServerAcceptStateObject sAcceptObject;

        public void StartServer(IPEndPoint epBindTo)
        {
            if(!bListening)
            {
                lClientList = new List<ServerClientStateObject>();
                sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sSocket.Bind(epBindTo);
                sSocket.Listen(SocketBacklog);

                sAcceptObject = new ServerAcceptStateObject();
                sAcceptObject.Listener = sSocket;
                sAcceptObject.mReset = new ManualResetEvent(false);
                sAcceptObject.Listening = true;
                sAcceptObject.HasEnded = false;
                sAcceptObject.Parent = this;

                Thread listenThread = new Thread(new ParameterizedThreadStart(Core.DoListen));
                listenThread.Start(sAcceptObject);

                bListening = true;
            }
        }

        public void StopServer()
        {
            bListening = false;
            sAcceptObject.Listening = false;
            
            while(!sAcceptObject.HasEnded)
                Thread.Sleep(100);           

            foreach (ServerClientStateObject sClient in lClientList) try { sClient.Disconnect(); } catch { }
            while (ClientCount!=0)
                Thread.Sleep(100);

            sSocket.Close();
            
        }
    }
}

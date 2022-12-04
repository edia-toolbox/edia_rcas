using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using Unity.Collections;
using UnityEngine.Android;
using System.Collections.Concurrent;
using UnityEngine.Events;

namespace RCAS
{
    public sealed class RCAS_UDP_Connection : System.IDisposable
    {
        public bool isConnected => Client != null;

        internal delegate void dOnReceivedMessage(RCAS_UDPMessage msg);
        internal dOnReceivedMessage OnReceivedMessage = delegate { };

        Task ReceiverTask;
        Task SenderTask;

        UdpClient Client;

        ConcurrentQueue<(byte[], IPEndPoint EP)> _SendQueue = null;
        ConcurrentQueue<(byte[], IPEndPoint EP)> SendQueue
        {
            get
            {
                if(_SendQueue is null)
                {
                    _SendQueue = new ConcurrentQueue<(byte[], IPEndPoint EP)>();
                    StartSender();
                }
                return _SendQueue;
            }
        }

        ConcurrentQueue<byte[]> ReceiveQueue = new ConcurrentQueue<byte[]>();

        /// <summary>
        /// Sends a UDP message to a remote peer if currently connected to one
        /// </summary>
        public void SendMessage(RCAS_UDPMessage msg, IPEndPoint EP)
        {
            if (!isConnected) return;

            SendQueue.Enqueue((msg.raw_data.ToArray(), EP));
        }

        /// <summary>
        /// Sends a UDP message to a remote peer if currently connected to one
        /// </summary>
        public void SendMessage(string msg, RCAS_UDP_CHANNEL channel, IPEndPoint EP)
        {
            SendMessage(new RCAS_UDPMessage(msg, channel), EP);
        }

        /// <summary>
        /// Sends a JPEG image to a remote peer if currently connected to one
        /// </summary>
        public void SendImage(byte[] ImageData, IPEndPoint EP)
        {
            SendMessage(RCAS_UDPMessage.EncodeImage(ImageData), EP);
        }

        /// <summary>
        /// Send a message to all devices on a local network to Peer.RemotePort
        /// </summary>
        public void BroadcastMessage(RCAS_UDPMessage msg, int port)
        {
            if (!isConnected) return;

            SendQueue.Enqueue((msg.raw_data.ToArray(), new IPEndPoint(System.Net.IPAddress.Broadcast, port)));
        }

        /// <summary>
        /// Send a message to all devices on a local network to Peer.RemotePort
        /// </summary>
        public void BroadcastMessage(string message, RCAS_UDP_CHANNEL channel, int port)
        {
            BroadcastMessage(new RCAS_UDPMessage(message, channel), port);
        }

        public RCAS_UDP_Connection()
        {
            // Need internet access on android
            Debug.Assert(Permission.HasUserAuthorizedPermission("android.permission.INTERNET"));
        }

        public void Connect(IPEndPoint LocalEndPoint)
        {
            if (isConnected) return; // already connected

            Debug.Log($"New UDP Connection on {LocalEndPoint.Address}:{LocalEndPoint.Port}");

            // Need internet access on android
            Debug.Assert(Permission.HasUserAuthorizedPermission("android.permission.INTERNET"));

            if (Client == null)
            {
                Client = new UdpClient();
                Client.ExclusiveAddressUse = false;
                Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                Client.Client.Bind(LocalEndPoint);
            }
            Client.EnableBroadcast = true;

            // Automatically start receiver. TODO: Do we want this?
            StartReceiver();
        }

        private void StartSender()
        {
            if (SenderTask == null || SenderTask.Status != TaskStatus.Running)
            {
                SenderTask = Task.Run(() => TaskFunc_Sender());
            }
        }

        private void StartReceiver()
        {
            if (ReceiverTask == null || ReceiverTask.Status != TaskStatus.Running)
            {
                ReceiverTask = Task.Run(() => TaskFunc_Receiver());
            }
        }

        private async Task TaskFunc_Sender()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(10);

                        if (SendQueue.TryDequeue(out var item) && item.Item2 != null)
                        {
                            Client.Send(item.Item1, item.Item1.Length, item.Item2);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Failed to send out UDP package!");
                        Debug.Log(e.Message);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal void Update()
        {
            if (!isConnected) return;

            if (ReceiveQueue.TryDequeue(out var data))
            {
                ProcessData(data);
            }
        }

        private async Task TaskFunc_Receiver()
        {
            try
            {
                while (true)
                {
                    // Non-asymc variant:
                    //byte[] receiveData = Client.Receive(ref EP);

                    // async variant:
                    byte[] receiveData = (await Client.ReceiveAsync()).Buffer;

                    ReceiveQueue.Enqueue(receiveData);
                }
            }
            catch (SocketException e)
            {
                Debug.LogError("SocketException whilst Receiving UDP data!");
                Debug.LogException(e);
            }
        }

        public void CloseConnection()
        {
            if (Client != null) Debug.Log("UDP Connection Closed.");
            SendQueue.Clear();
            Client?.Close();
            Client = null;
        }

        public void Dispose()
        {
            CloseConnection();
        }

        private void ProcessData(byte[] data)
        {
            RCAS_UDPMessage msg = new RCAS_UDPMessage(data);
            OnReceivedMessage.Invoke(msg);
        }
    }
}
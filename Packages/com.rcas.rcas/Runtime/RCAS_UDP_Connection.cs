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
    public class RCAS_UDP_Connection : System.IDisposable
    {
        public int LocalPort => Peer.LocalPort;

        public delegate void dOnReceivedMessage(RCAS_UDPMessage msg);
        public dOnReceivedMessage OnReceivedMessage = delegate { };

        public delegate void dOnReceivedPairingOffer(RCAS_UDPMessage msg);
        public dOnReceivedPairingOffer OnReceivedPairingOffer = delegate { };

        public delegate void dOnReceivedImage(RCAS_UDPMessage msg);
        public dOnReceivedImage OnReceivedImage = delegate { };

        Task ReceiverTask;
        Task SenderTask;

        static UdpClient Client;

        public RCAS_Peer Peer { get; private set; }

        CancellationTokenSource CTS;

        ConcurrentQueue<(byte[], IPEndPoint EP)> SendQueue = new ConcurrentQueue<(byte[], IPEndPoint EP)>();

        ConcurrentQueue<byte[]> ReceiveQueue = new ConcurrentQueue<byte[]>();

        public void SendMessage(RCAS_UDPMessage msg)
        {
            SendQueue.Enqueue((msg.raw_data.ToArray(), null));
        }

        public void SendImage(byte[] ImageData)
        {
            SendMessage(RCAS_UDPMessage.EncodeImage(ImageData));
        }

        public void SendMessageToEndpoint(RCAS_UDPMessage msg, IPEndPoint EP)
        {
            SendQueue.Enqueue((msg.raw_data.ToArray(), EP));
        }

        public void BroadcastMessage(RCAS_UDPMessage msg)
        {
            SendQueue.Enqueue((msg.raw_data.ToArray(), new IPEndPoint(System.Net.IPAddress.Broadcast, Peer.RemotePort)));
        }

        public RCAS_UDP_Connection(RCAS_Peer peer)
        {
            this.Peer = peer;

            CTS = new CancellationTokenSource();

            // Need internet access on android
            Debug.Assert(Permission.HasUserAuthorizedPermission("android.permission.INTERNET"));

            if (Client == null)
            {
                Client = new UdpClient();
                Client.ExclusiveAddressUse = false;
                Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                Client.Client.Bind(Peer.LocalEndPoint);
            }
            Client.EnableBroadcast = true;
        }

        public void StartSender()
        {
            if (SenderTask == null || SenderTask.Status != TaskStatus.Running)
            {
                SenderTask = Task.Run(() => TaskFunc_Sender(CTS.Token));
            }
        }

        public void StartReceiver()
        {
            if (ReceiverTask == null || ReceiverTask.Status != TaskStatus.Running)
            {
                ReceiverTask = Task.Run(() => TaskFunc_Receiver(CTS.Token));
            }
        }

        private async Task TaskFunc_Sender(CancellationToken CT)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        if (CT.IsCancellationRequested) throw new TaskCanceledException();

                        await Task.Delay(10);

                        if (SendQueue.TryDequeue(out var item))
                        {
                            Client.Send(item.Item1, item.Item1.Length, item.Item2 != null ? item.Item2 : Peer.RemoteEndpoint);
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        return;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Something went wrong!");
                        Debug.Log(e.Message);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Update()
        {
            if (ReceiveQueue.TryDequeue(out var data))
            {
                ProcessData(data);
            }
        }

        private async Task TaskFunc_Receiver(CancellationToken CT)
        {
            try
            {
                while (true)
                {
                    if(CT.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    // Non-asymc variant:
                    //IPEndPoint EP = Peer.LocalEndPoint;
                    //byte[] receiveData = Client.Receive(ref EP);

                    // async variant:
                    byte[] receiveData = (await Client.ReceiveAsync()).Buffer;

                    ReceiveQueue.Enqueue(receiveData);
                }
            }
            catch (SocketException e)
            {
                Debug.LogException(e);
            }
        }

        public void Dispose()
        {
            CTS?.Cancel();
            CTS?.Dispose();
            Client?.Close();
        }

        private void ProcessData(byte[] data)
        {
            RCAS_UDPMessage msg = new RCAS_UDPMessage(data);
            OnReceivedMessage.Invoke(msg);

            switch (msg.GetChannel())
            {
                case RCAS_UDP_CHANNEL.RESERVED_PAIRING:
                    {
                        OnReceivedPairingOffer(msg);
                        break;
                    }
                case RCAS_UDP_CHANNEL.RESERVED_JPEG_STREAM:
                    {
                        OnReceivedImage(msg);
                        break;
                    }
            }
        }
    }
}
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

public class RCAS_UDP_Connection
{
    public int Port { get; private set; }

    public delegate void dOnReceivedMessage(RCAS_UDPMessage msg);
    public dOnReceivedMessage OnReceivedMessage = delegate { };

    public delegate void dOnReceivedPairingOffer(RCAS_UDPMessage msg);
    public dOnReceivedPairingOffer OnReceivedPairingOffer = delegate { };

    public delegate void dOnReceivedImage(RCAS_UDPMessage msg);
    public dOnReceivedImage OnReceivedImage = delegate { };


    static UdpClient Client;

    public RCAS_Peer Peer { get; private set; }

    Task SenderTask;
    Task ReceiverTask;

    ConcurrentQueue<(byte[], RCAS_UDP_Channel)> SendQueue = new ConcurrentQueue<(byte[], RCAS_UDP_Channel)>();

    ConcurrentQueue<(byte[], RCAS_UDP_Channel)> ReceiveQueue = new ConcurrentQueue<(byte[], RCAS_UDP_Channel)>();

    public void SendMessage(RCAS_UDPMessage msg, RCAS_UDP_Channel channel)
    {
        SendQueue.Enqueue((msg.raw_data.ToArray(), channel));
    }

    public RCAS_UDP_Connection(RCAS_Peer peer)
    {
        this.Peer = peer;

        // Need internet access on android
        Debug.Assert(Permission.HasUserAuthorizedPermission("android.permission.INTERNET"));

        if (Client == null)
        {
            Client = new UdpClient(Peer.Port);
        }

        Client.EnableBroadcast = true;
    }

    public void StartSender()
    {
        SenderTask = new Task(TaskFunc_Sender);
        SenderTask.Start();
    }

    public void StartReceiver()
    {
        ReceiverTask = new Task(TaskFunc_Receiver);
        ReceiverTask.Start();
    }

    private async void TaskFunc_Sender()
    {
        try
        {
            while (true)
            {
                try
                {
                    await Task.Delay(10);

                    if (SendQueue.TryDequeue(out var item))
                    {
                        Client.Send(item.Item1, item.Item1.Length, item.Item2.channelEP);
                    }
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
        finally
        {
            Client.Close();
        }
    }

    public void Update()
    {
        if(ReceiveQueue.TryDequeue(out var item))
        {
            (byte[] data, _) = item;
            ProcessData(data);
        }
    }

    private void TaskFunc_Receiver()
    {
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, Peer.Port);

        try
        {
            while (true)
            {
                byte[] receiveData = Client.Receive(ref groupEP);

                ReceiveQueue.Enqueue((receiveData, null));
            }
        }
        catch (SocketException e)
        {
            Debug.LogException(e);
        }
        finally
        {
            Client.Close();
        }
    }

    private void ProcessData(byte[] data)
    {
        RCAS_UDPMessage msg = new RCAS_UDPMessage(data);
        OnReceivedMessage.Invoke(msg);

        switch(msg.GetChannel())
        {
            case RCAS_UDP_CHANNEL.PAIRING:
                {
                    OnReceivedPairingOffer(msg);
                    break;
                }
            case RCAS_UDP_CHANNEL.JPEG_STREAM:
                {
                    OnReceivedImage(msg);
                    break;
                }
        }
    }
}

public class RCAS_UDP_Channel
{
    public IPEndPoint channelEP;
    public byte channelID;

    public RCAS_UDP_Channel(IPAddress IpAddress, int port, byte channelID)
    {
        channelEP = new IPEndPoint(IpAddress, port);
        this.channelID = channelID;
    }

    public RCAS_UDP_Channel(IPEndPoint EndPoint, byte channelID)
    {
        channelEP = EndPoint;
        this.channelID = channelID;
    }

    public RCAS_UDP_Channel(string IpAddress, int port, byte channelID)
    {
        channelEP = new IPEndPoint(IPAddress.Parse(IpAddress), port);
        this.channelID = channelID;
    }
}
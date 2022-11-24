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
using System.Linq;

public sealed class RCAS_TCP_Connection
{
    public bool isConnected => Client is not null && Client.Connected;
    public bool isAwaitingConnection => ListenerTask != null && ListenerTask.Status != TaskStatus.Running;

    TcpClient Client;
    TcpListener Listener;

    Task ListenerTask;
    Task ReceiverTask;
    Task SenderTask;

    ConcurrentQueue<byte[]> SendQueue = new ConcurrentQueue<byte[]>();
    ConcurrentQueue<byte[]> ReceiveQueue = new ConcurrentQueue<byte[]>();

    public RCAS_Peer Peer { get; private set; }

    public IPEndPoint LocalEndPoint;

    public Dictionary<string, List<System.Action>> RemoteEvents = new Dictionary<string, List<System.Action>>();

    public delegate void dOnConnectionEstablished(EndPoint endpoint);
    public dOnConnectionEstablished OnConnectionEstablished;

    public RCAS_TCP_Connection(RCAS_Peer peer)
    {
        this.Peer = peer;

        this.LocalEndPoint = new IPEndPoint(IPAddress.Parse(peer.localIPAddress), peer.TCP_Port);

        // Find any and all methods marked as "RemoteEvent" in all assemplies:
        var methodsMarked =
            from a in System.AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            from m in t.GetMethods(System.Reflection.BindingFlags.Static | (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            let attributes = m.GetCustomAttributes(typeof(RemoteEvent), true)
            where attributes != null && attributes.Length > 0
            select new { method = m, attributes = attributes.Cast<RemoteEvent>() };

        foreach(var method in methodsMarked)
        {
            foreach (var att in method.attributes)
            {
                try
                {
                    System.Action action = (System.Action)System.Delegate.CreateDelegate(typeof(System.Action), method.method);
                    string eventName = att.getEventName();
                    if(!RemoteEvents.ContainsKey(eventName))
                    {
                        RemoteEvents.Add(eventName, new List<System.Action>());
                    }
                    RemoteEvents[eventName].Add(action);
                }
                catch (System.Exception)
                {
                    Debug.LogError("Following method is marked as RemoteEvent, but doesnt follow the necessary requirements.");
                }
            }
        }
    }

    public void SendMessage(RCAS_TCPMessage message)
    {
        Debug.Log("Sending Data: " + Encoding.ASCII.GetString(message.GetMessage()));
        Debug.Log("Type: " + message.GetMessageType());
        SendQueue.Enqueue(message.raw_data.ToArray());
        if (SenderTask == null || SenderTask.Status != TaskStatus.Running)
        {
            SenderTask = new Task(TaskFunc_Sender);
            SenderTask.Start();
        }
    }

    public void SendData(string sendData)
    {      
        SendMessage(new RCAS_TCPMessage(sendData, RCAS_TCP_MESSAGETYPE.NONE));
    }

    public void SendRemoteEvent(string message)
    {
        SendMessage(new RCAS_TCPMessage(message, RCAS_TCP_MESSAGETYPE.REMOTE_EVENT));
    }

    private bool prev_Connected = false;
    public void Update()
    {
        if(!prev_Connected && isConnected)
        {
            OnConnectionEstablished.Invoke(Client.Client.RemoteEndPoint);
        }
        prev_Connected = isConnected;

        if (ReceiveQueue.TryDequeue(out var item))
        {
            byte[] data = item;
            ReceiveData(data);
        }
    }

    public void ReceiveData(byte[] receiveData)
    {
        RCAS_TCPMessage msg = new RCAS_TCPMessage(receiveData);

        Debug.Log("Received Data: " + Encoding.ASCII.GetString(msg.GetMessage()));
        Debug.Log("Type: " + msg.GetMessageType());

        if (msg.GetMessageType() == RCAS_TCP_MESSAGETYPE.REMOTE_EVENT)
        {
            TriggerEvent(msg.GetMessageAsString());
        }
    }

    public void TriggerEvent(string event_name)
    {
        if(RemoteEvents.ContainsKey(event_name))
        {
            foreach (var m in RemoteEvents[event_name]) m.Invoke();
        }
        else
        {
            Debug.LogError("Event does not exist!");
        }
    }

    internal bool OpenConnection()
    {
        // TODO: Ensure we're not already a client or awaiting connection or similar

        Debug.Log("OPENING TCP SERVER...");

        try
        {
            Listener = new TcpListener(LocalEndPoint);
            
            ListenerTask = new Task(TaskFunc_Listener);
            ListenerTask.Start();

            return true;
        }
        catch
        {
            Debug.Log("COULD NOT OPEN TCP SERVER!");
            return false;
        }
    }

    internal bool ConnectTo(string ipAddress, int port)
    {
        // TODO: Ensure we are not awaiting connection already

        try {
            Debug.Log($"Trying to connect to {ipAddress}:{port}");
            Client = new TcpClient(LocalEndPoint);
            Client.Connect(ipAddress, port);

            return true;
        }
        catch {
            Debug.Log("Could not connect!");
            return false;
        }
    }

    internal bool CloseConnection()
    {
        Client?.Close();
        Listener?.Stop();

        return false;
    }

    private void TaskFunc_Listener()
    {
        Listener.Start();
        Client = Listener.AcceptTcpClient();
        ReceiverTask = new Task(TaskFunc_Receiver);
        ReceiverTask.Start();
        Listener.Stop();

        Debug.Log("RCAS: Client connected to Server!");
    }

    private void TaskFunc_Receiver()
    {
        while (isConnected)
        {
            System.Span<byte> buffer = new byte[Client.ReceiveBufferSize];

            int bytesRead = Client.GetStream().Read(buffer);

            //ReceiveData(buffer.Slice(0, bytesRead).ToArray());

            ReceiveQueue.Enqueue(buffer.Slice(0, bytesRead).ToArray());
        }
    }

    private void TaskFunc_Sender()
    {
        while (SendQueue.TryDequeue(out byte[] sendData))
        {
            if (!Client.Connected) continue;

            Client.GetStream().Write(sendData);
        }
    }
}

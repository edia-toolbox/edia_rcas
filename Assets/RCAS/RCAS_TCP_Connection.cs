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
    public int LocalPort => Peer.LocalPort;

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

    public Dictionary<string, List<System.Action<string[]>>> RemoteEvents = new Dictionary<string, List<System.Action<string[]>>>();

    public delegate void dOnConnectionEstablished(EndPoint endpoint);
    public dOnConnectionEstablished OnConnectionEstablished = delegate { };

    public RCAS_TCP_Connection(RCAS_Peer peer)
    {
        this.Peer = peer;

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
                    string eventName = att.getEventName();

                    // Single string function   static void func(string arg)
                    if (method.method.GetParameters().Count() == 1 && method.method.GetParameters()[0].ParameterType == typeof(System.String))
                    {
                        System.Action<string> action = (System.Action<string>)System.Delegate.CreateDelegate(typeof(System.Action<string>), method.method);
                        RegisterRemoteEvent(eventName, (args) => action(args[0]));
                    }
                    // String-array function   static void func(string[] args)
                    else if (method.method.GetParameters().Count() > 0)
                    {
                        System.Action<string[]> action = (System.Action<string[]>)System.Delegate.CreateDelegate(typeof(System.Action<string[]>), method.method);
                        RegisterRemoteEvent(eventName, action);
                    }
                    // No-parameter function   static void func()
                    else
                    {
                        System.Action action = (System.Action)System.Delegate.CreateDelegate(typeof(System.Action), method.method);
                        RegisterRemoteEvent(eventName, (args) => action());
                    }
                }
                catch (System.Exception)
                {
                    Debug.LogError("Following method is marked as RemoteEvent, but doesnt follow the necessary requirements.");
                }
            }
        }
    }

    private void RegisterRemoteEvent(string eventName, System.Action<string[]> action)
    {
        if (!RemoteEvents.ContainsKey(eventName))
        {
            RemoteEvents.Add(eventName, new List<System.Action<string[]>>());
        }
        RemoteEvents[eventName].Add(action);
    }

    public void SendMessage(RCAS_TCPMessage message)
    {
        SendQueue.Enqueue(message.raw_data.ToArray());
        if (SenderTask == null || SenderTask.Status != TaskStatus.Running)
        {
            SenderTask = new Task(TaskFunc_Sender);
            SenderTask.Start();
        }
    }

    public void SendRemoteEvent(string eventName, string[] args)
    {
        SendMessage(RCAS_TCPMessage.EncodeRemoteEvent(eventName, args));
    }

    public void SendRemoteEvent(string eventName, string arg)
    {
        SendRemoteEvent(eventName, new string[] { arg });
    }

    public void SendRemoteEvent(string eventName)
    {
        SendRemoteEvent(eventName, "");
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
            ProcessData(data);
        }
    }

    public void ProcessData(byte[] receiveData)
    {
        RCAS_TCPMessage msg = new RCAS_TCPMessage(receiveData);

        if (msg.GetChannel() == RCAS_TCP_CHANNEL.REMOTE_EVENT)
        {

            TriggerEvent(msg);
        }
    }

    public void TriggerEvent(string eventName, string[] args)
    {
        if (RemoteEvents.ContainsKey(eventName))
        {
            foreach (var m in RemoteEvents[eventName]) m.Invoke(args);
        }
        else
        {
            Debug.LogError("Event does not exist!");
        }
    }

    public void TriggerEvent(RCAS_TCPMessage msg)
    {
        (string eventName, string[] args) = RCAS_TCPMessage.DecodeRemoteEvent(msg);
        TriggerEvent(eventName, args);
    }

    internal bool OpenConnection()
    {
        // TODO: Ensure we're not already a client or awaiting connection or similar

        Debug.Log("OPENING TCP SERVER...");

        try
        {
            Listener = new TcpListener(Peer.LocalEndPoint);
            
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
            Client = new TcpClient(Peer.LocalEndPoint);
            Client.Connect(ipAddress, port);

            return true;
        }
        catch (System.Exception e) {
            Debug.Log("Could not connect!");
            Debug.LogException(e);
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

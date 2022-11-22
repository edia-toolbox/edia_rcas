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

    public Dictionary<string, List<System.Action>> RemoteEvents = new Dictionary<string, List<System.Action>>();

    public RCAS_TCP_Connection()
    {
        //return;
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

    public void SendData(System.Span<byte> sendData)
    {
        Debug.Log("Sending Data:" + Encoding.ASCII.GetString(sendData));
        SendQueue.Enqueue(sendData.ToArray());
        if (SenderTask == null || SenderTask.Status != TaskStatus.Running)
        {
            SenderTask = new Task(TaskFunc_Sender);
            SenderTask.Start();
        }
    }

    public void SendData(byte[] sendData)
    {
        Debug.Log("Sending Data:" + Encoding.ASCII.GetString(sendData));
        SendQueue.Enqueue(sendData);
        if (SenderTask == null || SenderTask.Status != TaskStatus.Running)
        {
            SenderTask = new Task(TaskFunc_Sender);
            SenderTask.Start();
        }
    }

    public void SendData(string sendData)
    {
        SendData(Encoding.ASCII.GetBytes(sendData));        
    }

    public void SendMessage(string message, byte messageType)
    {
        TCPMessage msg = new TCPMessage(message, messageType);

        SendData(msg.Data);
    }

    public void Update()
    {
        if (ReceiveQueue.TryDequeue(out var item))
        {
            byte[] data = item;
            ReceiveData(data);
        }
    }

    public void ReceiveData(byte[] receiveData)
    {
        //string dataReceived = Encoding.ASCII.GetString(receiveData);
        //Debug.Log("Received : " + dataReceived);

        System.Span<byte> rd = receiveData;
        //byte messageType = rd[0];
        //var message = rd.Slice(1);
        var message = receiveData;
        var message_string = Encoding.ASCII.GetString(message);

        //Debug.Log($"Message: |{message_string}|");
        //Debug.Log(message_string == "change_color_to_red");

        //if(messageType == 1)
        //{
        TriggerEvent(message_string);
        //}
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

    internal bool OpenConnection(string localIpAddress, int port)
    {
        // TODO: Ensure we're not already a client or awaiting connection or similar

        Debug.Log("OPENING TCP SERVER...");

        try
        {
            IPAddress localAdd = IPAddress.Parse("192.168.178.32");
            Listener = new TcpListener(localAdd, 27016);
            
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
            Client = new TcpClient(ipAddress, port);
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


public ref struct TCPMessage
{
    public System.Span<byte> Data;

    public TCPMessage(string message, byte messageType)
    {
        Data = Encoding.ASCII.GetBytes(message);
        //Data[0] = messageType;
    }

    // TODO: Make sure this works
    //public unsafe TCPMessage(byte[] message, byte messageType)
    //{
    //    Data = new byte[message.Length + 16];
    //    Data[0] = messageType;
    //    fixed (byte* bp = Data)
    //    {
    //        System.IntPtr ptr = System.IntPtr.Add((System.IntPtr)bp, 16 * sizeof(byte));
    //        System.Runtime.InteropServices.Marshal.Copy(message, 0, ptr, message.Length);
    //    }
    //}
}

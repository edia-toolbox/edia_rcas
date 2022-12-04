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

namespace RCAS
{
    public sealed class RCAS_TCP_Connection : System.IDisposable
    {
        #region MEMBERS
        internal bool isConnected => Client is not null && Client.Connected;
        internal bool isAwaitingConnection => ListenerTask != null && ListenerTask.Status != TaskStatus.Running;

        TcpClient Client;
        TcpListener Listener;

        Task ListenerTask;
        Task ReceiverTask;
        Task SenderTask;

        ConcurrentQueue<byte[]> SendQueue = new ConcurrentQueue<byte[]>();
        ConcurrentQueue<byte[]> ReceiveQueue = new ConcurrentQueue<byte[]>();

        internal delegate void dOnConnectionEstablished(IPEndPoint EP);
        internal dOnConnectionEstablished OnConnectionEstablished = delegate { };

        internal delegate void dOnReceivedMessage(RCAS_TCPMessage msg);
        internal dOnReceivedMessage OnReceivedMessage = delegate { };

        internal delegate void dOnConnectionLost(IPEndPoint EP);
        internal dOnConnectionLost OnConnectionLost = delegate { };

        public IPEndPoint LocalEndPoint
        {
            get
            {
                return (IPEndPoint)(Listener != null ? Listener.LocalEndpoint : Client?.Client?.LocalEndPoint);
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return (IPEndPoint)Client?.Client?.RemoteEndPoint;
            }
        }
        #endregion

        #region INIT
        public RCAS_TCP_Connection()
        {

        }
        #endregion

        #region SENDMESSAGE
        public void SendMessage(string message, RCAS_TCP_CHANNEL channel)
        {
            SendMessage(new RCAS_TCPMessage(message, channel));
        }

        public void SendMessage(RCAS_TCPMessage message)
        {
            if (!isConnected)
            {
                Debug.LogWarning("Tried to send a TCP Message without being connected to a remote client! Message will be discarded.");
                return;
            }

            SendQueue.Enqueue(message.raw_data.ToArray());
            if (SenderTask == null || SenderTask.Status != TaskStatus.Running)
            {
                SenderTask = new Task(TaskFunc_Sender);
                SenderTask.Start();
            }
        }
        #endregion

        #region UPDATE
        private bool prev_Connected = false;
        IPEndPoint prev_RemoteEP = null;
        internal void Update()
        {
            if (!prev_Connected && isConnected)
            {
                OnConnectionEstablished.Invoke(RemoteEndPoint);
            }
            else if(prev_Connected && !isConnected)
            {
                OnConnectionLost.Invoke(prev_RemoteEP);
            }
            prev_Connected = isConnected;
            prev_RemoteEP = RemoteEndPoint;

            if (ReceiveQueue.TryDequeue(out var item))
            {
                byte[] data = item;
                ProcessData(data);
            }
        }
        #endregion

        #region MISC
        private void ProcessData(byte[] receiveData)
        {
            RCAS_TCPMessage msg = new RCAS_TCPMessage(receiveData);

            OnReceivedMessage.Invoke(msg);
        }
        #endregion

        #region NETWORKING
        internal bool OpenConnection(IPEndPoint LocalEndPoint)
        {
            if (isConnected || isAwaitingConnection)
            {
                Debug.LogError("Tried to connect to a TCP EndPoint whilst awaiting a client or already connected!");
                return false;
            }

            Debug.Log("Opening TCP Connection");

            try
            {
                Listener = new TcpListener(LocalEndPoint);

                ListenerTask = new Task(TaskFunc_Listener);
                ListenerTask.Start();

                return true;
            }
            catch
            {
                Debug.Log("Could not open TCP Connection!");
                return false;
            }
        }

        internal bool ConnectTo(string ipAddress, int port, IPEndPoint LocalEndPoint)
        {
            // Ensure we are not awaiting connection already
            if(isConnected || isAwaitingConnection)
            {
                Debug.LogError("Tried to connect to a TCP EndPoint whilst awaiting a client or already connected!");
                return false;
            }

            try
            {
                Debug.Log($"Trying to connect to {ipAddress}:{port}");
                Client = new TcpClient(LocalEndPoint);
                Client.Connect(ipAddress, port);

                ReceiverTask = new Task(TaskFunc_Receiver);
                ReceiverTask.Start();

                return true;
            }
            catch (System.Exception e)
            {
                CloseConnection();
                Debug.Log("Could not connect!");
                Debug.LogException(e);
                return false;
            }
        }

        internal void CloseConnection()
        {
            SendQueue.Clear();
            Debug.Log("Connection closed");
            try
            {
                Listener?.Stop();
                Client?.GetStream()?.Close();
                //Client?.Client?.Close();
                Client?.Close();
            }
            catch { }
            Client = null;
        }
        #endregion

        #region TASKS
        private void TaskFunc_Listener()
        {
            try
            {
                Debug.Log("Listener started");
                Listener.Start();
                Client = Listener.AcceptTcpClient();
                ReceiverTask = new Task(TaskFunc_Receiver);
                ReceiverTask.Start();
                Listener.Stop();
                Debug.Log("RCAS: Client connected to Server!");
            }
            finally
            {
                Debug.Log("Listener stopped");
            }
        }

        private void TaskFunc_Receiver()
        {
            try
            {
                Debug.Log("Receiver started");
                while (isConnected)
                {
                    System.Span<byte> buffer = new byte[Client.ReceiveBufferSize];

                    int bytesRead = Client.GetStream().Read(buffer);

                    if (bytesRead == 0)
                    {
                        CloseConnection();
                        Debug.Log("Receiver ended");
                        return;
                    }

                    ReceiveQueue.Enqueue(buffer.Slice(0, bytesRead).ToArray());
                }
            }
            finally
            {
                Debug.Log("Receiver ended");
            }
        }

        private void TaskFunc_Sender()
        {
            try
            {
                Debug.Log("Sender started");
                while (SendQueue.TryDequeue(out byte[] sendData))
                {
                    if (!Client.Connected) continue;

                    Client.GetStream().Write(sendData);
                }
            }
            finally
            {
                Debug.Log("Sender ended");
            }
        }
        #endregion

        public void Dispose()
        {
            CloseConnection();
        }
    }
}

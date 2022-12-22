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
    public sealed class RCAS_Peer : MonoBehaviour
    {
        #region MEMBERS
        public static RCAS_Peer Instance {
            get{
                if(_Instance==null) _Instance = FindObjectOfType<RCAS_Peer>();
                return _Instance;
            }
            private set{
                _Instance = value;
            }
        }

        private static RCAS_Peer _Instance;

        public bool isConnected => TCP != null && TCP.isConnected;
        public bool isAwaitingConnection => TCP != null && TCP.isAwaitingConnection;

        public RCAS_TCP_Connection TCP;
        public RCAS_UDP_Connection UDP;
        public RCAS_UDP_Connection PAIRING;

        [Header("Device")]
        public bool isHost = false;
        public string deviceName = "HTC Vive Focus 3";

        [Header("Ports")]
        public int PairingPort = 27015;
        public bool AutoSetLocalPort = true;
        public int LocalPort = 27016;

        [Header("Pairing")]
        public bool startPairingFunctionOnStart = true;
        public bool startPairingFunctionOnDisconnect = false;
        public bool isPairing { get; private set; }

        private string _localIPAddress = "";
        public string localIPAddress
        {
            get
            {
                if (_localIPAddress == "") _localIPAddress = RCAS_NetworkUtils.GetLocalIPAddress();
                return _localIPAddress;
            }
        }
        private IPEndPoint LocalEndPoint_init => new IPEndPoint(IPAddress.Parse(localIPAddress), AutoSetLocalPort ? 0 : LocalPort);

        public IPEndPoint GetCurrentRemoteEndpoint()
        {
            // TODO: possible warning if not connected?
            return TCP.RemoteEndPoint;
        }
        public IPEndPoint GetCurrentLocalEndpoint()
        {
            // TODO: possible warning if not connected?
            return TCP.LocalEndPoint;
        }


        public Dictionary<string, List<System.Action<string[]>>> RemoteEvents = new Dictionary<string, List<System.Action<string[]>>>();
        #endregion

        #region MONOBEHAVIOUR
        private void Awake()
        {
            if(Instance)
            {
                Debug.LogError("A new RCAS_Peer object was instantiated whilst another already exists!. RCAS_Peer is a singleton object.");
                Destroy(this);
                return;
            }

            _Instance ??= this;

            TCP = new RCAS_TCP_Connection();
            UDP = new RCAS_UDP_Connection();
            PAIRING = new RCAS_UDP_Connection();

            TCP.OnConnectionEstablished += ConnectionEstablished;
            TCP.OnConnectionLost += ConnectionLost;
            TCP.OnReceivedMessage += ReceiveTCPMessage;
            UDP.OnReceivedMessage += ReceiveUDPMessage;

            RegisterAllRemoteEvents();
        }

        private void Start()
        {
            if (startPairingFunctionOnStart) BeginPairing();
        }

        private void Update()
        {
            PAIRING?.Update();
            UDP?.Update();
            TCP?.Update();
        }

        private void OnDestroy()
        {
            PAIRING?.Dispose();
            UDP?.Dispose();
            TCP?.Dispose();
        }
        #endregion

        #region NETWORKING
        public void OpenConnection() => TCP.OpenConnection(LocalEndPoint_init);

        public void ConnectTo(string IP, int port) => TCP.ConnectTo(IP, port, LocalEndPoint_init);

        public void CloseConnection() => TCP.CloseConnection();

        public void TriggerRemoteEvent(string EventName, string[] args) => TCP.SendMessage(RCAS_TCPMessage.EncodeRemoteEvent(EventName, args));
        public void TriggerRemoteEvent(string EventName, string args) => TriggerRemoteEvent(EventName, new string[] { args });
        public void TriggerRemoteEvent(string EventName) => TriggerRemoteEvent(EventName, "");

        public delegate void dOnReceivedPairingOffer(string IPAddress, int Port, string DeviceInfo);
        public dOnReceivedPairingOffer OnReceivedPairingOffer = delegate { };

        public delegate void dOnConnectionEstablished(IPEndPoint EP);
        public dOnConnectionEstablished OnConnectionEstablished = delegate { };

        public delegate void dOnConnectionLost(IPEndPoint EP);
        public dOnConnectionLost OnConnectionLost = delegate { };

        public delegate void dOnReceivedImage(RCAS_UDPMessage msg);
        public dOnReceivedImage OnReceivedImage = delegate { };

        public delegate void dOnReceivedTCPMessage(RCAS_TCPMessage msg);
        public dOnReceivedTCPMessage OnReceivedTCPMessage = delegate { };

        public delegate void dOnReceivedUDPMessage(RCAS_UDPMessage msg);
        public dOnReceivedUDPMessage OnReceivedUDPMessage = delegate { };

        public delegate void dOnBeginPairing();
        public dOnBeginPairing OnBeginPairing = delegate { };

        public void SendImage(byte[] raw_img_data) => UDP.SendImage(raw_img_data, GetCurrentRemoteEndpoint());

        public void SendTCPMessage(RCAS_TCPMessage msg) => TCP.SendMessage(msg);
        public void SendTcpMessage(string msg, RCAS_TCP_CHANNEL channel) => TCP.SendMessage(msg, channel);

        public void SendUDPMessage(RCAS_UDPMessage msg) => UDP.SendMessage(msg, GetCurrentRemoteEndpoint());
        public void SendUDPMessage(string msg, RCAS_UDP_CHANNEL channel) => UDP.SendMessage(msg, channel, GetCurrentRemoteEndpoint());
        public void BroadcastUDPMessage(RCAS_UDPMessage msg, int port) => UDP.BroadcastMessage(msg, port);
        public void BroadcastUDPMessage(string msg, RCAS_UDP_CHANNEL channel, int port) => UDP.BroadcastMessage(msg, channel, port);

        #endregion

        #region PAIRING
        public void BeginPairing()
        {
            if(isPairing)
            {
                Debug.LogWarning("Tried to start pairing-process on RCAS_Peer whilst it already is running. Aborting...");
                return;
            }
            Debug.Log("Commencing Device-Pairing.");

            isPairing = true;
            OnBeginPairing.Invoke();

            PAIRING.Connect(new IPEndPoint(IPAddress.Any, PairingPort));

            UDP.CloseConnection();
            TCP.CloseConnection();

            if (isHost)
            {
                TCP.OpenConnection(LocalEndPoint_init);
                StartCoroutine(StartDevicePairingBroadcast());
            }
            else
            {
                StartCoroutine(StartDevicePairingSearch());
            }
        }

        public void EndPairing()
        {
            if (!isPairing) return;

            isPairing = false;
            PAIRING.CloseConnection();
        }

        IEnumerator StartDevicePairingBroadcast()
        {
            yield return new WaitForSeconds(1);

            while (!isConnected)
            {
                yield return new WaitForSeconds(1);

                PAIRING.BroadcastMessage(RCAS_UDPMessage.EncodePairingOffer(
                    localIPAddress,
                    GetCurrentLocalEndpoint().Port,
                    deviceName),
                    PairingPort
                );
            }

            EndPairing();
        }


        IEnumerator StartDevicePairingSearch()
        {
            yield return new WaitForSeconds(1);

            PAIRING.OnReceivedMessage += ReceivePairingOffer;

            yield return new WaitUntil(() => isConnected);

            PAIRING.OnReceivedMessage -= ReceivePairingOffer;

            EndPairing();
        }
        #endregion

        #region CALLBACKS
        void ConnectionEstablished(EndPoint endpoint)
        {
            IPEndPoint EP = (IPEndPoint)endpoint;
            Debug.Log($"Connection established with: {EP.Address}:{EP.Port}");
            
            UDP.Connect(GetCurrentLocalEndpoint());
            this.OnConnectionEstablished.Invoke(EP);
        }

        void ConnectionLost(IPEndPoint EP)
        {
            Debug.Log($"Connection with {EP.Address}:{EP.Port} lost.");
            UDP.CloseConnection();

            if (startPairingFunctionOnDisconnect)
            {
                BeginPairing();
            }
            this.OnConnectionLost.Invoke(EP);
        }

        void ReceiveTCPMessage(RCAS_TCPMessage msg)
        {
            switch (msg.GetChannel())
            {
                case RCAS_TCP_CHANNEL.RESERVED_REMOTE_EVENT:
                    {
                        ExecuteRemoteEventLocally(msg);
                        break;
                    }
            }
            OnReceivedTCPMessage(msg);
        }

        void ReceiveUDPMessage(RCAS_UDPMessage msg)
        {
            switch (msg.GetChannel())
            {
                case RCAS_UDP_CHANNEL.RESERVED_PAIRING:
                    {
                        ReceivePairingOffer(msg);
                        break;
                    }
                case RCAS_UDP_CHANNEL.RESERVED_JPEG_STREAM:
                    {
                        OnReceivedImage(msg);
                        break;
                    }
            }
            OnReceivedUDPMessage(msg);
        }

        void ReceivePairingOffer(RCAS_UDPMessage msg)
        {
            if (msg.GetChannel() != RCAS_UDP_CHANNEL.RESERVED_PAIRING) return;

            (var v1, var v2, var v3) = RCAS_UDPMessage.DecodePairingOffer(msg);
            OnReceivedPairingOffer(v1, v2, v3);
        }
        #endregion

        #region REMOTEEVENTS
        private void RegisterAllRemoteEvents()
        {
            // Find any and all methods marked as "RemoteEvent" in all assemplies:
            var methodsMarked =
                from a in System.AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                from m in t.GetMethods(System.Reflection.BindingFlags.Static | (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                let attributes = m.GetCustomAttributes(typeof(RCAS_RemoteEvent), true)
                where attributes != null && attributes.Length > 0
                select new { method = m, attributes = attributes.Cast<RCAS_RemoteEvent>() };

            foreach (var method in methodsMarked)
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

        private void ExecuteRemoteEventLocally(RCAS_TCPMessage msg)
        {
            (string eventName, string[] args) = RCAS_TCPMessage.DecodeRemoteEvent(msg);

            if (RemoteEvents.ContainsKey(eventName))
            {
                foreach (var m in RemoteEvents[eventName]) m.Invoke(args);
            }
            else
            {
                Debug.LogError($"Tried to execute event {eventName}, but it does not exist on this machine!");
            }
        }
        #endregion
    }
}

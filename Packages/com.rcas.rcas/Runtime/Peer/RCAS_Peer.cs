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
        public static RCAS_Peer Instance;

        public bool isConnected => TCP != null && TCP.isConnected;
        public bool isAwaitingConnection => TCP != null && TCP.isAwaitingConnection;

        public RCAS_TCP_Connection TCP;
        public RCAS_UDP_Connection UDP;
        public RCAS_UDP_Connection PAIRING;

        public bool isHost = false;

        public bool startPairingFunctionOnStart = true;
        public bool startPairingFunctionOnDisconnect = false;
        public bool isPairing { get; private set; }

        public int PairingPort = 27015;

        public string deviceName = "HTC Vive Focus 3";

        private string _localIPAddress = "";
        public string localIPAddress
        {
            get
            {
                if (_localIPAddress == "") _localIPAddress = RCAS_NetworkUtils.GetLocalIPAddress();
                return _localIPAddress;
            }
        }

        //public IPEndPoint LocalEndPoint => TCP.LocalEndPoint;

        public IPEndPoint LocalEndPoint_init => new IPEndPoint(IPAddress.Parse(localIPAddress), 0);

        //public IPEndPoint RemoteEndpoint => TCP.RemoteEndPoint;

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

            Instance ??= this;

            TCP = new RCAS_TCP_Connection();
            UDP = new RCAS_UDP_Connection();
            PAIRING = new RCAS_UDP_Connection();

            TCP.OnConnectionEstablished += ConnectionEstablished;
            TCP.OnConnectionLost += ConnectionLost;
            TCP.OnReceivedMessage += ReceiveTCPMessage;
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


        public void SendImage(byte[] raw_img_data) => UDP.SendImage(raw_img_data, TCP.RemoteEndPoint);


        #endregion

        #region PAIRING
        public void BeginPairing()
        {
            if(isPairing)
            {
                Debug.LogWarning("Tried to start pairing-process on RCAS_Peer whilst it already is running.");
                return;
            }

            isPairing = true;
            
            PAIRING.Connect(new IPEndPoint(IPAddress.Any, PairingPort));

            if (isHost)
            {
                TCP.CloseConnection();
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
            // TODO: Stop PAIRING object
        }

        IEnumerator StartDevicePairingBroadcast()
        {
            yield return new WaitForSeconds(1);

            while (!isConnected)
            {
                yield return new WaitForSeconds(1);

                PAIRING.BroadcastMessage(RCAS_UDPMessage.EncodePairingOffer(
                    localIPAddress,
                    TCP.LocalEndPoint.Port,
                    deviceName),
                    PairingPort
                );
            }

            EndPairing();
        }


        IEnumerator StartDevicePairingSearch()
        {
            yield return new WaitForSeconds(1);
            RCAS_UDP_Connection.dOnReceivedPairingOffer callback = (m) =>
            {
                (var ip, var port, var info) = RCAS.RCAS_UDPMessage.DecodePairingOffer(m);
                this.OnReceivedPairingOffer.Invoke(ip, port, info);
            };

            PAIRING.OnReceivedPairingOffer += callback;

            yield return new WaitUntil(() => isConnected);

            PAIRING.OnReceivedPairingOffer -= callback;

            EndPairing();
        }
        #endregion

        #region CALLBACKS
        void ConnectionEstablished(EndPoint endpoint)
        {
            IPEndPoint EP = (IPEndPoint)endpoint;
            Debug.Log($"Connection established with: {EP.Address}:{EP.Port}");
            //RemoteEndpoint = EP;
            UDP.Connect(TCP.LocalEndPoint);
            this.OnConnectionEstablished.Invoke(EP);
        }

        void ConnectionLost(IPEndPoint EP)
        {
            Debug.Log($"Connection to {EP.Address}:{EP.Port} lost.");

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
                Debug.LogError("Event does not exist!");
            }
        }
        #endregion
    }
}

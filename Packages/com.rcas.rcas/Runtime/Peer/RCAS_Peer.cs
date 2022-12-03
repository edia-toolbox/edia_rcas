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

        public bool isHost = false;

        public bool startPairingFunctionOnStart = true;
        public bool startPairingFunctionOnDisconnect = false;
        public bool isPairing { get; private set; }

        public int LocalPort = 27015;
        public int RemotePort = 27016;

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

        public IPEndPoint LocalEndPoint { get; private set; } = null;

        public IPEndPoint RemoteEndpoint { get; private set; } = null;
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

            LocalEndPoint = new IPEndPoint(IPAddress.Parse(localIPAddress), LocalPort);


            TCP = new RCAS_TCP_Connection(this);
            UDP = new RCAS_UDP_Connection(this);

            TCP.OnConnectionEstablished += ConnectionEstablished;
            TCP.OnConnectionEstablished += (m) => this.OnConnectionEstablished.Invoke(m);
            TCP.OnConnectionLost += ConnectionLost;
            TCP.OnConnectionLost += () => this.OnConnectionLost.Invoke();
            UDP.OnReceivedPairingOffer += (m) =>
            {
                (var ip, var port, var info) = RCAS.RCAS_UDPMessage.DecodePairingOffer(m);
                this.OnReceivedPairingOffer.Invoke(ip, port, info);
            };
        }

        private void Start()
        {
            if (startPairingFunctionOnStart) BeginPairing();
        }

        private void Update()
        {
            UDP.Update();
            TCP.Update();
        }

        private void OnDestroy()
        {
            UDP.Dispose();
            TCP.Dispose();
        }
        #endregion

        #region NETWORKING
        public void OpenConnection() => TCP.OpenConnection();

        public void ConnectTo(string IP, int port) => TCP.ConnectTo(IP, port);

        public void CloseConnection() => TCP.CloseConnection();

        public void TriggerRemoteEvent(string EventName, string[] args) => TCP.SendRemoteEvent(EventName, args);
        public void TriggerRemoteEvent(string EventName, string args) => TCP.SendRemoteEvent(EventName, args);
        public void TriggerRemoteEvent(string EventName) => TCP.SendRemoteEvent(EventName);

        public delegate void dOnReceivedPairingOffer(string IPAddress, int Port, string DeviceInfo);
        public dOnReceivedPairingOffer OnReceivedPairingOffer = delegate { };

        public delegate void dOnConnectionEstablished(EndPoint endpoint);
        public dOnConnectionEstablished OnConnectionEstablished = delegate { };

        public delegate void dOnConnectionLost();
        public dOnConnectionLost OnConnectionLost = delegate { };


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
            if(isHost)
            {
                TCP.CloseConnection();
                TCP.OpenConnection();
                StartCoroutine(StartDevicePairingBroadcast());
            }
            else
            {
                StartCoroutine(StartDevicePairingSearch());
            }
        }

        IEnumerator StartDevicePairingBroadcast()
        {
            yield return new WaitForSeconds(1);

            UDP.StartSender();

            while (!isConnected)
            {
                yield return new WaitForSeconds(1);

                UDP.BroadcastMessage(RCAS_UDPMessage.EncodePairingOffer(
                    localIPAddress,
                    LocalPort,
                    deviceName)
                );
            }

            isPairing=false;
        }


        IEnumerator StartDevicePairingSearch()
        {
            yield return new WaitForSeconds(1);
            UDP.StartReceiver();

            yield return new WaitUntil(() => isConnected);

            isPairing = false;
        }
        #endregion

        #region CALLBACKS
        void ConnectionEstablished(EndPoint endpoint)
        {
            IPEndPoint EP = (IPEndPoint)endpoint;
            Debug.Log($"Connection established with: {EP.Address}:{EP.Port}");
            RemoteEndpoint = EP;
        }

        void ConnectionLost()
        {
            Debug.Log($"Connection to {RemoteEndpoint.Address}:{RemoteEndpoint.Port} lost.");

            RemoteEndpoint = null;

            if (startPairingFunctionOnDisconnect)
            {
                BeginPairing();
            }
        }
        #endregion
    }
}

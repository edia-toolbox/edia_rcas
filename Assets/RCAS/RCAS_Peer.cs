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

public sealed class RCAS_Peer : MonoBehaviour
{
    public static RCAS_Peer Instance;

    public bool isConnected => TCP!=null && TCP.isConnected;
    public bool isAwaitingConnection => TCP!=null && TCP.isAwaitingConnection;

    public RCAS_TCP_Connection TCP;
    public RCAS_UDP_Connection UDP;

    public bool isHost = false;

    public bool startPairingFunctionOnStart = true;
    public bool startPairingFunctionOnDisconnect = false; //TODO: this doesn't do anything yet

    public int Port = 27015;

    public string deviceName = "HTC Vive Focus 3";

    private string _localIPAddress = "";
    public string localIPAddress
    {
        get
        {
            if(_localIPAddress=="") _localIPAddress = RCAS_NetworkUtils.GetLocalIPAddress();
            return _localIPAddress;
        }
    }

    public IPEndPoint LocalEndPoint { get; private set; } = null;

    public IPEndPoint CurrentRemoteEndpoint { get; private set; } = null;

    private void Awake()
    {
        Instance ??= this;

        LocalEndPoint = new IPEndPoint(IPAddress.Parse(localIPAddress), Port);

        TCP = new RCAS_TCP_Connection(this);
        UDP = new RCAS_UDP_Connection(this);

        TCP.OnConnectionEstablished += OnConnectionEstablished;
    }

    private void Start()
    {
        if (startPairingFunctionOnStart)
        {
            if (isHost)
            {
                TCP.OpenConnection();
                StartCoroutine(StartDevicePairingBroadcast());
            }
            else
            {
                StartCoroutine(StartDevicePairingSearch());
            }
        }
    }

    private void Update()
    {
        UDP.Update();
        TCP.Update();
    }

    IEnumerator StartDevicePairingBroadcast()
    {
        yield return new WaitForSeconds(1);

        UDP.StartSender();

        while(!isConnected)
        {
            yield return new WaitForSeconds(1);

            UDP.BroadcastMessage(RCAS_UDPMessage.EncodePairingOffer(
                localIPAddress,
                Port,
                deviceName)
            );
        }
    }

    
    IEnumerator StartDevicePairingSearch()
    {
        yield return new WaitForSeconds(1);
        UDP.StartReceiver();

        yield return new WaitUntil(() => isConnected);

        // TODO:
        //UDP.EndReceiver();
    }

    void OnConnectionEstablished(EndPoint endpoint)
    {
        IPEndPoint EP = (IPEndPoint)endpoint;
        Debug.Log($"Connection established with: {EP.Address}:{EP.Port}");
        CurrentRemoteEndpoint = EP;
    }
}

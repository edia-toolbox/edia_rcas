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

    public int UDP_Port = 27015;
    public int TCP_Port = 27015;

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

    public PairingOffer_UIPanel UIPanel;

    public RCAS_RemotePeer CurrentRemotePeer { get; private set; } = null;

    private void Awake()
    {
        Instance ??= this;

        TCP = new RCAS_TCP_Connection(this);
        UDP = new RCAS_UDP_Connection(this);

        TCP.OnConnectionEstablished += OnConnectionEstablished;
    }

    private void Start()
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

        foreach (var m in TCP.RemoteEvents.Keys) Debug.Log(m);

        if (TCP.RemoteEvents.Keys.Count < 1) Debug.Log("Empty");
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

        RCAS_UDP_Channel pairing_channel = new RCAS_UDP_Channel(System.Net.IPAddress.Broadcast, UDP_Port, 0);

        while(!isConnected)
        {
            yield return new WaitForSeconds(1);

            //UDP.SendData(RCAS_NetworkUtils.GetLocalIPAddress(), pairing_channel);
            UDP.SendMessage(RCAS_UDPMessage.EncodePairingOffer(
                localIPAddress,
                TCP_Port, UDP_Port,
                deviceName),
            pairing_channel);
        }
    }

    IEnumerator StartDevicePairingSearch()
    {
        yield return new WaitForSeconds(1);
        UDP.StartReceiver();

        UDP.OnReceivedData += OnPairingOfferReceived;

        yield return new WaitUntil(() => isConnected);

        UDP.OnReceivedData -= OnPairingOfferReceived;
    }

    void OnPairingOfferReceived(byte[] data)
    {
        //Debug.Log($"DEVICE PAIRING OFFER: {Encoding.ASCII.GetString(data, 0, data.Length)}");

        if (data[0] != 0)
        {
            Debug.Log("Message is not a pairing offer");
            return;
        }

        (string ip_address, int tcp_port, int udp_port, string info) = RCAS_UDPMessage.DecodePairingOffer(new RCAS_UDPMessage(data));

        if (UIPanel)
        {
            UIPanel.gameObject.SetActive(true);
            UIPanel.IP_Text.text = $"IP: {ip_address}\nPORT: {tcp_port}/{udp_port}\n{info}";

            if(UIPanel.connectWasPressed)
            {
                UIPanel.connectWasPressed = false;
                TCP.ConnectTo(ip_address, tcp_port);
            }
        }
    }

    void OnConnectionEstablished(EndPoint endpoint)
    {
        IPEndPoint EP = (IPEndPoint)endpoint;
        Debug.Log($"Connection established with: {EP.Address}:{EP.Port}");
        CurrentRemotePeer = new RCAS_RemotePeer(EP.Address, EP.Port, EP.Port);
    }
}

public class RCAS_RemotePeer
{
    public RCAS_RemotePeer(IPAddress IpAddress, int TCP_Port, int UDP_Port)
    {
        this.IPAddress = IpAddress;
        this.TCP_Port = TCP_Port;
        this.UDP_Port = UDP_Port;
    }

    public IPAddress IPAddress;
    public int TCP_Port;
    public int UDP_Port;
}

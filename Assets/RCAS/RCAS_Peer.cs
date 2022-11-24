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

    public string ipAddress = "192.168.178.32";

    public int UDP_Port = 27015;
    public int TCP_Port = 27016;

    public PairingOffer_UIPanel UIPanel;

    private void Awake()
    {
        Instance ??= this;

        TCP = new RCAS_TCP_Connection();
        UDP = new RCAS_UDP_Connection(UDP_Port);
    }

    private void Start()
    {
        if (isHost)
        {
            TCP.OpenConnection(ipAddress, TCP_Port);
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
                RCAS_NetworkUtils.GetLocalIPAddress(),
                TCP_Port, UDP_Port,
                "HTC Vive Focus 3"),
            pairing_channel);
        }
    }

    IEnumerator StartDevicePairingSearch()
    {
        yield return new WaitForSeconds(1);
        UDP.StartReceiver();

        UDP.OnReceivedData += OnPairingOfferReceived;

        yield return new WaitUntil(() => isConnected);

        UDP.OnReceivedData += OnPairingOfferReceived;
    }

    void OnPairingOfferReceived(byte[] data)
    {
        Debug.Log($"DEVICE PAIRING OFFER: {Encoding.ASCII.GetString(data, 0, data.Length)}");

        if(UIPanel)
        {
            UIPanel.gameObject.SetActive(true);
            UIPanel.IP_Text.text = "IP: " + Encoding.ASCII.GetString(data, 0, data.Length);

            if(UIPanel.connectWasPressed)
            {
                UIPanel.connectWasPressed = false;
                TCP.ConnectTo("192.168.178.32", 27016);
            }
        }
    }
}

struct RCAS_RemotePeer
{
    IPAddress IPAddress;
    int TCP_Port;
    int UDP_Port;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RCAS;

public class PairingOffer_UIPanel : MonoBehaviour
{
    public RectTransform PairingOfferPanel;
    public TMPro.TMP_Text IP_Text;


    private string ip = "";
    private int port = 0;

    public void ConnectPressedd()
    {
        RCAS_Peer.Instance.TCP.ConnectTo(ip, port);
    }

    private void Start()
    {
        RCAS_Peer.Instance.UDP.OnReceivedPairingOffer += OnPairingOfferReceived;
        RCAS_Peer.Instance.TCP.OnConnectionEstablished += OnConnected;
        RCAS_Peer.Instance.TCP.OnConnectionLost += OnDisconnected;
    }

    private void OnDestroy()
    {
        RCAS_Peer.Instance.UDP.OnReceivedPairingOffer -= OnPairingOfferReceived;
        RCAS_Peer.Instance.TCP.OnConnectionEstablished -= OnConnected;
        RCAS_Peer.Instance.TCP.OnConnectionLost -= OnDisconnected;
    }

    void OnPairingOfferReceived(RCAS_UDPMessage msg)
    {
        if (RCAS_Peer.Instance.isConnected) return;

        (string ip_address, int port, string info) = RCAS_UDPMessage.DecodePairingOffer(msg);

        PairingOfferPanel.gameObject.SetActive(true);
        IP_Text.text = $"IP: {ip_address}\nPORT: {port}\n{info}";

        ip = ip_address;
        this.port = port;
    }

    void OnDisconnected()
    {
        PairingOfferPanel.gameObject.SetActive(false);
    }

    void OnConnected(System.Net.EndPoint EP)
    {
        PairingOfferPanel.gameObject.SetActive(false);
    }
}

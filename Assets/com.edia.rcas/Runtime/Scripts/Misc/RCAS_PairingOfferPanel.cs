using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Edia.Rcas.Samples
{
    public class RCAS_PairingOfferPanel : MonoBehaviour
    {
        public RectTransform PairingOfferPanel;
        public Text Output_Info;

        private string ip = "";
        private int port = 0;

        public void ConnectPressed()
        {
            RCAS_Peer.Instance.ConnectTo(ip, port);
        }

        private void Start()
        {
            RCAS_Peer.Instance.OnReceivedPairingOffer += PairingOfferReceived;
            RCAS_Peer.Instance.OnConnectionEstablished += Connected;
            RCAS_Peer.Instance.OnConnectionLost += Disconnected;
        }

        private void OnDestroy()
        {
            RCAS_Peer.Instance.OnReceivedPairingOffer -= PairingOfferReceived;
            RCAS_Peer.Instance.OnConnectionEstablished -= Connected;
            RCAS_Peer.Instance.OnConnectionLost -= Disconnected;
        }

        void PairingOfferReceived(string ip_address, int port, string deviceInfo)
        {
            if (RCAS_Peer.Instance.isConnected) return;

            PairingOfferPanel.gameObject.SetActive(true);
            Output_Info.text = $"IP: {ip_address}\nPORT: {port}\n{deviceInfo}";

            ip = ip_address;
            this.port = port;
        }

        void Disconnected(System.Net.EndPoint EP)
        {
            PairingOfferPanel.gameObject.SetActive(false);
        }

        void Connected(System.Net.EndPoint EP)
        {
            PairingOfferPanel.gameObject.SetActive(false);
        }
    }
}

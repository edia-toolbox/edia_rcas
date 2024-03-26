using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Edia.RCAS;
using Edia;
using TMPro;

namespace Edia.Controller
{
	public class PanelPairing : ExperimenterPanel
	{
		public Image icon;
		public TextMeshProUGUI Output_Info;

		string _ip = "";
		int _port = 0;
		string _deviceInfo = "None";

		public void BtnTSubmitPressed()
		{
			RCAS_Peer.Instance.ConnectTo(_ip, _port);
		}

		public override void Awake() {
			base.Awake();

			RCAS_Peer.Instance.OnReceivedPairingOffer += PairingOfferReceived;
			RCAS_Peer.Instance.OnConnectionEstablished += Connected;
			RCAS_Peer.Instance.OnConnectionLost += Disconnected;

			HidePanel();
		}


		private void OnDestroy()
		{
			RCAS_Peer.Instance.OnReceivedPairingOffer -= PairingOfferReceived;
			RCAS_Peer.Instance.OnConnectionEstablished -= Connected;
			RCAS_Peer.Instance.OnConnectionLost -= Disconnected;
		}

		void PairingOfferReceived(string ip_address, int port, string deviceInfo)
		{
			Output_Info.text = $"{deviceInfo}";
			_deviceInfo = deviceInfo;

			_ip = ip_address;
			this._port = port;

			ShowPanel();

			Debug.Log("Pairing recieved");
		}

		void Disconnected(System.Net.EndPoint EP)
		{
			Debug.Log("Disconnected");
			HidePanel();
		}

		void Connected(System.Net.EndPoint EP)
		{
			Debug.Log("Connected");
			EventManager.TriggerEvent(Edia.Events.ControlPanel.EvConnectionEstablished, new eParam(_deviceInfo));
			HidePanel();
		}
	}
}
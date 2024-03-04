using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using RCAS; 
using TMPro;

namespace eDIA.Manager
{
	/// <summary>Panel for setting up config files, for now choosing them from pre-set versions</summary>
	/// 
	public class PanelPairing : ExperimenterPanel
	{
		public Image icon;
		public TextMeshProUGUI Output_Info;

		private string ip = "";
		private int port = 0;
		private int deviceIndex = -1;

		public void BtnTSubmitPressed()
		{
			RCAS_Peer.Instance.ConnectTo(ip, port);
		}

		public override void Awake() {
			base.Awake();

			RCAS_Peer.Instance.OnReceivedPairingOffer += PairingOfferReceived;
			RCAS_Peer.Instance.OnConnectionEstablished += Connected;
			RCAS_Peer.Instance.OnConnectionLost += Disconnected;

			HidePanel();
		}

		private void Start()
		{
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

			//deviceIndex = ControlPanel.Instance.GetXRDeviceIndex(deviceInfo);

			//if (deviceIndex is not -1)
			//	icon.sprite = ControlPanel.Instance.GetXRDeviceIcon(deviceIndex);

			ip = ip_address;
			this.port = port;

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
			EventManager.TriggerEvent(eDIA.Events.ControlPanel.EvConnectionEstablished, new eParam(deviceIndex));
			HidePanel();
		}

	}
}
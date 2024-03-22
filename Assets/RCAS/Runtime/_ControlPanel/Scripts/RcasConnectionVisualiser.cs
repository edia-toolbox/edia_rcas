using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RCAS;
using Edia;
using UnityEngine.UI;
using System.Net;

namespace Edia.Manager {

	public class RcasConnectionVisualiser : MonoBehaviour
	{

		Image connectionIcon = null;

		private void Start()
		{
			connectionIcon = GetComponent<Image>();
			
			if (ControlPanel.Instance.Settings.ControlMode == ControlMode.Remote)
				RegisterEventListeners();
		}

		private void RegisterEventListeners () {
			RCAS_Peer.Instance.OnConnectionEstablished += Connected;
			RCAS_Peer.Instance.OnConnectionLost += Disconnected;
		}

		private void OnDestroy()
		{
			if (ControlPanel.Instance.Settings.ControlMode != ControlMode.Remote)
				return;

			RCAS_Peer.Instance.OnConnectionEstablished -= Connected;
			RCAS_Peer.Instance.OnConnectionLost -= Disconnected;
		}

		private void Disconnected(IPEndPoint EP)
		{
			connectionIcon.color = Color.red;
		}

		private void Connected(IPEndPoint EP)
		{
			connectionIcon.color = Color.green;
		}


	}
	
}

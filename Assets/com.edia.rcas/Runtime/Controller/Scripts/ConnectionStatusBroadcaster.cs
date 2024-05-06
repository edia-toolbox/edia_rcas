using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edia;
using Edia.Controller;
<<<<<<< Updated upstream
using Edia.Rcas;
using UnityEngine.UI;
using System.Net;

=======
using UnityEngine.UI;
using System.Net;

namespace Edia.RCAS {
>>>>>>> Stashed changes

public class ConnectionStatusBroadcaster : MonoBehaviour
{
	private void Start()
	{
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
		EventManager.TriggerEvent(Edia.Events.ControlPanel.EvConnectionEstablished, new eParam(false));
	}

	private void Connected(IPEndPoint EP)
	{
		EventManager.TriggerEvent(Edia.Events.ControlPanel.EvConnectionEstablished, new eParam(true));
	}


}

using UnityEngine;
using Edia;
using Edia.Controller;
using Edia.Rcas;
using System.Net;

public class ConnectionStatusBroadcaster : MonoBehaviour
{
	private void Start()
	{
		if (ControlPanel.Instance.ControlMode == Edia.Constants.ControlModes.Remote)
			RegisterEventListeners();
	}

	private void RegisterEventListeners () {
		RCAS_Peer.Instance.OnConnectionEstablished += Connected;
		RCAS_Peer.Instance.OnConnectionLost += Disconnected;
	}

	private void OnDestroy()
	{
		if (ControlPanel.Instance.ControlMode != Edia.Constants.ControlModes.Remote)
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

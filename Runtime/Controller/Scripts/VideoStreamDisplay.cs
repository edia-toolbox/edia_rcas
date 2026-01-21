using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edia;
using Edia.Controller;
using Edia.Rcas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoStreamDisplay : ExperimenterPanel {
		
	public Texture2D DisplayTexture;
		
	public override void Awake() {
		base.Awake();

		RCAS_Peer.Instance.OnConnectionEstablished += Connected;
		RCAS_Peer.Instance.OnConnectionLost += Disconnected;

		HidePanel();
	}

	private void Start () {
	}

	private void Connected (System.Net.EndPoint EP) {
		ShowPanel();

		RCAS_Peer.Instance.OnReceivedImage += OnReceiveNewFrame;
	}

	private void Disconnected (System.Net.EndPoint EP) {
		HidePanel();
			
		RCAS_Peer.Instance.OnReceivedImage -= OnReceiveNewFrame;
	}

	private void OnDestroy () {
		RCAS_Peer.Instance.OnReceivedImage -= OnReceiveNewFrame;
	}

	public void OnReceiveNewFrame (RCAS_UDPMessage msg) {
		if (!DisplayTexture || !RCAS_Peer.Instance.isConnected) return;

		if (!ImageConversion.LoadImage (DisplayTexture, msg.GetMessage ().ToArray ())) {
			Debug.LogError ("Could not load image from received texture data!");
		} else {
			DisplayTexture.Apply ();
		}
	}

	public void BtnPressed () {
				EventManager.TriggerEvent(Edia.Events.Casting.EvToggleCasting, null);
	}

}

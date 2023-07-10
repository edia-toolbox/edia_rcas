using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RCAS
{
	  sealed class RCAS_VideoReceiver : MonoBehaviour
	  {
			public Texture2D DisplayTexture;

			private void Start()
			{
				  if (!DisplayTexture)
				  {
						Debug.LogWarning("RCAS_VideoReceiver has no DisplayTexture set!");
				  }

				  RCAS_Peer.Instance.OnReceivedImage += OnReceiveNewFrame;
			}

			private void OnDestroy()
			{
				  RCAS_Peer.Instance.OnReceivedImage -= OnReceiveNewFrame;
			}

			public void OnReceiveNewFrame(RCAS_UDPMessage msg)
			{
				  if (!DisplayTexture || !RCAS_Peer.Instance.isConnected) return;

				  if (!ImageConversion.LoadImage(DisplayTexture, msg.GetMessage().ToArray()))
				  {
						Debug.LogError("Could not load image from received texture data!");
				  }
				  else
				  {
						DisplayTexture.Apply();
				  }
			}
	  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCAS_VideoReceiver : MonoBehaviour
{
    public Texture2D DisplayTexture;

    private void Start()
    {
        RCAS_Peer.Instance.UDP.OnReceivedImage += OnReceiveNewFrame;
    }

    private void OnDestroy()
    {
        RCAS_Peer.Instance.UDP.OnReceivedImage -= OnReceiveNewFrame;
    }

    public void OnReceiveNewFrame(RCAS_UDPMessage msg)
    {
        if (!RCAS_Peer.Instance.isConnected) return;

        if (!ImageConversion.LoadImage(DisplayTexture, msg.GetMessage().ToArray()))
        {
            Debug.LogError("???");
        }
        DisplayTexture.Apply();
    }
}

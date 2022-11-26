using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoReceiver : MonoBehaviour
{
    public Texture2D DisplayTexture;

    public static VideoReceiver Instance;

    public RCAS_Peer peer;

    private void Start()
    {
        Instance ??= this;

        peer.UDP.OnReceivedImage += OnReceiveNewFrame;
    }

    private void OnDestroy()
    {
        peer.UDP.OnReceivedImage -= OnReceiveNewFrame;
    }

    public void OnReceiveNewFrame(RCAS_UDPMessage msg)
    {
        if (!peer.isConnected) return;

        if (!ImageConversion.LoadImage(DisplayTexture, msg.GetMessage().ToArray()))
        {
            Debug.LogError("???");
        }
        DisplayTexture.Apply();
    }
}

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
        Instance = this;

        peer.UDP.OnReceivedData.AddListener(OnReceiveNewFrame);
    }

    public void OnReceiveNewFrame(byte[] newframe)
    {
        if (!peer.isConnected) return;

        if (!ImageConversion.LoadImage(DisplayTexture, newframe))
        {
            Debug.LogError("???");
        }
        DisplayTexture.Apply();
    }
}

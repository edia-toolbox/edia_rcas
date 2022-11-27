using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCAS_VideoSender : MonoBehaviour
{
    public RenderTexture mScreenCaptureTex;
    public Texture2D mStreamTexture;

    public float delta = 0.04f;

    
    void Start()
    {
        // Make sure this platform support what we need
        Debug.Assert(SystemInfo.copyTextureSupport.HasFlag(UnityEngine.Rendering.CopyTextureSupport.RTToTexture));

        StartCoroutine(CaptureAndSendScreen());
    }

    
    void Update()
    {
        
    }

    private IEnumerator CaptureAndSendScreen()
    {
        while(true)
        {
            yield return new WaitForSeconds(delta);
            yield return new WaitForEndOfFrame();

            // NOTE: We currently don't use the lines below because we've (temporarily) switched to a separate render-texture camera
            // Take screenshot
            //ScreenCapture.CaptureScreenshotIntoRenderTexture(mScreenCaptureTex);
            // TODO: Potentially use AsyncRequest to speed up ReadPixels: https://docs.unity3d.com/ScriptReference/ScreenCapture.CaptureScreenshotIntoRenderTexture.html

            /*
            Debug.Assert(SystemInfo.copyTextureSupport.HasFlag(UnityEngine.Rendering.CopyTextureSupport.RTToTexture));

            ScreenCapture.CaptureScreenshotIntoRenderTexture(m_screenTexture);

            Graphics.ConvertTexture(m_screenTexture, m_screenCopyTexture);
            //UnityEngine.Rendering.CommandBuffer cbuf = new UnityEngine.Rendering.CommandBuffer();
            //cbuf.ConvertTexture(m_screenTexture, m_screenCopyTexture);
            */

            // Copy to stream-texture
            Graphics.ConvertTexture(mScreenCaptureTex, mStreamTexture);


            // Retrieve stream-tex
            mStreamTexture.ReadPixels(new Rect(0, 0, mScreenCaptureTex.width, mScreenCaptureTex.height), 0, 0);
            // get data 
            //byte[] tex_data = mStreamTexture.EncodeToPNG();
            byte[] tex_data = mStreamTexture.EncodeToJPG(70);

            // TODO:
            // Send tex_data
            if (RCAS_Peer.Instance.isConnected)
            {
                //peer.UDP.SendData(tex_data, video_channel);
                RCAS_Peer.Instance.UDP.SendMessage(
                    RCAS_UDPMessage.EncodeImage(tex_data)
                );
            }
        }
    }
}

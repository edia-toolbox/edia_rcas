using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eDIA;
using System;

namespace RCAS
{
	  public class RCAS_VideoSender : MonoBehaviour
	  {
			public RenderTexture mRenderTexture;
			public TextureFormat mTextureFormat = TextureFormat.ARGB32;

			private Texture2D mSendTexture;

			public float fps = 25f;

			[Range(1, 100)]
			public int quality = 70;

			public bool IsAllowed = true;


			void Start()
			{
				  // Make sure this platform support what we need
				  Debug.Assert(SystemInfo.copyTextureSupport.HasFlag(UnityEngine.Rendering.CopyTextureSupport.RTToTexture));

				  mSendTexture = new Texture2D(
					  mRenderTexture.width,
					  mRenderTexture.height,
					  mTextureFormat,
					  false
				  );

				  StartCoroutine(CaptureAndSendScreen());

			}

			private void OnEnable()
			{
				  EventManager.StartListening(eDIA.Events.Casting.EvToggleCasting, OnEvEnableCasting);
			}

			private void OnDisable()
			{
				  EventManager.StopListening(eDIA.Events.Casting.EvToggleCasting, OnEvEnableCasting);
			}
			
			void OnEvEnableCasting(eParam param)
			{
				  IsAllowed = !IsAllowed;
			}

			private IEnumerator CaptureAndSendScreen()
			{
				  while (true)
				  {

						yield return new WaitForSeconds(1.0f / fps);
						yield return new WaitForEndOfFrame();

						if (IsAllowed)
						{
							  // NOTE: We currently don't use the lines below because we've (temporarily) switched to a separate render-texture camera
							  // Take screenshot
							  //ScreenCapture.CaptureScreenshotIntoRenderTexture(mRenderTexture);
							  // TODO: Potentially use AsyncRequest to speed up ReadPixels: https://docs.unity3d.com/ScriptReference/ScreenCapture.CaptureScreenshotIntoRenderTexture.html

							  /*
							  Debug.Assert(SystemInfo.copyTextureSupport.HasFlag(UnityEngine.Rendering.CopyTextureSupport.RTToTexture));

							  ScreenCapture.CaptureScreenshotIntoRenderTexture(m_screenTexture);

							  Graphics.ConvertTexture(m_screenTexture, m_screenCopyTexture);
							  //UnityEngine.Rendering.CommandBuffer cbuf = new UnityEngine.Rendering.CommandBuffer();
							  //cbuf.ConvertTexture(m_screenTexture, m_screenCopyTexture);
							  */

							  // Copy to stream-texture
							  Graphics.ConvertTexture(mRenderTexture, mSendTexture);

							  // Retrieve stream-tex
							  mSendTexture.ReadPixels(new Rect(0, 0, mRenderTexture.width, mRenderTexture.height), 0, 0);
							  // get data 
							  //byte[] tex_data = mSendTexture.EncodeToPNG();
							  byte[] tex_data = mSendTexture.EncodeToJPG(quality);

							  // Send tex_data
							  if (RCAS_Peer.Instance.isConnected)
							  {
									RCAS_Peer.Instance.SendImage(tex_data);
							  }


						}
				  }
			}
	  }
}

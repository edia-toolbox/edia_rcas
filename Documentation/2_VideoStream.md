
# Video Streaming

To set up video-streaming, simply add an `RCAS_VideoSender` and `RCAS_VideoReceiver` component respectively.

On the `RCAS_VideoSender` assign the RenderTexture, the contents of which will be sent over the network.
If you want to stream the view of a camera, simply assign the corresponding RenderTexture as the camera's output.

NOTE: The available resolution is highly limited, especially when the stream quality is high. Try starting with a small RenderTexture (like 200x200).

NOTE: The RenderTexture should (preferably) have no mipmaps and a R8G8B8A8_SRGB color format. Other formats may work aswell, but make sure they line up with the TextureFormat selected on the sender component.

On the `RCAS_VideoReceiver` assign a texture that the streamed images will be written to. It doesn't matter how big the texture is (it will automatically be resized to the resolution of the received image), make sure that the `Read/Write` flag is enabled.

All Video-streaming runs through the dedicated `RESERVED_JPEG_STREAM` UDP channel, so multiple video streams are not available without a custom messaging solution.


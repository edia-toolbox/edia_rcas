RCAS (Remote-Control-And-Streaming) is a Unity framework intended to enable remote control of an application running on a remote mobile VR headset in addition to live-streaming the VR view.

# Samples

For this package, two example scenes are provided.
1. Controller
2. Executer

## Controller
This scene is an example of the `controller` end.
It implements a few networked components:
- Interfacing RCAS events <> Internal events -> prefab: `[ Controller Peer ]` 
- Video stream reciever panel -> prefab: `[ PanelVideoStreamReceiver ]`
- Pairing offer -> prefab: `[ PanelPairingOffer ]`





## Executer
This scene is an example of the `execution` end.
It implements a few networked components:
- Interfacing RCAS events <> Internal events -> prefab: `[ Executer Peer ]` 
- Camera Capture streaming -> prefab: `[ CameraCaptureStreamer ]`

It executes the experiment and and expects network events from the `Controller` side, which get converted via the RCAS HMD interface.
On the other hand it sends a converted rendertexture from a dedicated streaming camera as bytes towards the `controller`.


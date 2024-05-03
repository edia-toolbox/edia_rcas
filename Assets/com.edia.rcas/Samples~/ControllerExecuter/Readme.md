RCAS (Remote-Control-And-Streaming) is a Unity framework enabling remote control of an VR application on a HMD. 

# Samples

For this package, two example scenes are provided.
1. Controller
2. Executer

RCAS is responsible for all networked interactions between `controller` and `executer` sides. RCAS makes it possible to do this on separate machines via network traffic. Currently only tested with Windows-based builds, but should be working as an Android APK too. Which gives opportunities for a tablet or even phone based app.

For details checkout the `Documentation` (in `[Assets/]com.edia.rcas`).

## Controller
This scene is an example of the `controller` end.

Main purpose of the `controller` is to be the controling side of the experiment (i.e., the interface for the experimenter). 

The controller side is responsible for:
- Act as network client which connects to the server
- Provide the possibility to connect
- Convert and send internal event over the network connection and viseversa
- Displaying streaming view from the VR user 

It implements a few networked components:
- Interfacing RCAS events <> Internal events -> prefab: `[ Controller Peer ]` 
- Video stream reciever panel -> prefab: `[ PanelVideoStreamReceiver ]`
- Pairing offer -> prefab: `[ PanelPairingOffer ]`

## Executer
This scene is an example of the `executer` end.

The `executer` is the execution side of the experiment, which runs on a VR capable device. 

The executer side is responsible for:
- Act as network server which the controller can connect to
- Convert and send internal event over the network connection and vice versa

It implements a few networked components:
- Interfacing RCAS events <> Internal events -> prefab: `[ Executer Peer ]` 
- Camera Capture streaming -> prefab: `[ CameraCaptureStreamer ]`

It executes the experiment and and expects network events from the `Controller` side, which get converted via the RCAS HMD interface.
On the other hand it sends a converted rendertexture from a dedicated streaming camera as bytes towards the `controller`.  
:warning: At the moment, turning on video streaming (can be de-/activated from the `controller` side) is pretty resource intensive and will most likely cause loss of performance (e.g., frame drops).


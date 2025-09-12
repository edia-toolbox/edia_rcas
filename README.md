<p align="center">
  <img src="./Assets/com.edia.rcas/Editor/Resources/Icons/IconRCAS.png" width="128" />
</p>

`EDIA Remote` (aka `RCAS` — Remote-Control-And-Streaming) is a Unity framework enabling remote control of an VR application on a HMD. 

# Samples

For this package, two example scenes are provided.
1. Controller
2. Executer

`EDIA Remote` is responsible for all networked interactions between `controller` and `executer` sides. It makes it possible to do this on separate machines via network traffic. Currently only tested with Windows-based builds, but should be working as an Android APK too. Which gives opportunities for a tablet or even phone based app.

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

## Description
This is the source project in which the package is a part.

## Technical Documentation
For details when it comes to setup, installation and usage, refer to the following documentation:
- [Intro](https://github.com/edia-toolbox/edia_rcas/blob/dev/Documentation/0_Readme.md)
- [Device-Pairing](https://github.com/edia-toolbox/edia_rcas/blob/dev//Documentation/1_DevicePairing.md)
- [Video-Streaming](https://github.com/edia-toolbox/edia_rcas/blob/dev//Documentation/2_VideoStream.md)
- [Remote-Events](https://github.com/edia-toolbox/edia_rcas/blob/dev//Documentation/3_RemoteEvents.md)
- [Custom-Messages](https://github.com/edia-toolbox/edia_rcas/blob/dev//Documentation/4_CustomMessages.md)


## Credits
Essential parts of the code in this package were written by [Benjamin Kahl](https://github.com/Helliaca) ([website](https://benjamin.kahl.fi/index.html)).  
If you are using this repository for your research or other public work, please cite the [EDIA Toolbox](https://github.com/edia-toolbox/edia_core/).

RCAS (Remote-Control-And-Streaming) is a Unity framework intended to enable remote control of an application running on a remote mobile VR headset in addition to live-streaming the VR view.

# Installation

To use RCAS in a Unity project, follow these steps:

- Download the tarball-package from [here](https://gitlab.gwdg.de/3dia/edia_rcas/-/raw/main/tarballs/com.rcas.rcas-0.1.0.tgz).
OR
- In the Unity Editor, navigate to `Window > Package Manager`.
- Click on the "+" in the top-left and click on `Add package from git URL`.
- Copy paste `https://gitlab.gwdg.de/3dia/edia_rcas.git?path=Assets/RCAS` in the field.

Alternatively, you can also clone this repository and open it as a new Unity project.

# Setup

Using RCAS requires you to build two separate projects, one for the VR headset and the other one for the Manager-app.
You can do this through two separate Unity projects, or by building the same project with different build-settings.

Either way, the important part is that each build needs a separate, active `RCAS_Peer` script in their respective scenes.
Do *not* use multiple Peers simultaneously within one game-instance. If you have scene changes, drag the Peer along through
`DontDestroyOnLoad(...)`.

The VR Headset should have its peer's `isHost` set to true, whilst the Manager set to false.
The `PairingPort` property will be used for automatic device-pairing.
You can also set a dedicated `LocalPort` through which all non-pairing network traffic will flow. Using the same LocalPort for both headset and manager will prevent you from being able to run (i.e. test) both applications locally on a single machine. It is recommended to enable `AutoSetLocalPort`, which will automatically select an available networking port for you.

<table><tr>
<th>VR Headset Peer</th>
<th>Manager App Peer</th>
</tr><tr><td>

![img](https://i.imgur.com/AMhHwCu.png)    
</td><td>
![img](https://i.imgur.com/PBe4925.png)
</td></tr>
</table>

# RCAS_Peer

Each Peer is a singleton object that has a TCP and UDP connection. These are used depending on the underlying use-case: TCP messages are used for reliable messages like RPCs/remote events, whilst UDP is used for live-streaming data, like the VR view.

Both connections have a series of channels, some of which are reserved for existing featrues, whilst other (channels named `CUSOM_X`) can be customized for your own use.

![img](https://i.imgur.com/j1Mn1MJ.png)

Each RCAS_Peers has several callback delegates you can use, including `Peer.OnEstablishedConnection` and `Peer.OnLostConnection`.

For details on each specific feature, consult the indicidual documenation:

- [Device-Pairing](https://gitlab.gwdg.de/3dia/edia_rcas/-/blob/main/Assets/com.edia.rcas/Documentation/1_DevicePairing.md)
- [Video-Streaming](https://gitlab.gwdg.de/3dia/edia_rcas/-/blob/main/Assets/com.edia.rcas/Documentation/2_VideoStream.md)
- [Remote-Events](https://gitlab.gwdg.de/3dia/edia_rcas/-/blob/main/Assets/com.edia.rcas/Documentation/3_RemoteEvents.md)
- [Custom-Messages](https://gitlab.gwdg.de/3dia/edia_rcas/-/blob/main/Assets/com.edia.rcas/Documentation/4_CustomMessages.md)

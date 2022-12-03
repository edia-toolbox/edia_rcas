RCAS (Remote-Control-And-Streaming) is a Unity framework intended to enable remote control of an application running on a remote mobile VR headset in addition to live-streaming the VR view.

# Installation

To use RCAS in a Unity project, follow these steps:

- Download the tarball-package from [here](https://gitlab.gwdg.de/3dia/edia_manager/-/raw/main/tarballs/com.rcas.rcas-0.1.0.tgz).
- In the Unity Editor, navigate to `Window > Package Manager`.
- Click on the "+" in the top-left and click on `Add package from tarball...`.
- Select the downloaded package and wait for the import to complete.

Alternatively, you can also clone this repository and open it as a new Unity project.

# Setup

Using RCAS requires you to build two separate projects, one for the VR headset and the other one for the Manager-app.
You can do this through two separate Unity projects, or by building the same project with different build-settings.

Either way, the important part is that each build needs a separate, active `RCAS_Peer` script in their respective scenes.
Do *not* use multiple Peers simultaneously within one game-instance. If you have scene changes, drag the Peer along through
`DontDestroyOnLoad(...)`.

The VR Headset should have its peer's `isHost` set to true, whilst the Manager set to false.
The Headsets's local port should be the Manager's remote port and vice-versa.

NOTE: You *can* set both the local and remote port to be the same value, but this will prevent you from running both host and client on the same machine (for local testing).

<table><tr>
<th>VR Headset Peer</th>
<th>Manager App Peer</th>
</tr><tr><td>
    
![img](https://i.imgur.com/CEmH0Vj.png)
</td><td>
![img](https://i.imgur.com/RwM98dD.png)
</td></tr>
</table>









Callbacks:

Peer.OnEstablishedConnection()
Peer.OnLostConnection()



Setting up video-Stream:

Always unreliable
VideoReceiver & VideoSender



Events:

Always reliable (TCP)
RCAS_Peer.TriggerRemoteEvent(...)
and [RemoteEvent(...)]




Additional Custom Messages:

Peer.TCP.SendMessage(...)
Peer.UDP.SendMessage(...)
Peer.UDP.BroadCastMessage(...)

Peer.TCP.OnReceiveMessage(...)
Peer.UDP.OnReceiveMessage(...)



Example: Setting up additional video stream:

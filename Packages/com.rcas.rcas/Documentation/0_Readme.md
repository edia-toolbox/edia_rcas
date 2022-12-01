

# Setup

- Download the tarball-package from [here](TODO).
- In Unity, go to `Window > Package Manager`.
- Click on the "+" in the top-left and click on `Add package from tarball...`.
- Select the downloaded package and wait for the import to complete.

Alternatively, clone this repository and open it as a new Unity project.


# Usage

To use RCAS you need an active `RCAS_Peer` script in your scene.
Do *not* have multiple Peers active simultaneously on one game-instance. If you change scenes, drag the Peer along through
`DontDestroyOnLoad(...)`.

The VR Headset should have its peer's `isHost` set to true, whilst the Manager set to false.
The Headsets's local port should be the Manager's remote port and vice-versa.

NOTE: You *can* set both Local and Remote ports to the same value, but this will prevent you from running both host and client on the same machine (for local testing).

TODO: Image of Host vs Client setup of Peer









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

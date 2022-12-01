

To use RCAS you need an active `RCAS_Peer` script in your scene.
Do *not* have multiple Peers active simultaneously on one game-instance. If you change scenes, drag the Peer along through
`DontDestroyOnLoad(...)`.

The VR Headset should have its peer's `isHost` set to true, whilst the Manager set to false.
The Headsets's local port should be the Manager's remote port and vice-versa.

NOTE: You *can* set both Local and Remote ports to the same value, but this will prevent you from running both host and client on the same machine (for local testing).

TODO: Image of Host vs Client setup of Peer




Events:

Peer.OnEstablishedConnection()
Peer.OnLostConnection()




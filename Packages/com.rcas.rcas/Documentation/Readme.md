

Connecting manually:

Peer.BeginHost()

Peer.ConnectTo(IP, port)



Connect through Pairing:

Peer.BeginPairing() // Client will start listening for other devices, Host will start boradcasting itself

Peer.OnReceivePairingOffer(ip, port, info)

Peer.ConnectTo(...)



Events:

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

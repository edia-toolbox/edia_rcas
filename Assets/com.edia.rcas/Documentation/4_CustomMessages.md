# Custom Messages

If you want to send custom, peer-to-peer messages, you should first think about whether you want to use the TCP or UDP networking protocol.

Use TCP for irregular, important messages that should reliably arrive at the remote peer, whilst UDP is more adequate for a continous stream of messages where individual packet-loss does not pose a big problem.

## Sending Messages

Sending a message is as simple as running one of the following functions:

```
RCAS_Peer.Instance.SendTCPMessage("MyMessage", RCAS_TCP_CHANNEL.CUSTOM_3);
RCAS_Peer.Instance.SendUDPMessage("MyMessage", RCAS_UDP_CHANNEL.CUSTOM_3);
RCAS_Peer.Instance.BroadcastUDPMessage("MyMessage", RCAS_UDP_CHANNEL.CUSTOM_3, Port);
```

Where the first parameter is the message, and the second the channel to send it over.
`BroadcastUDPMessage` sends the message to all devices on the network listening on the given port. The Pairing process uses `PairingPort` in this instance.

## Receiving Messages

Receiving and processing messages can be done through the `OnReceivedTCPMessage` and `OnReceivedUDPMessage` callbacks.

Below is a simple example which continuously synchronizes a random float variable over the `CUSTOM_2` TCP channel:

<table><tr>
<th>Receiver</th>
<th>Sender</th>
</tr><tr><td>

```csharp
using RCAS;

class ReliableVariableReceiver : Monobehaviour {

    float variable;

    void Start() {
        RCAS_Peer.Instance.OnReceivedUDPMessage += Receive;
    }

    void Receive(RCAS_UDPMessage msg) {
        if(msg.GetChannel() == RCAS_UDP_CHANNEL.CUSTOM_2) {
            variable = float.Parse(msg.GetMessageAsString());
        }
    }

}
```

</td><td>

```csharp
using RCAS;

class ReliableVariableSender : Monobehaviour {

    float variable;

    void Update() {
        variable = Random.Range(0, 100);
        RCAS_Peer.Instance.SendUDPMessage(variable.ToString(), RCAS_UDP_CHANNEL.CUSTOM_2);
    }

}
```

</td></tr>
</table>

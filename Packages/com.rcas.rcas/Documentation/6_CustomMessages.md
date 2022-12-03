Additional Custom Messages:

Peer.TCP.SendMessage(...)
Peer.UDP.SendMessage(...)
Peer.UDP.BroadCastMessage(...)

Peer.TCP.OnReceiveMessage(...)
Peer.UDP.OnReceiveMessage(...)



Example: 

We are syncing a float variable (reliably) over the CUSTOM_2 TCP channel

<table><tr>
<th>Receiver</th>
<th>Sender</th>
</tr><tr><td>
    
```csharp
using RCAS;

class ReliableVariableReceiver : Monobehaviour {

    float variable;

    void Start() {
        RCAS_Peer.Instance.TCP.OnReceivedMessage += Receive;
    }

    void Receive(RCAS_TCPMessage msg) {
        if(msg.GetChannel() == RCAS_TCP_CHANNEL.CUSTOM_2) {
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
        RCAS_Peer.Instance.TCP.SendMessage(variable.ToString(), RCAS_TCP_CHANNEL.CUSTOM_2);
    }

}
```
</td></tr>
</table>

For unrelaible, do the same but with UDP instead of TCP
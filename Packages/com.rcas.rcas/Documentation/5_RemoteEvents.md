



Events:

Always reliable (TCP)
RCAS_Peer.TriggerRemoteEvent(...)
and [RemoteEvent(...)]



<table><tr>
<th>Peer 1</th>
<th>Peer 2</th>
</tr><tr><td>
    
```csharp
using RCAS;

[RCAS_RemoteEvent("poke")]
static void Poke() {
    Debug.Log("You got poked!");
}

[RCAS_RemoteEvent("whisper")]
static void Whisper(string message) {
    Debug.Log("Someone whispers us a message: "+message);
}

[RCAS_RemoteEvent("set_params")]
static void SetParams(string [] args) {
    Debug.Log($"Parameters received: {args[0]}, {args[1]}, {args[2]}");
}
```
</td><td>
```csharp
using RCAS;

void PokeRemotePeer() {
    RCAS_Peer.Instance.TriggerRemoteEvent("poke");
}

void WhisperToRemotePeer() {
    RCAS_Peer.Instance.TriggerRemoteEvent("whisper", "hello there");
}

void SetParamsOnRemotePeer() {
    RCAS_Peer.Instance.TriggerRemoteEvent("set_params", new string[] { "12", "eleven", "Paris" });
}
```
</td></tr>
</table>
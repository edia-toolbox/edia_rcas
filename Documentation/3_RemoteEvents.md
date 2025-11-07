# Remote Events

Remote events let you run functions remotely to trigger certain actions.
RemoteEvents are triggered over TCP and are thus reliable.

To designate a remote event, simply mark a function with the `RCAS.RCAS_RemoteEvent` attribute. The function needs to be static.

```csharp
[RCAS_RemoteEvent("some_custom_event")]
static void MyCustomEventFunction() {
    // Run something locally
}
```

You can now run the marked function remotely by calling `RCAS_Peer.Instance.TriggerRemoteEvent("some_custom_event");`.

Remote Events can also take arguments in the form of a string, or an array of strings (see Caveat below). Here are a few examples:


<table><tr>
<th>Headset</th>
<th>Manager</th>
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

Caveat: Messages longer then 65kB are likely to produce problems. This also applies when sending arrays. Here the sum of the single elements' sizes should not exceed 65kB.

</td></tr>
</table>

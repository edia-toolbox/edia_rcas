# Device-Pairing

This section describes how two peers, running on separate instances can be connected over a network.

NOTE: You may need to set firewall exceptions for both inbound and outbound TCP and UDP traffic for each application. If you are running it through the Unity Editor, then you'll also have to set an exception for the Editor.

## Connecting Headset and Manager Manually

You can manually connect the VR app (i.e. the host) and the manager (i.e. the client) by running

```
RCAS_Peer.Instance.BeginHost();
```

on the headset, and then

```
RCAS_Peer.Instance.ConnectTo("192.168.178.42", 27016);
```

on the manager, with the respective IP-Address and port of the host-machine passed as parameters.

## Automatic Pairing

RCAS includes automatic pairing-functionality with which you can connect two instances without knowing their explicit IPs.

For a quick & dirty start:

- Enable the `Start Pairing Function...` options on both RCAS_Peer components.
- Simply  drag a `PairingOfferPanel` prefab, located under `Packages/RCAS/Runtime/Pairing`, onto a UI Canvas in the manager-app.

### Custom Pairing

If you want a more sophisticated pairing function with your own custom UI.

You can commence the pairing-process manually by running:

```
RCAS_Peer.Instance.BeginPairing();
```

NOTE: This is done automatically on start or disconnect if the respective `Start Pairing Function...` options on an RCAS_Peer are set.

Whilst pairing, the host will commence regular, 1-second UDP broadcasts that advertise the Peer.
On the client, this function will begin listening for exaclty these types of broadcasts.

Whenever one is received, the RCAS_Peer.OnReceivePairingOffer event is fired, after which you can run `ConnectTo` with the received IP and port.

A basic, custom implementation of pairing through the UI my look like so:

```csharp
public class PairingUI : MonoBehaviour
{
    public RectTransform PairingOfferPanel; // UI-Panel we display when receiving a pairing-offer. Includes a connect-button
    public Text IP_Text; // Text-component we write info (like IP, port etc.) into


    private string ip = "";
    private int port = 0;

    // This function is executed when the "Connect" button is pressed
    public void ConnectPressed()
    {
        RCAS_Peer.Instance.ConnectTo(ip, port);
    }

    private void Start()
    {
        RCAS_Peer.Instance.OnReceivedPairingOffer += PairingOfferReceived;
        RCAS_Peer.Instance.OnConnectionEstablished += Connected;
        RCAS_Peer.Instance.OnConnectionLost += Disconnected;
    }

    private void OnDestroy()
    {
        RCAS_Peer.Instance.OnReceivedPairingOffer -= PairingOfferReceived;
        RCAS_Peer.Instance.OnConnectionEstablished -= Connected;
        RCAS_Peer.Instance.OnConnectionLost -= Disconnected;
    }

    void PairingOfferReceived(string ip_address, int port, string deviceInfo)
    {
        if (RCAS_Peer.Instance.isConnected) return;

        PairingOfferPanel.gameObject.SetActive(true);
        IP_Text.text = $"IP: {ip_address}\nPORT: {port}\n{deviceInfo}";

        ip = ip_address;
        this.port = port;
    }

    void Disconnected(System.Net.EndPoint EP)
    {
        PairingOfferPanel.gameObject.SetActive(false);
    }

    void Connected(System.Net.EndPoint EP)
    {
        PairingOfferPanel.gameObject.SetActive(false);
    }
}
```

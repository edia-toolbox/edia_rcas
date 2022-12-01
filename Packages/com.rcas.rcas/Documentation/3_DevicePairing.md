## Connecting Headset and Manager Manually

You can connect the host and client easily by running

```
RCAS_Peer.Instance.BeginHost();
```

on the headset and 

```
RCAS_Peer.Instance.ConnectTo("192.168.178.42", 27016);
```

on the Manager, with the respective IP-Address and port of the host-machines passed as parameters.

## Connecting Headset and Manager through Pairing

RCAS includes automatic pairing-functionality if you want to connect two instances without knowing their explicit IPs.

On both the headset and the manager, run 

```
RCAS_Peer.Instance.BeginPairing();
```

On the host, this will begin regular, 1-second UDP broadcasts that advertise the Peer.
The the client, this function will begin listening for these types of broadcasts.

Whenever one is received, the RCAS_Peer.OnReceivePairingOffer event is fired, after which you can run `ConnectTo` with the received IP and port.


A fairly basic implementation of Pairing through the UI would look like so:

```
public class PairingUI : MonoBehaviour
{
    public RectTransform PairingOfferPanel;
    public TMPro.TMP_Text IP_Text;


    private string ip = "";
    private int port = 0;

    public void ConnectPressedd()
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

    void Disconnected()
    {
        PairingOfferPanel.gameObject.SetActive(false);
    }

    void Connected(System.Net.EndPoint EP)
    {
        PairingOfferPanel.gameObject.SetActive(false);
    }
}
```

If you want a quick-start, you can also use the `PairingOfferPanel` prefab located under `Packages/RCAS/Runtime/Pairing`. 
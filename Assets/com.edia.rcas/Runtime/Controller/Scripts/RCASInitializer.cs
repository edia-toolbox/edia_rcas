using Edia.Controller;
using UnityEngine;

public class RCASInitializer : MonoBehaviour
{
    public GameObject PairingPanelPrefab = null;

    private void Awake() {
        // Set controller of this scene to remote
        Edia.Controller.ControlPanel.Instance.ControlMode = Edia.Constants.ControlModes.Remote;
        
        // Instantiate the pairingpanel
        GameObject pairingPanel = Instantiate(PairingPanelPrefab, ControlPanel.Instance.NonActivePanelHolder.transform);
    }
}

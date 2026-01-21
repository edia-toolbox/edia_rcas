using Edia.Controller;
using UnityEngine;

public class RCASInitializer : MonoBehaviour
{
    private void Awake() {
        // Set controller of this scene to remote
        Edia.Controller.ControlPanel.Instance.ControlMode = Edia.Constants.ControlModes.Remote;
            
        // Instantiate the pairingpanel
        GameObject pairingPanel = Resources.Load ("Panel-PairingOffer") as GameObject;
        Instantiate(pairingPanel, ControlPanel.Instance.NonActivePanelHolder.transform);
    }
}

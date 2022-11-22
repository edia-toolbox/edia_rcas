using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PairingOffer_UIPanel : MonoBehaviour
{
    public TMPro.TMP_Text IP_Text;

    public bool connectWasPressed = false;

    public void ConnectPressedd()
    {
        connectWasPressed = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRemoteEvent : MonoBehaviour
{
    public void TriggerEvent(string eventName)
    {
        RCAS_Peer.Instance.TCP.SendMessage(eventName, 1);
    }
}

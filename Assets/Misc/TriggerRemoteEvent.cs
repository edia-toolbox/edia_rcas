using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRemoteEvent : MonoBehaviour
{
    public void TriggerEvent(string eventName)
    {
        RCAS_Peer.Instance.TCP.SendRemoteEvent(eventName);
    }

    public void TriggerEvent(string eventName, string arg)
    {
        RCAS_Peer.Instance.TCP.SendRemoteEvent(eventName, arg);
    }

    public void TriggerEvent(string eventName, string[] args)
    {
        RCAS_Peer.Instance.TCP.SendRemoteEvent(eventName, args);
    }

    public void TriggerEvent_Color_To_Red()
    {
        RCAS_Peer.Instance.TCP.SendRemoteEvent("change_color_to_red", "Hello World");
    }
}

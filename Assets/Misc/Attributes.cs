using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class RemoteEvent : Attribute
{
    private string event_name;

    public RemoteEvent(string event_name)
    {
        this.event_name = event_name;
    }

    public string getEventName() => event_name;
}

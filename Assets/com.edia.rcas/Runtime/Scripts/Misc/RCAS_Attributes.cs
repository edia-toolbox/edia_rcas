using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Edia.Rcas
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class RCAS_RemoteEvent : Attribute
    {
        private string event_name;

        public RCAS_RemoteEvent(string event_name)
        {
            this.event_name = event_name;
        }

        public string getEventName() => event_name;
    }
}

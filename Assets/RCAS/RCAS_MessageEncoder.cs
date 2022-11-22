using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public enum RCAS_MESSAGETYPE
{
    NONE = 0,
    REMOTE_EVENT = 1,
    // TODO ...
};

public ref struct RCAS_Message
{
    public Span<byte> raw_data;

    public RCAS_Message(string message, byte messageType)
    {
        raw_data = Encoding.ASCII.GetBytes((char)messageType + message);
    }

    public RCAS_Message(string message, RCAS_MESSAGETYPE messageType)
    {
        raw_data = Encoding.ASCII.GetBytes((char)messageType + message);
    }

    public RCAS_Message(byte[] raw_data)
    {
        this.raw_data = raw_data;
    }

    public ReadOnlySpan<byte> GetMessage()
    {
        return raw_data.Slice(1);
    }

    public string GetMessageAsString()
    {
        return Encoding.ASCII.GetString(GetMessage());
    }

    public RCAS_MESSAGETYPE GetMessageType()
    {
        return (RCAS_MESSAGETYPE)raw_data[0];
    }
}

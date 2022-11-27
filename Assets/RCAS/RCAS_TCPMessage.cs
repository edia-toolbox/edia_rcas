using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public enum RCAS_TCP_CHANNEL
{
    ROOT = 0,
    REMOTE_EVENT = 1,
    // TODO ...
};

public ref struct RCAS_TCPMessage
{
    public static char SEPARATOR = (char)17;

    public Span<byte> raw_data;

    public RCAS_TCPMessage(string message, byte channel)
    {
        raw_data = Encoding.ASCII.GetBytes((char)channel + message);
    }

    public RCAS_TCPMessage(string message, RCAS_TCP_CHANNEL channel)
    {
        raw_data = Encoding.ASCII.GetBytes((char)channel + message);
    }

    public RCAS_TCPMessage(string raw_data)
    {
        this.raw_data = Encoding.ASCII.GetBytes(raw_data);
    }

    public RCAS_TCPMessage(byte[] raw_data)
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

    public RCAS_TCP_CHANNEL GetChannel()
    {
        return (RCAS_TCP_CHANNEL)raw_data[0];
    }

    public static RCAS_TCPMessage EncodeRemoteEvent(string eventName, string[] args)
    {
        eventName = (char)RCAS_TCP_CHANNEL.REMOTE_EVENT + eventName + SEPARATOR + string.Join(SEPARATOR, args);
        return new RCAS_TCPMessage(eventName);

    }

    public static (string eventName, string[] args) DecodeRemoteEvent(RCAS_TCPMessage msg)
    {
        string strm = msg.GetMessageAsString();
        Span<string> strs = strm.Split(SEPARATOR);
        return (strs[0], strs.Slice(1).ToArray());
    }
}

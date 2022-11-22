using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public ref struct RCAS_UDPMessage
{
    public Span<byte> raw_data;

    public RCAS_UDPMessage(string message, RCAS_UDP_Channel channel)
    {
        raw_data = Encoding.ASCII.GetBytes((char)channel.channelID + message);
    }


}

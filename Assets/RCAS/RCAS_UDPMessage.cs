using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public ref struct RCAS_UDPMessage
{
    public Span<byte> raw_data;

    public static RCAS_UDPMessage EncodePairingOffer(string ip_address, int tcp_port, int udp_port, string info)
    {
        string message = (char)0 + ip_address + "&" + tcp_port + "&" + udp_port + "&" + info;
        return new RCAS_UDPMessage(Encoding.ASCII.GetBytes(message));
    }

    public static (string ip_address, int tcp_port, int udp_port, string info) DecodePairingOffer(RCAS_UDPMessage msg)
    {
        string str = Encoding.ASCII.GetString(msg.raw_data.Slice(1)); // slice of the channel-byte
        string[] strs = str.Split("&");

        return (strs[0], int.Parse(strs[1]), int.Parse(strs[2]), strs[3]);
    }

    public static RCAS_UDPMessage EncodeImage(byte[] img_data)
    {
        return new RCAS_UDPMessage(img_data);
    }

    public static byte[] DecodeImage(RCAS_UDPMessage msg)
    {
        return msg.raw_data.ToArray();
    }

    public RCAS_UDPMessage(byte[] raw_data)
    {
        this.raw_data = raw_data;
    }

    public RCAS_UDPMessage(string message, RCAS_UDP_Channel channel)
    {
        raw_data = Encoding.ASCII.GetBytes((char)channel.channelID + message);
    }
}

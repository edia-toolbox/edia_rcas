using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace RCAS
{
    public enum RCAS_UDP_CHANNEL
    {
        ROOT = 0,
        PAIRING = 1,
        JPEG_STREAM = 2,
        // TODO ...
    };

    public ref struct RCAS_UDPMessage
    {
        public static char SEPARATOR = (char)17;

        public Span<byte> raw_data;

        public ReadOnlySpan<byte> GetMessage()
        {
            return raw_data.Slice(1);
        }

        public RCAS_UDP_CHANNEL GetChannel()
        {
            return (RCAS_UDP_CHANNEL)raw_data[0];
        }

        public static RCAS_UDPMessage EncodePairingOffer(string ip_address, int port, string info)
        {
            string message = ip_address + SEPARATOR + port + SEPARATOR + info;
            return new RCAS_UDPMessage(Encoding.ASCII.GetBytes(message), RCAS_UDP_CHANNEL.PAIRING);
        }

        public static (string ip_address, int port, string info) DecodePairingOffer(RCAS_UDPMessage msg)
        {
            string str = Encoding.ASCII.GetString(msg.GetMessage()); // slice of the channel-byte
            string[] strs = str.Split(SEPARATOR);

            return (strs[0], int.Parse(strs[1]), strs[2]);
        }

        public static RCAS_UDPMessage EncodeImage(byte[] img_data)
        {
            return new RCAS_UDPMessage(img_data, RCAS_UDP_CHANNEL.JPEG_STREAM);
        }

        public static byte[] DecodeImage(RCAS_UDPMessage msg)
        {
            return msg.raw_data.ToArray();
        }



        public RCAS_UDPMessage(byte[] message, RCAS_UDP_CHANNEL channel)
        {
            var temp = new byte[message.Length + 1];
            Array.Copy(message, 0, temp, 1, message.Length);
            temp[0] = (byte)channel;
            this.raw_data = temp;
        }

        public RCAS_UDPMessage(byte[] raw_data)
        {
            this.raw_data = raw_data;
        }

        public RCAS_UDPMessage(string message, RCAS_UDP_CHANNEL channel)
        {
            raw_data = Encoding.ASCII.GetBytes((char)channel + message);
        }
    }
}

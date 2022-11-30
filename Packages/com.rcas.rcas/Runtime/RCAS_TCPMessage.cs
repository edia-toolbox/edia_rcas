using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace RCAS
{
    public enum RCAS_TCP_CHANNEL
    {
        RESERVED_INVALID = 0,
        RESERVED_REMOTE_EVENT = 1,
        CUSTOM_2 = 2,
        CUSTOM_3 = 3,
        CUSTOM_4 = 4,
        CUSTOM_5 = 5,
        CUSTOM_6 = 6,
        CUSTOM_7 = 7,
        CUSTOM_8 = 8,
        CUSTOM_9 = 9,
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

        public RCAS_TCPMessage(byte[] raw_data)
        {
            if (raw_data.Length < 1) this.raw_data = new byte[] { (byte)RCAS_TCP_CHANNEL.RESERVED_INVALID };
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
            eventName = eventName + SEPARATOR + string.Join(SEPARATOR, args);
            return new RCAS_TCPMessage(eventName, RCAS_TCP_CHANNEL.RESERVED_REMOTE_EVENT);

        }

        public static (string eventName, string[] args) DecodeRemoteEvent(RCAS_TCPMessage msg)
        {
            string strm = msg.GetMessageAsString();
            Span<string> strs = strm.Split(SEPARATOR);
            return (strs[0], strs.Slice(1).ToArray());
        }
    }
}

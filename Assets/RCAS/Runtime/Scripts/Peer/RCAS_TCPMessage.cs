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

    /// <summary>
    /// MESSAGE STRUCTURE:
    /// message-start-indicator: 1 byte   # NOTE: MUST BE 255 (all bits true) to be a valid message
    /// dlength: 2 bytes
    /// channel: 1 byte
    /// data: dlength bytes
    /// </summary>

    public ref struct RCAS_TCPMessage
    {
        public static char SEPARATOR = (char)17;

        public const uint HEADERSIZE = 4;
        private const string HEADER_CONCAT = "0000"; // This needs to be a string with HEADERSIZE chars

        public const byte VALIDMESSAGEBYTE = 255;

        public Span<byte> raw_data;

        public RCAS_TCPMessage(string message, byte channel)
        {
            raw_data = Encoding.ASCII.GetBytes(HEADER_CONCAT + message);
            raw_data[0] = VALIDMESSAGEBYTE;
            raw_data[1] = (byte)message.Length;
            raw_data[2] = (byte)(message.Length >> 8);
            raw_data[3] = channel;
        }

        public RCAS_TCPMessage(string message, RCAS_TCP_CHANNEL channel) : this(message, (byte)channel)
        {

        }

        public RCAS_TCPMessage(byte[] raw_data)
        {
            if (raw_data.Length < HEADERSIZE) this.raw_data = new byte[] { VALIDMESSAGEBYTE, (byte)0, (byte)0, (byte)RCAS_TCP_CHANNEL.RESERVED_INVALID };
            this.raw_data = raw_data;
        }

        public ReadOnlySpan<byte> GetMessage()
        {
            return raw_data.Slice(4);
        }

        public string GetMessageAsString()
        {
            return Encoding.ASCII.GetString(GetMessage());
        }

        public RCAS_TCP_CHANNEL GetChannel()
        {
            return (RCAS_TCP_CHANNEL)raw_data[3];
        }

        public uint GetMessageLength()
        {
            return (uint)(raw_data[1] | raw_data[2] << 8);
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

        public static bool TryMessageLengthFromReceivedBytes(ReadOnlySpan<byte> buffer, out uint messageLength)
        {
            if (buffer.Length < HEADERSIZE || buffer[0] != VALIDMESSAGEBYTE)
            {
                messageLength = 0;
                return false;
            }
            else
            {
                messageLength = (uint)(buffer[1] | buffer[2] << 8);
                return true;
            }
        }
    }
}

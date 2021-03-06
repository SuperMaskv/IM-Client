﻿namespace IM_Client.Protocol
{
    public abstract class Packet
    {
        public abstract byte getPacketType();

        public long token { get; set; }

        public byte getVersion()
        {
            return 1;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Protocol
{
    public abstract class Packet
    {
        public abstract byte getPacketType();
        public byte getVersion() { return 1; }
    }
}

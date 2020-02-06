using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 * 
 */
namespace IM_Client.Protocol
{
    public class PacketCodec
    {
        private static readonly short MAGIC = unchecked((short)0xabcd);
        public static PacketCodec INSTANCE = new PacketCodec();
        private static Dictionary<byte, dynamic> packetMap;

        private PacketCodec()
        {
            packetMap = new Dictionary<byte, dynamic>();
            packetMap.Add(Packet.NO_SERVER_LOGIN, new NoServerPacket.NoServerLoginPacket());
        }

        public byte[] Encode(Packet packet)
        {
            string packetJson = JsonConvert.SerializeObject(packet);
            byte[] packetbytes = Encoding.UTF8.GetBytes(packetJson);

            byte[] bytes = new byte[packetbytes.Length + 16];
            BitConverter.GetBytes(MAGIC).CopyTo(bytes, 0);
            bytes[2] = packet.VERSION;
            bytes[3] = packet.getPacketType();
            BitConverter.GetBytes(packetbytes.Length).CopyTo(bytes, 4);
            packetbytes.CopyTo(bytes, 8);

            return bytes;
        }

        public Packet Decode(byte[] bytes)
        {
            byte[] type = bytes.Skip(3).Take(1).ToArray();
            var packetTypeDef = GetPacketType(type[0]);

            byte[] lengthBytes = bytes.Skip(4).Take(4).ToArray();
            var length = BitConverter.ToUInt32(lengthBytes,0);
            
            byte[] msgBytes = new byte[length];
            bytes.CopyTo(msgBytes, 8);
            string packetJson = Encoding.UTF8.GetString(msgBytes);

            return JsonConvert.DeserializeAnonymousType(packetJson, packetTypeDef);
        }

        public Packet GetPacketType(byte type)
        {
            return packetMap[type];
        }
    }
}

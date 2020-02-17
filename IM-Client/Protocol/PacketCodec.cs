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
            packetMap.Add(PacketType.NO_SERVER_LOGIN, new NoServerPacket.NoServerLoginPacket());
            packetMap.Add(PacketType.NO_SERVER_LOGOUT, new NoServerPacket.NoServerLogoutPacket());
            packetMap.Add(PacketType.NO_SERVER_TEXT_MSG, new NoServerPacket.TextMessagePacket());
            packetMap.Add(PacketType.NO_SERVER_PIC_MSG, new NoServerPacket.NoServerPicMsgPacket());
            packetMap.Add(PacketType.NO_SERVER_SEND_FILE, new NoServerPacket.NoServerSendFilePacket());

            packetMap.Add(PacketType.SERVER_RESPONSE, new ServerPacket.ResponsePacket());
            packetMap.Add(PacketType.LOGIN, new ServerPacket.LoginPacket());
            packetMap.Add(PacketType.LOGOUT, new ServerPacket.LogoutPacket());
            packetMap.Add(PacketType.ADD_CONTACT, new ServerPacket.AddContactPacket());
            packetMap.Add(PacketType.REMOVE_CONTACT, new ServerPacket.RemoveContactPacket());
            packetMap.Add(PacketType.EDIT_CONTACT_ALIAS, new ServerPacket.EditContactAliasPacket());
        }

        public byte[] Encode(Packet packet)
        {
            string packetJson = JsonConvert.SerializeObject(packet);
            byte[] packetbytes = Encoding.UTF8.GetBytes(packetJson);

            byte[] bytes = new byte[packetbytes.Length + 8];
            byte[] magicBytes = BitConverter.GetBytes(MAGIC);
            Array.Reverse(magicBytes);
            Array.Copy(magicBytes, 0, bytes, 0, magicBytes.Length);

            bytes[2] = packet.getVersion();
            bytes[3] = packet.getPacketType();
            byte[] lengthBytes = BitConverter.GetBytes(packetbytes.Length);
            Array.Reverse(lengthBytes);
            Array.Copy(lengthBytes, 0, bytes, 4, lengthBytes.Length);

            Array.Copy(packetbytes, 0, bytes, 8, packetbytes.Length);

            return bytes;
        }

        public Packet Decode(byte[] bytes)
        {
            byte[] type = bytes.Skip(3).Take(1).ToArray();

            byte[] lengthBytes = bytes.Skip(4).Take(4).ToArray();
            Array.Reverse(lengthBytes);
            var length = BitConverter.ToUInt32(lengthBytes, 0);

            byte[] msgBytes = new byte[length];
            if (msgBytes.Length != bytes.Length - 8) return null;
            Array.Copy(bytes, 8, msgBytes, 0, length);
            string packetJson = Encoding.UTF8.GetString(msgBytes);

            return JsonConvert.DeserializeAnonymousType(packetJson, packetMap[type[0]]);
        }
    }
}
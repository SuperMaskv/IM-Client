namespace IM_Client.Protocol.ServerPacket
{
    public class LogoutPacket : Packet
    {
        public string userName { get; set; }

        public override byte getPacketType()
        {
            return PacketType.LOGOUT;
        }
    }
}
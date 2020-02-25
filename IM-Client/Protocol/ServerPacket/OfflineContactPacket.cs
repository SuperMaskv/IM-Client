namespace IM_Client.Protocol.ServerPacket
{
    public class OfflineContactPacket : Packet
    {
        public string userName { get; set; }

        public override byte getPacketType()
        {
            return PacketType.OFFLINE_CONTACT;
        }
    }
}
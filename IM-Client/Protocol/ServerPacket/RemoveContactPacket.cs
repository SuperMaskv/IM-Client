namespace IM_Client.Protocol.ServerPacket
{
    public class RemoveContactPacket : Packet
    {
        public string userName { get; set; }
        public string contactName { get; set; }

        public override byte getPacketType()
        {
            return PacketType.REMOVE_CONTACT;
        }
    }
}
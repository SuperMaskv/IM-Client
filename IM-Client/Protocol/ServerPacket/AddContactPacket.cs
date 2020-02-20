namespace IM_Client.Protocol.ServerPacket
{
    public class AddContactPacket : Packet
    {
        public string userName { get; set; }
        public string contactName { get; set; }
        public string alias { get; set; }


        public override byte getPacketType()
        {
            return PacketType.ADD_CONTACT;
        }
    }
}
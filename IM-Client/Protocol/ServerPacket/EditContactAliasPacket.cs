namespace IM_Client.Protocol.ServerPacket
{
    public class EditContactAliasPacket : Packet
    {
        public string userName { get; set; }
        public string contactName { get; set; }
        public string alias { get; set; }

        public override byte getPacketType()
        {
            return PacketType.EDIT_CONTACT_ALIAS;
        }
    }
}
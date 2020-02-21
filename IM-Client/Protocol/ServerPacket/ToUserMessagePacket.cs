namespace IM_Client.Protocol.ServerPacket
{
    public class ToUserMessagePacket : Packet
    {
        public string msgSender { get; set; }
        public string msgRecipient { get; set; }
        public string msgContent { get; set; }
        public byte[] photo { get; set; }


        public override byte getPacketType()
        {
            return PacketType.TO_USER_MESSAGE;
        }
    }
}
namespace IM_Client.Protocol.ServerPacket
{
    public class LoginPacket : Packet
    {
        public string userName { get; set; }
        public string userPwd { get; set; }

        public override byte getPacketType()
        {
            return PacketType.LOGIN;
        }
    }
}
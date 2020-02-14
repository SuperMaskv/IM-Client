namespace IM_Client.Protocol.NoServerPacket
{
    public class NoServerLoginPacket : Packet
    {
        public string UserName { get; set; }
        public byte[] Avator { get; set; }
        public int Port { get; set; }

        //表示是否为应答报文
        public bool IsReply { get; set; }

        public override byte getPacketType()
        {
            return PacketType.NO_SERVER_LOGIN;
        }
    }
}
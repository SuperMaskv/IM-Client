namespace IM_Client.Protocol.NoServerPacket
{
    public class NoServerSendFilePacket : Packet
    {
        public string FileName { get; set; }

        public int Port { get; set; }

        public bool IsSend { get; set; }

        public bool WillSend { get; set; }

        public string Author { get; set; }


        public override byte getPacketType()
        {
            return PacketType.NO_SERVER_SEND_FILE;
        }
    }
}
namespace IM_Client.Protocol.ServerPacket
{
    public class FileTransferPacket : Packet
    {
        public string fileSender { get; set; }
        public string fileReceiver { get; set; }
        public bool requestFlag { get; set; }
        public string receiverAddress { get; set; }
        public int receiverPort { get; set; }
        public string fileName { get; set; }
        public bool agreeFlag { get; set; }

        public override byte getPacketType()
        {
            return PacketType.FILE_TRANSFER;
        }
    }
}
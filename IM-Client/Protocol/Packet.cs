namespace IM_Client.Protocol
{
    public abstract class Packet
    {
        public abstract byte getPacketType();

        public byte getVersion()
        {
            return 1;
        }
    }
}
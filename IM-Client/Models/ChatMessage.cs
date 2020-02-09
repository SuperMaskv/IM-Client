using System;

namespace IM_Client.Models
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public string Author { get; set; }
        public DateTime Time { get; set; }
        public byte[] Picture { get; set; }
        public bool IsOriginNative { get; set; }
    }
}
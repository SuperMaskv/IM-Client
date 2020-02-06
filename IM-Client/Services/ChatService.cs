using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace IM_Client.Services
{
    public class ChatServices:IChatServices
    {
        private UdpClient sendUdpClient;
        private UdpClient reciveUdpClient;

        private event Action<string> _noServerLogin;

    }
}

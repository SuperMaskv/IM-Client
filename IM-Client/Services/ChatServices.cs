using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Services
{
    public class ChatServices:IChatServices
    {
        private event Action<string> _noServerLogin;

        public ChatServices()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Models
{
    public class User
    {
        public string UserName { get; set; }
        public int ID { get; set; }
        public byte[] Photo { get; set; }
    }
}

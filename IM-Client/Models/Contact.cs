using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Models
{
    public class Contact
    {
        public string userName { get; set; }
        public string contactName { get; set; }
        public string alias { get; set; }
        public byte[] photo { get; set; }
    }
}

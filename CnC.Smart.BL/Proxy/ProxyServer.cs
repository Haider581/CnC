using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Smart.BL.Proxy
{
    public class ProxyServer
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public string Scheme { get; set; }
    }
}

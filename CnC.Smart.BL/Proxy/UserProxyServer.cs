using CC.Smart.BL.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CC.Smart.BL.Proxy
{
    public class UserProxyServer
    {
        public ProxyServer ProxyServer { get; set; }
        public User User { get; set; }
    }
}
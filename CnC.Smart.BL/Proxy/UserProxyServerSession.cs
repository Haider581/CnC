using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Smart.BL.Proxy
{
    public class UserProxyServerSession
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiredOn { get; set; }
        public int ProxyServerId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Smart.BL.Proxy
{
    public class ProxyService
    {
        public ProxyServer GetProxyServer(int userId)
        {
            return new ProxyServer() { Id = 1, IpAddress = "35.156.182.156", Port = 3128
                , UserName = "athar", Password = "p@kistan786", Scheme = "http", IsEnabled = true };
        }

        /// <summary>
        /// Save User Session Info in DB on User Connectivity
        /// Login
        /// </summary>
        public void CreateUserSession(int userId)
        {
            // Create User Session in DB on User Connectivity
        }

        /// <summary>
        /// Update User Session Info in DB on User Disconnectivity
        /// Logout
        /// </summary>        
        public void KillUserSession(int userId)
        {
            // Update User Session in DB on User Disconnectivity
        }

        public List<string> GetRestrictedWebsites()
        {
            List<string> restrictedWebsites = new List<string>();
            restrictedWebsites.Add("facebook.com");
            restrictedWebsites.Add("youtube.com");
            return restrictedWebsites;
        }

        /// <summary>
        /// Log User Browsing History
        /// </summary>        
        public void LogUserBrowsedWebsite(int userId, string url)
        {

        }
    }
}

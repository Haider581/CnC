using CC.Smart.BL.Accounts;
using CC.Smart.BL.Proxy;
using SmartApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SmartApi.Controllers
{
    public class ProxyController : ApiController
    {
        [Route("api/Proxy/ProxyServer")]
        [HttpGet]
        //[Authorize]
        public UserProxyServer GetProxyServer()
        {
            UserProxyServer userProxyServer = new UserProxyServer();
            User user = new User();
            user.Id = 1;
            user.Username = "Demo";
            userProxyServer.User = user;
            ProxyService proxyService = new ProxyService();
            userProxyServer.ProxyServer = proxyService.GetProxyServer(user.Id);
            return userProxyServer;
        }

        [Route("api/Proxy/RestrictedWebsites")]
        [HttpGet]
        public List<string> GetRestrictedWebsites()
        {
            ProxyService proxyService = new ProxyService();
            return proxyService.GetRestrictedWebsites();
        }

        [Route("api/Proxy/CreateUserSession")]
        [HttpGet]
        public void CreateUserSession(int userId)
        {
            ProxyService proxyService = new ProxyService();
            proxyService.CreateUserSession(userId);
        }

        [Route("api/Proxy/KillUserSession")]
        [HttpGet]
        public void KillUserSession(int userId)
        {
            ProxyService proxyService = new ProxyService();
            //HttpContext.Current.user
            proxyService.KillUserSession(userId);
        }        
    }
}

using CnC.Core.Exceptions;
using CnC.Core.Proxy;
using CnC.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Service
{
    public class ProxyService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(ProxyService));

        public ProxyServer GetProxyServer(int userId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var proxyServerAndUser = (from ps in context.ProxyServers
                                       join psu in context.ProxyServerUsers on ps.Domain equals psu.Domain
                                       where psu.UserId == userId
                                       && ps.Active == true
                                       select new { ProxyServer = ps, ProxyServerUser = psu }).SingleOrDefault();

                    if (proxyServerAndUser == null)
                    {
                        var leastUsedproxyServer = context.ProxyServers.Where(ps => ps.Active).Select(ps =>
                         new { ProxyServer = ps, Count = context.ProxyServerUsers.Count(psu => psu.Domain == ps.Domain) })
                        .OrderBy(ps => ps.Count).FirstOrDefault();

                        if (leastUsedproxyServer == null)
                            throw new Exception("Proxy Server not found");

                        var proxyServer = leastUsedproxyServer.ProxyServer;

                        var proxyServerUser = context.ProxyServerUsers.SingleOrDefault(psu => psu.UserId == userId);

                        if (proxyServerUser == null)
                        {
                            proxyServerUser = new ProxyServerUser();
                            proxyServerUser.CreatedOn = DateTime.Now;
                            proxyServerUser.Domain = proxyServer.Domain;
                            proxyServerUser.LastAccessedOn = DateTime.Now;
                            proxyServerUser.UserId = userId;

                            context.ProxyServerUsers.Add(proxyServerUser);
                        }
                        else
                        {
                            proxyServerUser.Domain = proxyServer.Domain;
                            proxyServerUser.LastAccessedOn = DateTime.Now;
                        }

                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to save");

                        return proxyServer;
                    }
                    else
                    {
                        proxyServerAndUser.ProxyServerUser.LastAccessedOn = DateTime.Now;

                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to save");

                        return proxyServerAndUser.ProxyServer;
                    }
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                throw new Exception("Unable to get Proxy Server");
            }
        }        
    }
}

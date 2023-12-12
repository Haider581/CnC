using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CnC.Service.Test
{
    [TestClass]
    public class ProxyServiceTest
    {        
        [TestMethod]
        public void GetProxyServer_Test()
        {
            var proxyServer = new ProxyService().GetProxyServer(3);
            if (proxyServer == null)
                Assert.Fail();

            proxyServer = new ProxyService().GetProxyServer(4);
            if (proxyServer == null)
                Assert.Fail();

            proxyServer = new ProxyService().GetProxyServer(5);
            if (proxyServer == null)
                Assert.Fail();
        }
    }
}

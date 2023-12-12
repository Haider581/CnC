using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CnC.Service.Test
{
    [TestClass]
    public class ExchangeRateServiceTest
    {        
        [TestMethod]
        public void GetExchangeRate_Test()
        {
            if (new ExchangeRateService().GetExchangeRate(2) == null)
                Assert.Fail();
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CnC.Service.Test
{
    [TestClass]
    public class TopUpServiceTest
    {
        [TestMethod]
        public void GetTopUpRequestFormsPendingForProcessing_Test()
        {
            int totalCount = 0;
            new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, customerId: 80);
        }

        [TestMethod]
        public void GetTopUpRequestFormsUnderProcessing_Test()
        {
            int totalCount = 0;
            new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, customerId: 80);
        }

        //[TestMethod]
        //public void GetTopUpRequestForms_Test()
        //{
        //    int totalCount = 0;
        //    new TopUpService().GetTopUpRequestForms(out totalCount, customerId: 80);
        //}

        [TestMethod]
        public void GetTopUpRequestFormsHistory_Test()
        {
            int totalCount = 0;
            new TopUpService().GetTopUpRequestForms(out totalCount, customerId: 80, topUpRequestStatus: Core.TopUpRequestStatus.History);
        }
    }
}

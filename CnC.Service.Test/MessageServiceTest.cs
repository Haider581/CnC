using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CnC.Core.Accounts;
using CnC.Core;
using CnC.Core.Cards;
using System.Collections.Generic;
using CnC.Core.Customers;

namespace CnC.Service.Test
{
    [TestClass]
    public class MessageServiceTest
    {        
        //[TestMethod]
        //public void SendCustomerWelcomeMessage_Test()
        //{
        //    User user = new User();
        //    user.Email = "ounabbas_me@hotmail.com";
        //    user.FirstName = "Oun";
        //    user.LastName = "Abbas";
        //    user.Username = "TestUser";

        //    var messageService = new MessageService();

        //    int queuedEmailId = messageService.SendCustomerWelcomeMessage(user, "123", 2, false);
        //    if (queuedEmailId <= 0)
        //        Assert.Fail();

        //    var queuedEmail = messageService.GetQueuedEmail(queuedEmailId);

        //    if (queuedEmail.To != user.Email)
        //        Assert.Fail();
        //}
        [TestMethod]
        public void SendChangePasswordMessage_Test()
        {
            User user = new User();
            user.Email = "ounabbas_me@hotmail.com";
            user.FirstName = "Oun";
            user.LastName = "Abbas";
            user.Username = "TestUser";

            var messageService = new MessageService();

            int queuedEmailId = messageService.SendChangePasswordMessage(user, 2);
            if (queuedEmailId <= 0)
                Assert.Fail();

            var queuedEmail = messageService.GetQueuedEmail(queuedEmailId);

            if (queuedEmail.To != user.Email)
                Assert.Fail();
        }
        [TestMethod]
        public void SendCustomerNewCardRequestMessage_Test()
        {
            User user = new User();
            user.Email = "ounabbas_me@hotmail.com";
            user.FirstName = "Oun";
            user.LastName = "Abbas";
            user.Username = "TestUser";

            var messageService = new MessageService();
            var customer = new Customer();
            var requestForm = new RequestForm();
            var cardRequest = new List<CardRequest>();
            int queuedEmailId = messageService.SendCustomerNewCardRequestMessage(customer, requestForm, cardRequest, 2);
            if (queuedEmailId <= 0)
                Assert.Fail();

            var queuedEmail = messageService.GetQueuedEmail(queuedEmailId);

            if (queuedEmail.To != user.Email)
                Assert.Fail();
        }


    }
}

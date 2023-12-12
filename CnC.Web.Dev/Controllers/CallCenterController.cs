using CnC.Core.Caching;
using CnC.Core.Customers;
using CnC.Service;
using CnC.Web.Helper;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CnC.Web.Controllers
{
    public class CallCenterController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CallCenterController));

        [HttpGet]
        [RoleActionValidator]
        public ActionResult Customer()
        {
            var customers = new List<Customer>();
            return View(customers);
        }

        [HttpPost]
        [RoleActionValidator]
        public ActionResult Customer(Customer customer)
        {
            try
            {
                int totalCount = 0;

                return View(new CustomerService().
                    GetCustomers(out totalCount, nic: customer.NIC,
                    passportNo: customer.PassportNo, contactNo: customer.ContactNo));
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [RoleActionValidator]
        public ActionResult CustomerCardRequestDetails(string customerId)
        {
            try
            {
                if (!string.IsNullOrEmpty(customerId))
                {
                    customerId = new Utility().UrlSafeDecrypt(customerId);
                    int totalCount = 0;
                    if (!string.IsNullOrEmpty(customerId))
                        new CachingProvider().Set("CallCenterCustomer", customerId);
                    return View("~/Views/Card/CustomerCardRequests.cshtml", new CardService()
                        .GetCardRequestForms(out totalCount, customerId: Convert.ToInt32(customerId)));
                }
                return View();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [RoleActionValidator]
        public ActionResult CustomerCardsDetails(string customerId)
        {
            try
            {
                int totalCount = 0;

                if (!string.IsNullOrEmpty(customerId))
                {
                    customerId = new Utility().UrlSafeDecrypt(customerId);
                    new CachingProvider().Set("CallCenterCustomer", customerId);

                    return View("~/Views/Card/CustomerCards.cshtml", new CardService()
                                .GetCards(out totalCount, customerId: Convert.ToInt32(customerId)));
                }
                return View();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [RoleActionValidator]
        public ActionResult CustomerTopUpDetails(string customerId)
        {
            try
            {
                int totalCount = 0;
                if (!string.IsNullOrEmpty(customerId))
                {
                    customerId = new Utility().UrlSafeDecrypt(customerId);
                    new CachingProvider().Set("CallCenterCustomer", customerId);

                    return View("~/Views/TopUp/CustomerTopUpRequestsProcessing.cshtml", new TopUpService().
                            GetTopUpRequestFormsUnderProcessing(out totalCount, customerId: Convert.ToInt32(customerId)));
                }
                return View();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
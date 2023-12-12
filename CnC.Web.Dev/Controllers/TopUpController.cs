using CnC.Core.TopUps;
using CnC.Core;
using CnC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Helper;
using CnC.Core.Payments;
using CnC.Core.Accounts;
using CnC.Core.Exceptions;
using CnC.Core.Customers;
using CnC.Web.Models;
using CnC.Service.PaymentGateway.Mabna;
using System.Configuration;
using log4net;

namespace CnC.Web.Controllers
{
    [Authorize]
    public class TopUpController : Controller
    {
        ILog log = LogManager.GetLogger(typeof(TopUpController));

        [RoleActionValidator]
        public ActionResult TopUpView()
        {
            return View();
        }

        [RoleActionValidator]
        public ActionResult GetTopUpHistory()
        {
            var userSession = Session["CurrentSession"] as CnC.Core.Accounts.User;
            int totalCount = 0;
            var topUpService = new TopUpService();
            ViewBag.exchangeRate = new CnC.Web.Controllers.ExchangeRateController().GetExchangeRate(98);
            return View(topUpService.GetTopUpRequestForms(out totalCount, customerId: userSession.Id, topUpRequestStatus: CnC.Core.TopUpRequestStatus.History));
        }

        [RoleActionValidator]
        public ActionResult TopUpHistory(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var userSession = Session["CurrentSession"] as User;
            var topUpRequestsForms = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: userSession.Id
                , topUpRequestStatus: TopUpRequestStatus.History, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;

            if (userSession == null) return View();
            return View(topUpRequestsForms);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult TopUpHistory(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;

            string cardNo = null;
            int? cardTypeId = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            string nic = null;
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardTypeId = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }
            int pageSize = new SettingService().ResultPageSize;
            var topUpRequests = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: userSession.Id, cardTypeId: cardTypeId
                   , cardNumber: cardNo, nic: nic, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo,
                   topUpRequestStatus: TopUpRequestStatus.History, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(topUpRequests);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult GetTopUpHistory(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;

            string cardNo = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int? cardType = null;
            int totalCount = 0;
            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardType = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            var topUpRequests = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: userSession.Id, cardNumber: cardNo
                , createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);
        }

        [RoleActionValidator]
        public ActionResult GetTopUpRequests()
        {
            //TODO: on session expiry redirect to login page
            var userSession = Session["CurrentSession"] as User;
            int totalCount = 0;
            var topUpService = new TopUpService();
            return View(topUpService.GetTopUpRequestForms(out totalCount, customerId: userSession.Id));
        }

        [RoleActionValidator]
        [AllowAnonymous]
        public ActionResult TopUpRequests(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var topUpRequestsForms = new TopUpService().GetTopUpRequestForms(out totalCount, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(topUpRequestsForms);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult TopUpRequests(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string cardNo = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int? cardTypeId = null;
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardTypeId = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            int pageSize = new SettingService().ResultPageSize;
            var topUpRequests = new TopUpService().GetTopUpRequestForms(out totalCount, nic: nic, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo
                , pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(topUpRequests);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult GetTopUpRequests(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;

            string nic = null;
            string cardNo = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int? cardTypeId = null;
            int totalCount = 0;
            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardTypeId = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            var topUpRequests = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: userSession.Id, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);

        }

        [RoleActionValidator]
        public List<TopUpRequestForm> GetTopupRequestForms(int customerId)
        {
            int totalCount = 0;
            try
            {
                return new TopUpService().GetTopUpRequestForms(out totalCount, customerId: customerId);
            }
            catch (Exception exception)
            {
                return null;
            }
        }
        public class MyCardsViewModel
        {
            public string cardId { get; set; }
            public string cardNo { get; set; }
            public string cardAmount { get; set; }
        }

        public class TransactionViewModel
        {
            public string CustomerId { get; set; }
            public string TransactionNo { get; set; }
            public string AccountNo { get; set; }
            public string Name { get; set; }
            public string Amount { get; set; }
            public string SubmitActionName { get; set; }
            public List<MyCardsViewModel> Cards { get; set; }
        }

        /// <summary>
        /// New Top up request pop up postback handler
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        #region New Topup Request
        [RoleActionValidator]
        [HttpGet]
        public ActionResult NewTopupRequest()
        {
            return View();
        }

        [RoleActionValidator]
        [HttpPost]
        public JsonResult AddTopupRequest(TransactionViewModel transaction)
        {
            // Request Form 
            var requestForm = new RequestForm();
            // decimal serviceFee = new ServiceFeeService().GetServiceFee(ServiceType.TopUpUrgent).Percentage;
            requestForm.Type = RequestFormType.TopUp;
            requestForm.TypeId = (int)RequestFormType.TopUp;
            requestForm.ExchangeRate = new ExchangeRateService()
                .GetExchangeRate(new SettingService().CustomerDefaultCurrency.Id).Rate;
            requestForm.CustomerId = Convert.ToInt32(transaction.CustomerId);

            // Card request
            List<TopUpRequest> topupRequestList = new List<TopUpRequest>();

            bool cardPurchased = false;

            decimal totalAmount = 0;//serviceFee;

            foreach (var cardItem in transaction.Cards)
            {
                decimal amount = 0;
                decimal.TryParse(cardItem.cardAmount, out amount);

                if (amount > 0)
                {
                    cardPurchased = true;

                    TopUpRequest topupRequest = new TopUpRequest();
                    topupRequest.CardId = int.Parse(cardItem.cardId);
                    topupRequest.Amount = amount;

                    topupRequestList.Add(topupRequest);
                    totalAmount = totalAmount + topupRequest.Amount;
                }
            }

            // Calculate service charges 
            //decimal serviceFee = (serviceFeePercent * totalAmount / 100);

            //requestForm.ServiceFee = serviceFee;
            //totalAmount = totalAmount + serviceFee;

            // Payment
            List<Payment> payments = new List<Payment>();
            payments.Add(new Payment()
            {
                TransactionAccountNo = transaction.AccountNo,
                TransactionName = transaction.Name,
                TransactionNo = transaction.TransactionNo,
                Amount = totalAmount,
                PaymentMethod = PaymentMethod.Bank,
                PaymentMethodId = (int)PaymentMethod.Bank
            });

            int res = new TopUpService().AddTopUpRequestForm(requestForm, topupRequestList, payments);

            return Json(res, JsonRequestBehavior.AllowGet);
        }


        #endregion

        /// <summary>
        /// Will use for Dashboard
        /// </summary>
        /// <param name="topUpRequestStatus"></param>
        /// <returns></returns>
        public int GetTopUpRequestsCountByStatus(CnC.Core.TopUpRequestStatus topUpRequestStatus, int? customerId = null)
        {
            var topUpService = new TopUpService();
            return topUpService.GetTopUpRequestsCountByStatus(topUpRequestStatus);
        }

        #region Customer 

        [RoleActionValidator]
        [HttpPost]
        public ActionResult CustomerTopUpRequests(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;

            string cardNo = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int? cardType = null;
            int totalCount = 0;
            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardType = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new TopUpService().GetTopUpRequestForms(out totalCount, customerId: userSession.Id, cardNumber: cardNo
                , createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        [RoleActionValidator]
        public ActionResult CustomerTopUpHistory()
        {
            int totalCount = 0;
            if (Session["CurrentSession"] != null)
            {
                var userSession = Session["CurrentSession"] as User;
                var topUpHistory = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: userSession.Id, topUpRequestStatus: TopUpRequestStatus.History);
                return View(topUpHistory);
            }
            return View();
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult CustomerTopUpHistory(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;

            string nic = null;
            string cardNo = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int? cardTypeId = null;
            int totalCount = 0;
            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardTypeId = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            var topUpRequests = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: userSession.Id, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo
                , topUpRequestStatus: TopUpRequestStatus.History);

            return View(topUpRequests);
        }


        [RoleActionValidator]
        [ActionName("PaymentGatewayResponse")]
        [HttpPost]
        public ActionResult CustomerNewTopUpRequest()
        {
            var userSession = Session["CurrentSession"] as User;

            if (Session["TempNewTopUpRequest"] == null)
                return Redirect("~/TopUp/CustomerNewTopUpRequest" + "?tempArgu="
                    + new Utility().UrlSafeEncrypt(userSession.Id.ToString()));
            int gateWayId = (int)RequestFormType.TopUp;
            IPaymentGateway paymentGateway = new PaymentGatewayInfoService().GetActivePaymentGateway(gateWayId);
            var response = paymentGateway.GetResponse(Request);

            if (!response.IsSuccess)
            {
                TempData["ResultCode"] = "";
                TempData["Description"] = response.Message;
                return Redirect("~/TopUp/CustomerNewTopUpRequest" + "?tempArgu="
                    + new Utility().UrlSafeEncrypt(userSession.Id.ToString()));
            }

            try
            {
                var newTopUpRequest = Session["TempNewTopUpRequest"] as CustomerNewRequestContainer;

                var totalAmtIncludingAllCharges = Convert.ToInt32(newTopUpRequest.Payments[0].Amount);

                newTopUpRequest.Payments[0].Amount = Convert.ToInt32(newTopUpRequest.Payments[0].Amount);
                newTopUpRequest.Payments[0].TransactionNo = response.TransactionNumber;

                int requestFormId = new TopUpService().AddTopUpRequestForm(newTopUpRequest.RequestFormObj,
                    newTopUpRequest.TopUpRequestCollection, newTopUpRequest.Payments);

                if (requestFormId > 0)
                {
                    Session["TempNewTopUpRequest"] = null;
                    TempData["ResultCode"] = "200";
                    TempData["Description"] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Top Up request added successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/TopUp/CustomerNewTopUpRequest'>Go back</a></span>";
                    return Redirect("~/TopUp/CustomerNewTopUpRequest");
                }
                else
                {
                    Session["TempNewTopUpRequest"] = null;
                    return Redirect("~/TopUp/CustomerNewTopUpRequest?tempArgu="
                        + new Utility().UrlSafeEncrypt(userSession.Id.ToString())); // redirect to topup request page
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return RedirectToAction("Login", "Account");
            }
        }

        [RoleActionValidator]
        [HttpGet]
        public ActionResult CustomerNewTopUpRequest(int tempArgu = 0)
        {
            ViewBag.ResultCode = TempData["ResultCode"];
            ViewBag.Description = TempData["Description"];

            return View();
        }

        [RoleActionValidator]
        [HttpPost]
        public JsonResult CustomerNewTopupRequest(TransactionViewModel transaction)
        {
            //Basic Validation on transation object
            var customerSession = Session["CurrentCustomer"] as Customer;
            LocalizationService localizationService = null;
            if (customerSession == null)
            {
                localizationService = new LocalizationService();
            }
            else
            {
                localizationService = new LocalizationService(customerSession.LanguageId);
            }
            var statusArray = new string[3];
            if (transaction.SubmitActionName != null && transaction.SubmitActionName != "Online")
            {
                if (transaction.Cards == null || transaction.Cards.Count <= 0 || !transaction.Cards.Any())
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid request.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Also check IsCardExist
                    if (!transaction.Cards.Any(amt => amt.cardAmount != null && amt.cardAmount != ""))
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Invalid request.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }

                }
                if (string.IsNullOrEmpty(transaction.AccountNo) || !Utility.IsAlphaNumeric(transaction.AccountNo) || transaction.AccountNo.Length <= 0 || transaction.AccountNo.Length > 20)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid account.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(transaction.TransactionNo) || !Utility.IsAlphaNumeric(transaction.TransactionNo) || transaction.TransactionNo.Length <= 0 || transaction.TransactionNo.Length > 20)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid transaction reference no.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(transaction.Name) || !Utility.IsAlphabetWithSpace(transaction.Name) || transaction.Name.Length < 4 || transaction.Name.Length > 40)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid Name.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(transaction.Amount) || !Utility.IsNumeric(transaction.Amount) || transaction.Amount.Length <= 0 || transaction.Amount.Length > 10)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid Amount.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            if (string.IsNullOrEmpty(transaction.CustomerId) || !Utility.IsNumeric(transaction.CustomerId) ||
                transaction.CustomerId.Length <= 0 || transaction.CustomerId.Length > 10)
            {
                statusArray[0] = "101";
                statusArray[1] = "Invalid CustomerId please try again or reload page.";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }

            var exchangeRate =
                new ExchangeRateService().GetExchangeRate(new SettingService().CustomerDefaultCurrency.Id);

            // Request Form 
            var requestForm = new RequestForm
            {
                Type = RequestFormType.TopUp,
                TypeId = (int)RequestFormType.TopUp,
                ExchangeRate = exchangeRate.Rate
            };

            requestForm.CustomerId = Convert.ToInt32(transaction.CustomerId);
            // Top Up Request
            var topupRequestList = new List<TopUpRequest>();
            decimal totalAmount = 0;
            try
            {

                var userSession = Session["CurrentSession"] as User;
                if (userSession != null)
                {
                    //var tempCards = new CardService().GetCardsReloadable(userSession.Id);
                    var tempCards = new CardService().GetCardsReloadable(Convert.ToInt32(transaction.CustomerId));

                    foreach (var cardItem in transaction.Cards)
                    {
                        decimal amount = 0;
                        decimal.TryParse(cardItem.cardAmount, out amount);
                        if (amount > 0)
                        {
                            var amtRestruction = tempCards.SingleOrDefault(x => x.CardRequestId == Convert.ToInt32(cardItem.cardId)); //item.CardRequest.CardType.ReloadLimit
                            if (amtRestruction?.CardRequest.CardType != null && (Convert.ToDecimal(cardItem.cardAmount) >= amtRestruction.CardRequest.CardType.MinimumReloadAtATime
                                                                                 && Convert.ToDecimal(cardItem.cardAmount) <= amtRestruction.CardRequest.CardType.MaximumReloadAtATime))
                            {
                                var topupRequest = new TopUpRequest();
                                topupRequest.CardId = int.Parse(cardItem.cardId);
                                topupRequest.Amount = amount;

                                var serviceFee = amount * amtRestruction.CardRequest.CardType.TopUpServiceFeePercentage / 100;
                                topupRequest.ServiceFee = amtRestruction.CardRequest.CardType.TopUpServiceFeeMinimum >
                                    serviceFee ? amtRestruction.CardRequest.CardType.TopUpServiceFeeMinimum : serviceFee;
                                topupRequest.ServiceExchangeRateFee = new SettingService().ExchangeRateServiceCharges;

                                topupRequestList.Add(topupRequest);
                                totalAmount = totalAmount + topupRequest.Amount + topupRequest.ServiceFee;
                            }
                            else
                            {
                                statusArray[0] = "101";
                                statusArray[1] = "Invalid reload amount.";
                                return Json(statusArray, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Unable to process the request";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }

                // Calculate service charges 
                //decimal serviceFee = (serviceFeePercent * totalAmount / 100);
                //totalAmount = totalAmount + serviceFee;

                //Put check that local currency amount entered in textbox must be equal or greater then total amount of new topup
                var tempActualAmt = totalAmount * exchangeRate.Rate;
                tempActualAmt = tempActualAmt + ((tempActualAmt / 100) * new SettingService().ExchangeRateServiceCharges); //Calculate Exchange rate charges and then add these charges in actual amount.

                // Payment
                var payments = new List<Payment>();
                if (transaction.SubmitActionName == "Online")
                {
                    payments.Add(new Payment
                    {
                        Amount = tempActualAmt,
                        PaymentMethod = PaymentMethod.Online,
                        PaymentMethodId = (int)PaymentMethod.Online
                    });
                }
                else
                {
                    var tempToVerifyAmt = Convert.ToInt32(transaction.Amount);
                    if (tempToVerifyAmt < tempActualAmt)
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Invalid amount entered in Amount Textbox.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    payments.Add(new Payment
                    {
                        TransactionAccountNo = transaction.AccountNo,
                        TransactionName = transaction.Name,
                        TransactionNo = transaction.TransactionNo,
                        Amount = tempToVerifyAmt,
                        PaymentMethod = PaymentMethod.Bank,
                        PaymentMethodId = (int)PaymentMethod.Bank
                    });
                }

                if (transaction.SubmitActionName == "Online")
                {
                    var amtt = Convert.ToInt32(payments[0].Amount);

                    IPaymentGateway paymentGateway = new PaymentGatewayInfoService()
                        .GetActivePaymentGateway(requestForm.TypeId);

                    PaymentGatewayResponse responsePaymentService = paymentGateway.GenerateToken(amtt
                        , DateTime.Now.ToString("yyMMddHHmmss"), DateTime.Now.ToString("yyMMddHHmmss")
                        , Request.Url.Scheme + "://" + Request.Url.Authority
                        + "/TopUp/PaymentGatewayResponse");

                    if (responsePaymentService.IsSuccess)
                    {
                        var tempContainer = new CustomerNewRequestContainer();
                        tempContainer.RequestFormObj = requestForm;
                        tempContainer.TopUpRequestCollection = topupRequestList;
                        tempContainer.Payments = payments;
                        Session["TempNewTopUpRequest"] = tempContainer;

                        statusArray[0] = "205"; // in case of url redidrect set status 205
                        statusArray[1] = responsePaymentService.Message;
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        statusArray[0] = "206";
                        statusArray[1] = responsePaymentService.Message;
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    int requestFormId = new TopUpService().AddTopUpRequestForm(requestForm, topupRequestList, payments);

                    if (requestFormId > 0)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i>" + localizationService.GetResource("Cnc.Complete", null, "Complete") + "</strong></span><br />" + localizationService.GetResource("Cnc.TopUpRequestAddedSuccessfully", null, "Top Up request added successfully!") + "<br /><a id='lkGoBack' style='font-size: 14px; ' href='/TopUp/CustomerNewTopUpRequest'>" + localizationService.GetResource("Cnc.GoBack", null, "Go Back") + "</a></span>";
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to process the request";
                    }
                }
            }
            catch (UserException exp)
            {
                statusArray[0] = "101";
                statusArray[1] = "Failed: " + exp.Message;
            }
            catch (Exception exp)
            {
                statusArray[0] = "101";
                statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }




        #endregion

        #region TBO Agent

        [RoleActionValidator]
        public ActionResult TopUpRequestsSearch()
        {
            int totalCount = 0;
            return View(new TopUpService().GetTopUpRequestForms(out totalCount));
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult TopUpRequestsSearch(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;

            string nic = null;
            string cardNo = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int? cardTypeId = null;
            int totalCount = 0;
            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardTypeId = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            var topUpRequests = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: userSession.Id, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);
        }

        [RoleActionValidator]
        public ActionResult Validation()
        {
            return View();
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult Validation(FormCollection fc)
        {
            return View();
        }

        [RoleActionValidator]
        public ActionResult CustomerTopUpRequestsProcessing(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var userSession = Session["CurrentSession"] as User;
            var customerTopUpRequetsUnderProcessing = new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, customerId: userSession.Id, pageIndex: page, pageSize: pageSize, isForCustomer: true);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            if (userSession == null) return View();
            return View(customerTopUpRequetsUnderProcessing);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult CustomerTopUpRequestsProcessing(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;
            string cardNo = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int? cardTypeId = null;
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardTypeId = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }
            int pageSize = new SettingService().ResultPageSize;
            var topUpRequests = new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, customerId: userSession.Id, cardNumber: cardNo
               , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(topUpRequests);
        }

        [RoleActionValidator]
        public ActionResult PaymentConfirmation(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var cardRequestFormsPendingForPaymentConfirmationOrFailed = new TopUpService()
                .GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount,
                pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(cardRequestFormsPendingForPaymentConfirmationOrFailed);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult PaymentConfirmation(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;

            string nic = null;
            string cardNo = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int? cardTypeId = null;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardTypeId = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var topUpRequests = new TopUpService()
                .GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, nic: nic,
                cardNumber: cardNo, pageIndex: page, pageSize: pageSize, cardTypeId: cardTypeId,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(topUpRequests);
        }

        [RoleActionValidator]
        public ActionResult Process(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var topUpRequestFormsPendingForProcessing = new TopUpService()
                .GetTopUpRequestFormsPendingForProcessing(out totalCount, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(topUpRequestFormsPendingForProcessing);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult Process(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;

            string nic = null;
            string cardNo = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int? cardTypeId = null;
            int totalCount = 0;
            int page = 0;
            string showFailedPayments = string.Empty;
            bool isShowFailedPayments = true;

            int pageSize = new SettingService().ResultPageSize;

            if (!string.IsNullOrEmpty(formCollection["ShowFailedPayments"]))
            {
                showFailedPayments = Request.Form["ShowFailedPayments"];
                if (showFailedPayments.Contains("1"))
                    isShowFailedPayments = true;
                else
                    isShowFailedPayments = false;
            }
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(formCollection["dllCardType"]))
                cardTypeId = int.Parse(formCollection["dllCardType"]);

            if (!string.IsNullOrEmpty(formCollection["txtCardNo"]))
                cardNo = formCollection["txtCardNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            if (!string.IsNullOrEmpty(formCollection["exportToCsvPayment"]))
            {
                if (createdDateFrom == null || createdDateTo == null)
                {
                    ViewBag.Message = "Date From and To are required";
                    return View();
                }

                var topRequestForms = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount
                    , pageSize: pageSize, pageIndex: page, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

                if (topRequestForms != null && topRequestForms.Count > 0)
                {
                    string fileName = "TopUpPendingforProcessing_" + createdDateFrom.Value.ToString("yyyyMMdd") + "_"
                        + createdDateTo.Value.ToString("yyyyMMdd");
                    TopUpRequestsPendingForProcessing_ExportToCSV(topRequestForms, fileName);
                    ViewBag.Message = "";
                    return null;
                }
                else
                {
                    nic = null;
                    cardNo = null;
                    createdDateFrom = null;
                    createdDateTo = null;
                    cardTypeId = null;
                    ViewBag.Message = "No record found for Export";
                    return View();
                }
            }

            var topUpRequests = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, nic: nic
                , cardNumber: cardNo, cardTypeId: cardTypeId, createdDateFrom: createdDateFrom
                , createdDateTo: createdDateTo, showFailed: isShowFailedPayments, pageIndex: page, pageSize: pageSize);

            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(topUpRequests);
        }

        private void TopUpRequestsPendingForProcessing_ExportToCSV(List<TopUpRequestForm> topUpRequestForms
            , string fileName)
        {
            var csvResult = new System.Text.StringBuilder();

            csvResult.Append("Card" + ";" + "Amount" + ";" + "Currency");
            csvResult.AppendLine();

            foreach (var topupRequestForm in topUpRequestForms)
            {
                foreach (var topUpRequest in topupRequestForm.TopUpRequests)
                {
                    string cardNumber = topUpRequest.Card.Number;
                    string lastChr = cardNumber.Substring(cardNumber.Length > 4 ? cardNumber.Length - 4
                        : cardNumber.Length);

                    csvResult.Append(lastChr + ";" + (int)(topUpRequest.Amount * 100) + ";" + "EUR;");
                    csvResult.AppendLine();
                }
            }

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "text/csv";
            Response.AddHeader("content-disposition", String.Format("attachment; filename={0}.csv", fileName));
            Response.Write(csvResult.ToString());
            Response.Flush();
            Response.End();
        }

        [RoleActionValidator]
        public JsonResult SetTopUpRequestUploadedOrFailed(string key, string failureReason)
        {
            var statusArray = new string[3];
            if (!string.IsNullOrEmpty(key))
            {
                try
                {
                    if (string.IsNullOrEmpty(failureReason))
                    {
                        // Confirmed
                        var isUpdated = new TopUpService().SetTopUpRequestUploadedOrFailed(Convert.ToInt32(key), null);
                        if (isUpdated)
                        {
                            statusArray[0] = "200";
                            statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Records updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/TopUp/Process'>Go back</a></span>";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to update.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    else if (!string.IsNullOrEmpty(failureReason) && !string.IsNullOrEmpty(key))
                    {
                        var isUpdated = new TopUpService().SetTopUpRequestUploadedOrFailed(Convert.ToInt32(key), failureReason);
                        if (isUpdated)
                        {
                            statusArray[0] = "200";
                            statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Records updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/TopUp/Process'>Go back</a></span>";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to update.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Missing required values.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ReSetTopUpRequestUploadedOrFailed(string key, string failureReason)
        {
            var statusArray = new string[3];
            if (!string.IsNullOrEmpty(key))
            {
                try
                {
                    if (string.IsNullOrEmpty(failureReason))
                    {
                        // Confirmed
                        var isUpdated = new TopUpService().SetTopUpRequestUploadedOrFailed(Convert.ToInt32(key), null);
                        if (isUpdated)
                        {
                            statusArray[0] = "200";
                            statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Records updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/TopUp/Process'>Go back</a></span>";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to update.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    else if (!string.IsNullOrEmpty(failureReason) && !string.IsNullOrEmpty(key))
                    {
                        var isUpdated = new TopUpService().SetTopUpRequestUploadedOrFailed(Convert.ToInt32(key)
                                                                        , failureReason);
                        if (isUpdated)
                        {
                            statusArray[0] = "200";
                            statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Records updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/TopUp/Process'>Go back</a></span>";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to update.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Missing required values.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        #endregion        
    }
}
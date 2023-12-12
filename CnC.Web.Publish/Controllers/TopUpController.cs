using CnC.Core.TopUps;
using CnC.Core;
using CnC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Dev;
using CnC.Core.Payments;
using CnC.Core.Accounts;

namespace CnC.Web.Controllers
{
    [RoleActionValidator]
    public class TopUpController : Controller
    {
        public ActionResult TopUpView()
        {
            return View();
        }

        public ActionResult GetTopUpHistory()
        {
            var userSession = Session["CurrentSession"] as CnC.Core.Accounts.User;

            var topUpService = new TopUpService();
            ViewBag.exchangeRate = new CnC.Web.Controllers.ExchangeRateController().GetExchangeRate(98);
            return View(topUpService.GetTopUpRequestForms(customerId: userSession.Id, topUpRequestStatus: CnC.Core.TopUpRequestStatus.History));
        }

        public ActionResult TopUpHistory()
        {
            var userSession = Session["CurrentSession"] as User;
            if (userSession == null) return View();
            var topUpHistory = new TopUpService().GetTopUpRequestForms(customerId: userSession.Id
                , topUpRequestStatus: TopUpRequestStatus.History);
            return View(topUpHistory);
        }

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

            var topUpRequests = new TopUpService().GetTopUpRequestForms(customerId: userSession.Id, cardTypeId: cardTypeId
                    , cardNumber: cardNo, nic: nic, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo,
                    topUpRequestStatus: TopUpRequestStatus.History);

            return View(topUpRequests);
        }

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

            var topUpRequests = new TopUpService().GetTopUpRequestForms(customerId: userSession.Id, cardNumber: cardNo
                , createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);
        }

        public ActionResult GetTopUpRequests()
        {
            //TODO: on session expiry redirect to login page
            var userSession = Session["CurrentSession"] as User;
            var topUpService = new TopUpService();
            return View(topUpService.GetTopUpRequestForms(customerId: userSession.Id));
        }

        [AllowAnonymous]
        public ActionResult TopUpRequests()
        {
            var topUpRequests = new TopUpService().GetTopUpRequestForms();
            return View(topUpRequests);
        }

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

            var topUpRequests = new TopUpService().GetTopUpRequestForms(nic: nic, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);
        }

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

            var topUpRequests = new TopUpService().GetTopUpRequestForms(customerId: userSession.Id, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);

        }

        public List<TopUpRequestForm> GetTopupRequestForms(int customerId)
        {
            try
            {
                var topUpService = new TopUpService();
                var topUpRequestForms = topUpService.GetTopUpRequestForms(customerId: customerId);
                return topUpRequestForms;
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
            public List<MyCardsViewModel> Cards { get; set; }
        }
        /// <summary>
        /// New Top up request pop up postback handler
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>

        #region New Topup Request
        [HttpGet]
        public ActionResult NewTopupRequest()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddTopupRequest(TransactionViewModel transaction)
        {
            // Request Form 
            var requestForm = new RequestForm();
            // decimal serviceFee = new ServiceFeeService().GetServiceFee(ServiceType.TopUpUrgent).Percentage;
            requestForm.Type = RequestFormType.TopUp;
            requestForm.TypeId = (int)RequestFormType.TopUp;
            requestForm.ExchangeRate = new ExchangeRateService().GetExchangeRate(new SettingService().CustomerDefaultCurrency.Id).Rate;
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

            var topUpRequests = new TopUpService().GetTopUpRequestForms(customerId: userSession.Id, cardNumber: cardNo
                , createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);
        }

        public ActionResult CustomerTopUpHistory()
        {
            if (Session["CurrentSession"] != null)
            {
                var userSession = Session["CurrentSession"] as User;
                var topUpHistory = new TopUpService().GetTopUpRequestForms(customerId: userSession.Id, topUpRequestStatus: TopUpRequestStatus.History);
                return View(topUpHistory);
            }
            return View();
        }
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

            var topUpRequests = new TopUpService().GetTopUpRequestForms(customerId: userSession.Id, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo
                , topUpRequestStatus: TopUpRequestStatus.History);

            return View(topUpRequests);
        }

        [HttpPost]
        public JsonResult CustomerNewTopupRequest(TransactionViewModel transaction)
        {
            //Basic Validation on transation object
            var statusArray = new string[3];
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
                //foreach (var card in transaction.Cards)
                //{
                //    if (string.IsNullOrEmpty(card.cardAmount))
                //    {
                //        statusArray[0] = "101";
                //        statusArray[1] = "Invalid Amount.";
                //        return Json(statusArray, JsonRequestBehavior.AllowGet);
                //    }
                //    if (string.IsNullOrEmpty(card.cardId) || string.IsNullOrEmpty(card.cardNo))
                //    {
                //        statusArray[0] = "101";
                //        statusArray[1] = "Invalid request.";
                //        return Json(statusArray, JsonRequestBehavior.AllowGet);
                //    }
                //}
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
                    var tempCards = new CardService().GetCardsReloadable(userSession.Id);

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
                var tempToVerifyAmt = Convert.ToInt32(transaction.Amount);

                if (tempToVerifyAmt < tempActualAmt)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid amount entered in Amount Textbox.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }


                // Payment
                var payments = new List<Payment>
                {
                    new Payment
                    {
                        TransactionAccountNo = transaction.AccountNo,
                        TransactionName = transaction.Name,
                        TransactionNo = transaction.TransactionNo,
                        Amount = tempActualAmt,
                        PaymentMethod = PaymentMethod.Bank,
                        PaymentMethodId = (int) PaymentMethod.Bank
                    }
                };

                int requestFormId = new TopUpService().AddTopUpRequestForm(requestForm, topupRequestList, payments);
                if (requestFormId > 0)
                {
                    statusArray[0] = "200";
                    statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/TopUp/CustomerNewTopUpRequest'>Go back</a></span>";
                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Unable to process the request";
                }
            }
            catch (Exception exp)
            {
                statusArray[0] = "101";
                statusArray[1] = exp.Message;
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CustomerNewTopUpRequest()
        {
            return View();
        }


        #endregion

        #region TBO Agent

        public ActionResult TopUpRequestsSearch()
        {
            var TopUpRequests = new TopUpService().GetTopUpRequestForms();
            return View(TopUpRequests);
        }

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

            var topUpRequests = new TopUpService().GetTopUpRequestForms(customerId: userSession.Id, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);
        }

        public ActionResult Validation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Validation(FormCollection fc)
        {
            return View();
        }

        public ActionResult CustomerTopUpRequestsProcessing()
        {
            var userSession = Session["CurrentSession"] as User;
            if (userSession == null) return View();
            var topUpRequests = new TopUpService().GetTopUpRequestFormsUnderProcessing(customerId: userSession.Id, isForCustomer: true);
            return View(topUpRequests);
        }

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

            var topUpRequests = new TopUpService().GetTopUpRequestFormsUnderProcessing(customerId: userSession.Id, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);
        }

        public ActionResult PaymentConfirmation()
        {
            var topupService = new TopUpService();
            var topUpRequestForms = topupService.GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed();
            return View(topUpRequestForms);
        }

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

            var topUpRequests = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(nic: nic, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);
        }


        public ActionResult Process()
        {
            var topupService = new TopUpService();
            var topUpRequestForms = topupService.GetTopUpRequestFormsPendingForProcessing();
            return View(topUpRequestForms);
        }

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

            var topUpRequests = new TopUpService().GetTopUpRequestFormsPendingForProcessing(nic: nic, cardNumber: cardNo
                , cardTypeId: cardTypeId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            return View(topUpRequests);
        }

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
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = exp.Message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
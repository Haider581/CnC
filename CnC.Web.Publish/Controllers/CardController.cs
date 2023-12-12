using CnC.Core.Cards;
using CnC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Dev;
using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Payments;

namespace CnC.Web.Controllers
{
    [RoleActionValidator]
    public class CardController : Controller
    {
        #region Card        
        
        public ActionResult GetCards()
        {
            try
            {
                var userSession = Session["CurrentSession"] as User;
                return View(new CardService().GetCards(customerId: userSession.Id));
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult Cards()
        {
            var cards = new CardService().GetCards();
            return View(cards);
        }

        [HttpPost]
        public ActionResult Cards(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            string cardNo = null;
            int? cardTypeId = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

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

            return View(new CardService().GetCards(nic: nic, passportNo: passport, cardNumber: cardNo, cardTypeId: cardTypeId,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        public string GetCardRequestStatusString(CardRequest cardRequest, bool isCustomer)
        {
            return new CardService().GetCardRequestStatusString(cardRequest, isCustomer);
        }
        /// <summary>
        /// Get Customer Cards
        /// </summary>        
        public ActionResult CustomerCards()
        {
            var userSession = Session["CurrentSession"] as User;
            List<Card> cards = new CardService().GetCards(customerId: userSession.Id);
            return View(cards);
        }

        [HttpGet]
        public JsonResult CardBalanceAndTransactions(string cardNumber)
        {
            var statusArray = new object[3];
            try
            {
                Card cardInfo = new CardService().GetCardBalanceAndPaymentTransactions(cardNumber);
                if (cardInfo != null)
                {   
                    statusArray[0] = "200";
                    statusArray[1] = cardInfo;

                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Unable to process.";
                }                
            }
            catch (Exception exp)
            {

                statusArray[0] = "101";
                statusArray[1] = exp.Message;
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        #endregion                

        #region Card Request
        public ActionResult CardRequests()
        {
            var cardRequests = new CardService().GetCardRequestForms();
            return View(cardRequests);
        }

        [HttpPost]
        public ActionResult CardRequests(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            RequestFormStatus? requestFormStatus = null;
            
            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

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

            if (!string.IsNullOrEmpty(formCollection["RequestFormStatus"]))
                requestFormStatus = (RequestFormStatus)(int.Parse(formCollection["RequestFormStatus"]));

            return View(new CardService().GetCardRequestForms(nic: nic, passportNo: passport,
                requestFormStatus: requestFormStatus, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        public ActionResult CustomerCardRequests(int? customerId = null)
        {
            try
            {
                var userSession = Session["CurrentSession"] as CnC.Core.Accounts.User;
                var cardRequestForms = new CardService().GetCardRequestForms(customerId: userSession.Id
                    , isForCustomer: true);
                return View(cardRequestForms);
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        /// <summary>
        /// For Customer
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustomerCardRequests(FormCollection formCollection)
        {
            var userSession = Session["CurrentSession"] as User;
                        
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            
            if (!string.IsNullOrEmpty(formCollection["DateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["DateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["DateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["DateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new CardService().GetCardRequestForms(customerId: userSession.Id, createdDateFrom: createdDateFrom
                , createdDateTo: createdDateTo, isForCustomer: true));
        }


        public JsonResult UpdateCardStatus(string key, string cardNumber, string cardTitle, string cardExpiry, string cardId, string customerClientId)
        {
            var statusArray = new string[3];
            try
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(cardNumber) && !string.IsNullOrEmpty(cardTitle) && !string.IsNullOrEmpty(cardExpiry) && !string.IsNullOrEmpty(cardId) && !string.IsNullOrEmpty(customerClientId))
                {
                    cardExpiry = DateTime.Parse(cardExpiry).ToString("yyMM");
                    var cardRequestIds = key.Split('-');
                    if (new CardService().IsCardExist(cardNumber, cardId))
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "CardId or Card Number already exists.";
                    }
                    var res = new CardService().SetCardRequestApproved(Convert.ToInt32(cardRequestIds[0]), Convert.ToInt32(cardRequestIds[1]), cardNumber, cardTitle, cardExpiry, cardId);
                    if (res > 0)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Card/CardIssuerResponseForCardRequests'>Go back</a></span>";
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to process the request";
                    }

                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Missing required values.";
                }
            }
            catch (Exception exp)
            {
                statusArray[0] = "101";
                statusArray[1] = "Unable to update data please try again.";
            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateCardStatusFailed(string key, string failureReason)
        {
            var statusArray = new string[3];
            try
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(failureReason))
                {
                    var cardRequestIds = key.Split('-');
                    var res = new CardService().SetCardRequestDeclined(Convert.ToInt32(cardRequestIds[0]), Convert.ToInt32(cardRequestIds[1]), failureReason);
                    if (res > 0)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Card/CardIssuerResponseForCardRequests'>Go back</a></span>";
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to process the request";
                    }


                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Missing required values.";
                }
            }
            catch (Exception exp)
            {
                statusArray[0] = "101";
                statusArray[1] = "Unable to update data please try again.";
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);


        }

        public class MyCardsViewModel
        {
            public string cardTypeId { get; set; }
            public string cardName { get; set; }
            public string cardAmount { get; set; }
            public string cardQuantity { get; set; }
        }

        public class TransactionViewModel
        {
            public int customerId { get; set; }
            public string transactionNo { get; set; }
            public string accountNo { get; set; }
            public string name { get; set; }
            public decimal amount { get; set; }
            public List<MyCardsViewModel> Cards { get; set; }
        }

        [HttpGet]
        public ActionResult NewCardRequest()
        {
            return View();
        }
        public JsonResult NewCardRequest(TransactionViewModel transaction)
        {
            // Request Form 
            var requestForm = new RequestForm();
            //decimal serviceFeePercent = new ServiceFeeService().GetServiceFee(ServiceType.Card).Percentage;


            requestForm.Type = RequestFormType.Card;
            requestForm.TypeId = (int)RequestFormType.Card;
            requestForm.ExchangeRate = new ExchangeRateService().GetExchangeRate(new CurrencyService().SystemCurrency.Id).Rate;
            requestForm.CustomerId = transaction.customerId;

            // Card request
            List<CardRequest> cardRequests = new List<CardRequest>();

            bool cardPurchased = false;
            decimal totalAmount = 0;

            foreach (var cardItem in transaction.Cards)
            {
                int cardQty = int.Parse(cardItem.cardQuantity);
                int cardTypeId = int.Parse(cardItem.cardTypeId);

                if (cardQty > 0)
                {
                    cardPurchased = true;

                    CardRequest cardRequest = new CardRequest();
                    cardRequest.CardQty = cardQty;
                    cardRequest.CardTypeId = cardTypeId;
                    cardRequest.Fee = (new CardService().GetCardType(cardTypeId).Fee) * cardQty;

                    cardRequests.Add(cardRequest);

                    totalAmount = totalAmount + cardRequest.Fee;
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
                TransactionAccountNo = transaction.accountNo,
                TransactionName = transaction.name,
                TransactionNo = transaction.transactionNo,
                Amount = totalAmount,
                CreatedOn = DateTime.UtcNow,
                PaymentMethod = PaymentMethod.Bank,
                PaymentMethodId = (int)PaymentMethod.Bank
            });

            new CardService().AddCardRequestForm(requestForm, cardRequests, payments, false);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Customer Card Requests
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        private List<CardRequest> GetCardRequest(int customerId)
        {
            try
            {
                var cardService = new CardService();
                List<CardRequest> cards = cardService.GetCardRequests(customerId, null, null, null, null);
                return cards;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public List<CardRequestForm> GetCardRequestForms(int customerId)
        {
            try
            {
                var cardService = new CardService();
                var cardRequestForms = cardService.GetCardRequestForms(customerId: customerId);
                return cardRequestForms;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        #endregion Card Request

        #region Pending Card requests

        public ActionResult PendingCardRequests()
        {
            var cardRequests = new CardService().GetPendingCardRequestForms();
            return View(cardRequests);
        }

        [HttpPost]
        public ActionResult PendingCardRequests(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;            

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

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

            return View(new CardService().GetPendingCardRequestForms(nic: nic, passportNo: passport
                , createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        #endregion

        #region CardIssuerResponseForCardRequests

        public ActionResult CardIssuerResponseForCardRequests()
        {

            var cardRequests = new CardService().GetCardRequestFormsWaitingForCardIssuerResponseOrDeclined();
            return View(cardRequests);
        }
        [HttpPost]
        public ActionResult CardIssuerResponseForCardRequests(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

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

            return View(new CardService().GetCardRequestFormsWaitingForCardIssuerResponseOrDeclined(nic: nic, passportNo: passport,
             createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        #endregion
        
        #region CardsDeliverToCustomer

        public ActionResult CardsDeliverToCustomer()
        {
            var cardRequests = new CardService().GetCardRequestsPendingToDeliverToCustomer();
            return View(cardRequests);
        }

        [HttpPost]
        public ActionResult CardsDeliverToCustomer(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

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

            return View(new CardService().GetCardRequestsPendingToDeliverToCustomer(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        public JsonResult SetCardStatusDeliveredToCustomer(string key)
        {
            var statusArray = new string[3];
            var cardService = new CardService();
            if (!string.IsNullOrEmpty(key))
            {
                //check isExist
                //Completed
                try
                {
                    int cardRequestId = Convert.ToInt32(key);
                    var res = cardService.SetCardRequestDeliveredToCustomer(cardRequestId);
                    if (res)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Card/CardsDeliverToCustomer'>Go back</a></span>";
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to process the request";
                    }
                }
                catch (Exception ex)
                {
                    statusArray[0] = "101";
                    statusArray[1] = ex.Message;
                }

            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Unable to process the request";
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Card Type

        public List<CardType> GetCardTypes()
        {
            try
            {
                var cardService = new CardService();
                List<CardType> cardTypes = cardService.GetCardTypes();
                return cardTypes;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Will use for Dashboard
        /// </summary>
        /// <param name="cardRequestStatus"></param>
        /// <returns></returns>
        public int GetCardRequestsCountByStatus(CardRequestStatus cardRequestStatus)
        {
            var cardService = new CardService();
            return cardService.GetCardRequestsCountByStatus(cardRequestStatus);
        }

        /// <summary>
        /// Will use for Dashboard
        /// </summary>
        /// <param name="requestFormStatus"></param>
        /// <returns></returns>
        public int GetCardRequestFormsCountByStatus(RequestFormStatus requestFormStatus)
        {
            var cardService = new CardService();
            return cardService.GetCardRequestFormsCountByStatus(requestFormStatus);
        }
    }
}
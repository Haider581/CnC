using CnC.Core.Cards;
using CnC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Payments;
using CnC.Core.Exceptions;
using CnC.Web.Helper;
using CnC.Core.TopUps;
using log4net;
using ClosedXML.Excel;
using System.IO;

namespace CnC.Web.Controllers
{
    [RoleActionValidator]
    [Authorize]
    public class CardController : Controller
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardController));

        #region Card

        [RoleActionValidator]
        public ActionResult GetCards()
        {
            int totalCount = 0;
            try
            {
                var userSession = Session["CurrentSession"] as User;
                return View(new CardService().GetCards(out totalCount, customerId: userSession.Id));
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        [RoleActionValidator]
        public ActionResult Cards(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().ResultPageSize;
                var cards = new CardService().GetCards(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cards);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        [HttpPost]
        [RoleActionValidator]
        public ActionResult Cards(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());
            try
            {
                string nic = null;
                string passport = null;
                string email = null;
                string cardNo = null;
                int? cardTypeId = null;
                DateTime? createdDateFrom = null;
                DateTime? createdDateTo = null;
                int totalCount = 0;
                int page = 0;
                if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                {
                    page = Convert.ToInt32(Request.Form["btnPagination"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                    nic = Request.Form["txtNICNo"];

                if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                    passport = Request.Form["txtPassportNo"];

                if (!string.IsNullOrEmpty(Request.Form["txtEmail"]))
                    email = Request.Form["txtEmail"];

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

                if (!string.IsNullOrEmpty(formCollection["exportPaymentPendingForConfirmation"]))
                {
                    if (!string.IsNullOrEmpty(formCollection["txtHiddenCardNumber"]))
                    {
                        string cardNumber = Request.Form["txtHiddenCardNumber"];
                        Card cardInfo = new CardService().GetCardBalanceAndPaymentTransactions(cardNumber
                            , dateFrom: DateTime.Now.AddYears(-2), dateTo: DateTime.Now);

                        //Card cardInfo = new Card();
                        //cardInfo.Balance = 200;
                        //cardInfo.PaymentTransactions = new List<CardPaymentTransaction>();

                        //for (int i = 0; i < 21; i++)
                        //{
                        //    cardInfo.PaymentTransactions.Add(new CardPaymentTransaction()
                        //    {
                        //        Amount = Convert.ToDecimal(200) / 100,
                        //        AccountServiceFee = Convert.ToDecimal(300) / 100,
                        //        CreatedOn = DateTime.Now,
                        //        Description = "Demo",
                        //        TransactionTypeDescription = "transaction",
                        //        IsApproved = true,
                        //        IsDebit = true,
                        //        Status = "Active",
                        //        AccountCurrencyAmount = Convert.ToDecimal(500) / 100
                        //    });
                        //}

                        if (cardInfo != null && cardInfo.PaymentTransactions.Count > 0)
                        {
                            string fileName = "BalanceAndTransactions_" +
                                DateTime.Now.AddYears(-2).ToString("yyyy-MM-dd") + "_"
                                + DateTime.Now.ToString("yyyy-MM-dd");
                            BalanceAndTransaction_ExportToExcel(cardInfo, fileName);
                            ViewBag.Message = "";
                            return null;
                        }
                        ViewBag.MessageFailure = "No Record Found";
                    }
                    else
                    {

                        return View();
                    }
                }

                int pageSize = new SettingService().ResultPageSize;
                var cards = new CardService().GetCards(out totalCount, nic: nic, passportNo: passport
                    , cardNumber: cardNo, email: email, cardTypeId: cardTypeId, createdDateFrom: createdDateFrom
                    , createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cards);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        public void BalanceAndTransaction_ExportToExcel(Card card, string fileName)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(new Utility().GetBalaceAndTransactionTable(card));
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= " + fileName + ".xlsx");

                using (MemoryStream myMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(myMemoryStream);
                    myMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        public string GetCardRequestStatusString(CardRequest cardRequest, bool isCustomer)
        {
            return new CardService().GetCardRequestStatusString(cardRequest, isCustomer);
        }
        /// <summary>
        /// Get Customer Cards
        /// </summary>  
        [RoleActionValidator]
        public ActionResult CustomerCards(int page = 0, string customerId = null)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().ResultPageSize;
                int? custId = null;
                if (customerId != null)
                    custId = Convert.ToInt32(new Utility().UrlSafeDecrypt(customerId.ToString()));
                var userSession = Session["CurrentSession"] as User;
                var cards = new CardService().GetCards(out totalCount, customerId: userSession.IsCustomer ?
                                                       userSession.Id : custId);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cards);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        [HttpGet]
        [RoleActionValidator]
        public JsonResult CardBalanceAndTransactions(string cardNumber, string dateFrom, string dateTo)
        {
            var statusArray = new object[3];
            try
            {
                if (!string.IsNullOrEmpty(cardNumber))
                {
                    //By default it will set one month filter
                    DateTime transactionDateFrom = DateTime.Now.AddMonths(-1);
                    DateTime transactionDateTo = DateTime.Now;
                    //else it will set user defined dates if not null
                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        transactionDateFrom = Convert.ToDateTime(dateFrom);
                        transactionDateTo = Convert.ToDateTime(dateTo);
                    }

                    int res = DateTime.Compare(transactionDateFrom, transactionDateTo);
                    if (res < 0)
                    {
                        Card cardInfo = new CardService().GetCardBalanceAndPaymentTransactions(cardNumber
                            , transactionDateFrom, transactionDateTo);

                        //Card cardInfo = new Card();
                        //cardInfo.Balance = 200;
                        //cardInfo.PaymentTransactions = new List<CardPaymentTransaction>();

                        //for (int i = 0; i < 21; i++)
                        //{
                        //    cardInfo.PaymentTransactions.Add(new CardPaymentTransaction()
                        //    {
                        //        Amount = Convert.ToDecimal(200) / 100,
                        //        AccountServiceFee = Convert.ToDecimal(300) / 100,
                        //        CreatedOn = DateTime.Now,
                        //        Description = "Demo",
                        //        TransactionTypeDescription = "transaction",
                        //        IsApproved = true,
                        //        IsDebit = true,
                        //        Status = "Active",
                        //        AccountCurrencyAmount = Convert.ToDecimal(500) / 100
                        //    });
                        //}

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
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "From date can not greater than To date.";
                    }
                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Unable to process.";
                }
            }
            catch (UserException exp)
            {
                statusArray[0] = "101";
                statusArray[1] = "Failed: " + exp.Message;
            }
            catch (Exception exp)
            {
                log.Error(exp.Message);
                statusArray[0] = "101";
                statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        #endregion
        [RoleActionValidator]
        public ActionResult PaymentConfirmation(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().ResultPageSize;
                var cardRequestPendingForPaymentCinfirmation = new CardService()
                    .GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, pageIndex: page
                    , pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cardRequestPendingForPaymentCinfirmation);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        [HttpPost]
        [RoleActionValidator]
        public ActionResult PaymentConfirmation(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            var userSession = Session["CurrentSession"] as User;

            try
            {
                string nic = null;
                string passportNo = null;
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

                if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                    passportNo = Request.Form["txtPassportNo"];

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
                var cardRequests = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, nic: nic, passportNo: passportNo,
                   createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cardRequests);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        #region Card Request
        [RoleActionValidator]
        public ActionResult CardRequests(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().ResultPageSize;
                var cardRequestsForms = new CardService().GetCardRequestForms(out totalCount, pageIndex: page
                    , pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cardRequestsForms);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        [HttpPost]
        [RoleActionValidator]
        public ActionResult CardRequests(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());
            try
            {
                string nic = null;
                string passport = null;
                DateTime? createdDateFrom = null;
                DateTime? createdDateTo = null;
                RequestFormStatus? requestFormStatus = null;
                int totalCount = 0;
                int page = 0;
                if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                {
                    page = Convert.ToInt32(Request.Form["btnPagination"]);
                }
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
                int pageSize = new SettingService().ResultPageSize;
                var cardRequestForms = new CardService().GetCardRequestForms(out totalCount, nic: nic, passportNo: passport,
                    requestFormStatus: requestFormStatus, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cardRequestForms);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        [RoleActionValidator]
        public ActionResult CustomerCardRequests(int? customerId = null, int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var userSession = Session["CurrentSession"] as CnC.Core.Accounts.User;
            var cardRequestsForms = new CardService().GetCardRequestForms(out totalCount, customerId: userSession.Id
                    , isForCustomer: true, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            try
            {
                return View(cardRequestsForms);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return null;
            }
        }

        /// <summary>
        /// For Customer
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [RoleActionValidator]
        public ActionResult CustomerCardRequests(FormCollection formCollection)
        {
            var userSession = Session["CurrentSession"] as User;
            try
            {
                DateTime? createdDateFrom = null;
                DateTime? createdDateTo = null;
                int totalCount = 0;
                int page = 0;
                if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                {
                    page = Convert.ToInt32(Request.Form["btnPagination"]);
                }
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
                int pageSize = new SettingService().ResultPageSize;
                var cardRequestsForms = new CardService().GetCardRequestForms(out totalCount, customerId: userSession.Id,
                                    createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, isForCustomer: true,
                                    pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cardRequestsForms);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        [RoleActionValidator]
        public JsonResult UpdateCardStatus(string key, string cardNumber, string cardTitle, string cardExpiry
            , string cardId, string customerClientId)
        {
            var statusArray = new string[3];
            try
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(cardNumber) && (cardId.Length >= 4 && cardId.Length <= 8)
                    && (cardNumber.Length >= 7 && cardNumber.Length <= 16) && (cardTitle.Length >= 4 && cardTitle.Length <= 25)
                    && !string.IsNullOrEmpty(cardTitle) && !string.IsNullOrEmpty(cardExpiry) && !string.IsNullOrEmpty(cardId)
                    && !string.IsNullOrEmpty(customerClientId))
                {
                    cardExpiry = DateTime.Parse(cardExpiry).ToString("yyMM");
                    var cardRequestIds = key.Split('-');

                    if (new CardService().IsCardExist(cardNumber, cardId))
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "CardId or Card Number already exists.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }

                    var res = new CardService().SetCardRequestApproved(Convert.ToInt32(cardRequestIds[0])
                        , Convert.ToInt32(cardRequestIds[1]), cardNumber, cardTitle, cardExpiry, cardId
                        , customerClientId);

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
            catch (UserException exp)
            {
                statusArray[0] = "101";
                statusArray[1] = "Failed: " + exp.Message;
            }
            catch (Exception exp)
            {
                log.Error(exp.Message);
                statusArray[0] = "101";
                statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [RoleActionValidator]
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
            catch (UserException exp)
            {
                statusArray[0] = "101";
                statusArray[1] = "Failed: " + exp.Message;
            }
            catch (Exception exp)
            {
                log.Error(exp.Message);
                statusArray[0] = "101";
                statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
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
        [RoleActionValidator]
        public ActionResult NewCardRequest()
        {
            return View();
        }
        [RoleActionValidator]
        public JsonResult NewCardRequest(TransactionViewModel transaction)
        {
            // Request Form 
            var requestForm = new RequestForm();
            //decimal serviceFeePercent = new ServiceFeeService().GetServiceFee(ServiceType.Card).Percentage;

            try
            {
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
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Customer Card Requests
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>

        [RoleActionValidator]
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

        [RoleActionValidator]
        public List<CardRequestForm> GetCardRequestForms(int customerId)
        {
            try
            {
                var cardService = new CardService();
                int totalCount = 0;
                var cardRequestForms = cardService.GetCardRequestForms(out totalCount, customerId: customerId);
                return cardRequestForms;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        #endregion Card Request

        #region Pending Card requests
        [RoleActionValidator]
        public ActionResult PendingCardRequests(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var getPendingCardRequests = new CardService().GetPendingCardRequestForms(out totalCount, pageIndex: page
                , pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(getPendingCardRequests);
        }

        [HttpPost]
        [RoleActionValidator]
        public ActionResult PendingCardRequests(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
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
            int pageSize = new SettingService().ResultPageSize;
            var pendingCardRequests = new CardService().GetPendingCardRequestForms(out totalCount, nic: nic, passportNo: passport
                , createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(pendingCardRequests);
        }

        #endregion

        #region CardIssuerResponseForCardRequests
        [RoleActionValidator]
        public ActionResult CardIssuerResponseForCardRequests(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var cardRequest = new CardService().
                GetCardRequestFormsWaitingForCardIssuerResponseOrDeclined(out totalCount, pageIndex: page,
                pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(cardRequest);
        }

        [HttpPost]
        [RoleActionValidator]
        public ActionResult CardIssuerResponseForCardRequests(FormCollection formCollection)
        {
            int totalCount = 0;
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int page = 0;
            string showFailedPayments = string.Empty;
            bool isShowFailedPayments = true;

            if (!string.IsNullOrEmpty(formCollection["ShowFailedPayments"]))
            {
                showFailedPayments = Request.Form["ShowFailedPayments"];
                if (showFailedPayments.Contains("1"))
                    isShowFailedPayments = true;
                else
                    isShowFailedPayments = false;
            }

            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
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
            int pageSize = new SettingService().ResultPageSize;
            var cardRequestFormsWaitingForCardIssuerResponseOrDeclined = new CardService()
                .GetCardRequestFormsWaitingForCardIssuerResponseOrDeclined(out totalCount, nic: nic, passportNo: passport,
             createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize
             , showFailed: isShowFailedPayments);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(cardRequestFormsWaitingForCardIssuerResponseOrDeclined);
        }
        public JsonResult CustomerDocuments(string nic)
        {
            string[] statusArray = new string[9];

            if (!string.IsNullOrEmpty(nic))
            {
                try
                {
                    var result = new CustomerService().GetCustomerWithNIC(nic: nic);
                    if (result == null)
                    {
                        statusArray[0] = "401";
                        statusArray[1] = "Customer not found";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }

                    if (!string.IsNullOrEmpty(result.ProofOfAddressDocInCustomerLanguage)
                        || !string.IsNullOrEmpty(result.ProofOfSourceOfFundsDocInCustomerLanguage))
                    {
                        string[] addressDocImg = new string[4];
                        string[] sourceOfFundsImg = new string[4];

                        if (result.ProofOfAddressDocInCustomerLanguage.Contains(";"))
                        {
                            addressDocImg = result.ProofOfAddressDocInCustomerLanguage.Split(';');

                            for (int i = 0; i < addressDocImg.Count(); i++)
                            {
                                string address = Path.Combine(new DocumentService()
                                           .GetDocumentDirectoryPath(result.NIC)
                                           , addressDocImg[i]);

                                addressDocImg[i] = new Utility().ConvertImgToBase64(address);
                            }
                        }
                        else
                        {
                            string address = Path.Combine(new DocumentService()
                                           .GetDocumentDirectoryPath(result.NIC)
                                           , result.ProofOfAddressDocInCustomerLanguage);

                            addressDocImg[0] = new Utility().ConvertImgToBase64(address);
                        }

                        if (result.ProofOfSourceOfFundsDocInCustomerLanguage.Contains(";"))
                        {

                            sourceOfFundsImg = result.ProofOfSourceOfFundsDocInCustomerLanguage.Split(';');

                            for (int i = 0; i < sourceOfFundsImg.Count(); i++)
                            {
                                string sourceFunds = Path.Combine(new DocumentService()
                                             .GetDocumentDirectoryPath(result.NIC)
                                             , sourceOfFundsImg[i]);

                                sourceOfFundsImg[i] = new Utility().ConvertImgToBase64(sourceFunds);
                            }

                        }
                        else
                        {
                            sourceOfFundsImg[0] = Path.Combine(new DocumentService()
                                           .GetDocumentDirectoryPath(result.NIC)
                                           , result.ProofOfSourceOfFundsDocInCustomerLanguage);

                            sourceOfFundsImg[0] = new Utility().ConvertImgToBase64(sourceOfFundsImg[0]);
                        }

                        if ((addressDocImg != null && addressDocImg.Count() > 0
                            && (sourceOfFundsImg != null && sourceOfFundsImg.Count() > 0)))
                        {
                            statusArray[0] = "400";
                            for (int i = 0; i < addressDocImg.Count(); i++)
                            {
                                statusArray[i + 1] = addressDocImg[i];
                            }
                            for (int i = 0; i < sourceOfFundsImg.Count(); i++)
                            {
                                statusArray[5 + i] = sourceOfFundsImg[i];
                            }
                            var jsonResult = Json(statusArray, JsonRequestBehavior.AllowGet);
                            jsonResult.MaxJsonLength = Int32.MaxValue;
                            return jsonResult;
                        }
                        else
                        {
                            statusArray[0] = "401";
                            statusArray[1] = "Customer Documents Missing";
                            var jsonResult = Json(statusArray, JsonRequestBehavior.AllowGet);
                            jsonResult.MaxJsonLength = Int32.MaxValue;
                            return jsonResult;
                        }
                    }
                    else
                    {
                        statusArray[0] = "402";
                        statusArray[1] = "Black Card does not exist for this customer";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                }

                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    statusArray[0] = "401";
                    statusArray[1] = exception.Message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                statusArray[0] = "401";
                statusArray[1] = "Nic is required";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region CardsDeliverToCustomer
        //[RoleActionValidator]
        //public ActionResult CardsDeliverToCustomer(int page = 0)
        //{
        //    try
        //    {
        //        int totalCount = 0;
        //        int pageSize = new SettingService().ResultPageSize;
        //        new CardService().GetCardRequestsPendingToDeliverToCustomer(out totalCount);
        //        this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
        //        this.ViewBag.Page = page;
        //        return View(new CardService().GetCardRequestsPendingToDeliverToCustomer(out totalCount, pageIndex: page, pageSize: pageSize));
        //    }
        //    catch (Exception exception)
        //    {
        //        log.Error(exception.Message);
        //        return View();
        //    }
        //}

        //[HttpPost]
        //[RoleActionValidator]
        //public ActionResult CardsDeliverToCustomer(FormCollection formCollection)
        //{
        //    if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
        //        return Redirect(Request.UrlReferrer.ToString());
        //    try
        //    {
        //        string nic = null;
        //        string passport = null;
        //        DateTime? createdDateFrom = null;
        //        DateTime? createdDateTo = null;
        //        int totalCount = 0;
        //        int page = 0;
        //        if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
        //        {
        //            page = Convert.ToInt32(Request.Form["btnPagination"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
        //            nic = Request.Form["txtNICNo"];

        //        if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
        //            passport = Request.Form["txtPassportNo"];

        //        if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
        //        {
        //            DateTime txtCreatedDateFrom;
        //            if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
        //            {
        //                return View();
        //            }

        //            createdDateFrom = txtCreatedDateFrom;
        //        }

        //        if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
        //        {
        //            DateTime txtCreatedDateTo;
        //            if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
        //            {
        //                return View();
        //            }

        //            createdDateTo = txtCreatedDateTo;
        //        }
        //        int pageSize = new SettingService().ResultPageSize;
        //        var cardRequestsPendingToDeliverToCustomer = new CardService().GetCardRequestsPendingToDeliverToCustomer(out totalCount,
        //            nic: nic, passportNo: passport, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page
        //            , pageSize: pageSize);
        //        this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
        //        this.ViewBag.Page = page;
        //        return View(cardRequestsPendingToDeliverToCustomer);
        //    }
        //    catch (Exception exception)
        //    {
        //        log.Error(exception.Message);
        //        return View();
        //    }
        //}
        //[RoleActionValidator]
        //public JsonResult SetCardStatusDeliveredToCustomer(string key)
        //{
        //    var statusArray = new string[3];
        //    var cardService = new CardService();
        //    if (!string.IsNullOrEmpty(key))
        //    {
        //        //check isExist
        //        //Completed
        //        try
        //        {
        //            int cardRequestId = Convert.ToInt32(key);
        //            var res = cardService.SetCardRequestDeliveredToCustomer(cardRequestId);
        //            if (res)
        //            {
        //                statusArray[0] = "200";
        //                statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Card/CardsDeliverToCustomer'>Go back</a></span>";
        //            }
        //            else
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "Unable to process the request";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error(ex.Message);
        //            statusArray[0] = "101";
        //            statusArray[1] = ex.Message;
        //        }

        //    }
        //    else
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Unable to process the request";
        //    }
        //    return Json(statusArray, JsonRequestBehavior.AllowGet);

        //}
        public ActionResult EditCardRequest(string customerId)
        {
            try
            {
                if (!string.IsNullOrEmpty(customerId))
                {
                    int totalCount = 0;
                    return View(new CardService().GetCardRequestForms(out totalCount,
                                customerId: Convert.ToInt16(new Utility().UrlSafeDecrypt(customerId))));
                }

                return View();
            }

            catch (Exception exception)
            {
                log.Error(exception.Message);
                return View();
            }
        }

        public JsonResult EditCardDetails(string email, string cardTypeId)
        {
            int totalCount = 0;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(cardTypeId))
            {
                return Json("Record not found! Either Customer not Exists or card not found"
                    , JsonRequestBehavior.AllowGet);
            }

            try
            {
                var result = new CardService().GetCards(out totalCount, email: email
                    , cardTypeId: Convert.ToInt32(cardTypeId));

                var card = new Card();
                card.Balance = result[0].Balance;
                card.CardServiceProviderCardId = result[0].CardServiceProviderCardId;
                card.CardServiceProviderClientId = result[0].CardServiceProviderClientId;
                card.ExpiryDate = result[0].ExpiryDate;
                card.Number = result[0].Number;
                card.Customer = result[0].CardRequest.CardRequestForm.Customer;
                card.Title = result[0].Title;
                card.ExpiryDate = result[0].ExpiryDate;

                if (result != null && result.Count > 0)
                    return Json(card, JsonRequestBehavior.AllowGet);


                return Json("Record Not Found", JsonRequestBehavior.AllowGet);
            }
            catch (UserException exception)
            {
                return Json(exception.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                return Json("Failed! Record not Updated! Please try again later", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateCardDetails(string email, string cardTypeId, string clientId, string cardNo
                                          , string cardId, string cardRequestId, string cardTitle, string cardExpiry
                                           , string originalCardId, string originalCardNumber)
        {
            string[] statusArray = new string[5];

            if (!cardId.Equals(originalCardId))
            {
                if (new CardService().IsCardExistWithCardServiceProviderCardId(cardId))
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Card Id already Exists";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }

            if (!cardNo.Equals(originalCardNumber))
            {
                if (new CardService().IsCardExistWithCardNumber(cardNo))
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Card Number already Exists";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }

            try
            {
                var card = new Card();
                card.CardServiceProviderClientId = clientId;
                card.CardServiceProviderCardId = cardId;
                card.Number = cardNo;
                card.CardRequestId = Convert.ToInt32(cardRequestId);
                card.Title = cardTitle;
                card.ExpiryDate = cardExpiry;

                bool isSaved = new CardService().UpdateCustomerCardDetails(card);

                if (isSaved)
                {
                    statusArray[0] = "200";
                    statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Card/Cards'>Go back</a></span>";
                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Unable to process the request";
                }
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            catch (UserException userException)
            {
                statusArray[0] = "101";
                statusArray[1] = userException.Message;
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                statusArray[0] = "101";
                statusArray[1] = exception.Message;
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditCardRequest(FormCollection formCollection)
        {
            var statusArray = new string[3];
            var arrCardTypeIds = Request.Form["ddlCardType"].Split(',');
            string customerId = Request.Form["txtCustomerId"];
            decimal totalAmount = 0;
            int totalCount = 0;
            bool isBlackCardExists = false;
            int? blackCardId = null;
            List<TopUpRequestForm> topupRequestsForms = null;
            try
            {
                var customerCards = new CardService().GetCardRequestForms(out totalCount
                    , customerId: Convert.ToInt32(customerId));

                for (int i = 0; i < arrCardTypeIds.Count(); i++)
                {
                    arrCardTypeIds[i] = arrCardTypeIds[i].Substring(0, arrCardTypeIds[i].IndexOf("-") + 1)
                                                         .Replace("-", "");
                }

                if (string.IsNullOrEmpty(customerId))
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Record not updated! Customer not Exists";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }

                string message = "Sorry! Cannot update the records because, Toup request exist for following Card/s: <br />";

                for (int i = 0; i < customerCards[0].CardRequests.Count(); i++)
                {
                    if (Convert.ToInt32(arrCardTypeIds[i]) != customerCards[0].CardRequests[i].CardType.Id)
                    {
                        // A check to know if the card is delivered or not
                        if (customerCards[0].CardRequests[i].Card != null)
                        {
                            // if the card is delivered, check if the topup request is made or not
                            topupRequestsForms = new TopUpService().GetTopUpRequestForms(out totalCount
                                , cardNumber: customerCards[0].CardRequests[0].Card.Number);
                            if (topupRequestsForms != null && topupRequestsForms.Count() > 0)
                                message += customerCards[0].CardRequests[i].Card.Number.ToString() + ", ";
                        }
                    }
                }

                if (topupRequestsForms != null && topupRequestsForms.Count > 0)
                {
                    message = message.Trim(',');
                    statusArray[0] = "103";
                    statusArray[1] = message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }

                var customer = new CustomerService().GetCustomer(Convert.ToInt32(customerId));
                var typesOfCards = new CardService().GetCardTypes(customer: customer);

                List<CardRequest> cardRequests = new List<CardRequest>();
                for (var i = 0; i < arrCardTypeIds.Count(); i++)
                {
                    var cardTypeId = int.Parse(arrCardTypeIds[i]);

                    CardRequest cr = new CardRequest();
                    cr.CardTypeId = cardTypeId;
                    cr.Fee = typesOfCards.Where(c => c.Id == cardTypeId).Select(c => c.Fee).Single();
                    cr.CardQty = 1;
                    cr.CardRequestFormId = customerCards[0].Id;
                    cardRequests.Add(cr);

                    totalAmount = totalAmount + (cr.Fee * 1);
                }

                var exchangeRate = new ExchangeRateService()
                                    .GetExchangeRate(new SettingService().CustomerDefaultCurrency.Id).Rate;

                var tempActualAmt = totalAmount * exchangeRate;
                tempActualAmt = tempActualAmt + ((tempActualAmt / 100) *
                    new SettingService().ExchangeRateServiceCharges);
                var amountToPay = Convert.ToInt32(tempActualAmt);

                if (amountToPay > customerCards[0].Payments[0].Amount)
                {
                    statusArray[0] = "104";
                    statusArray[1] = "Sorry !The record cannot be updated because, Current Amount is greater than Previously paid amount";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }

                var payment = new Payment();
                payment.Amount = amountToPay;

                bool isSaved = new CardService().UpdateCardRequests(cardRequest: cardRequests, payment: payment
                              , customer: customer);
                if (isSaved)
                {
                    //ViewBag.SuccessMessage = "Record Updated Successfully";
                    statusArray[0] = "200";
                    statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br /> <a id='lkGoBack' style='font-size: 14px; ' href='/Card/CardRequests'>Go Back</a></span>";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }

                statusArray[0] = "204";
                statusArray[1] = "Record not Updated";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            catch (UserException exception)
            {
                statusArray[0] = "205";
                statusArray[1] = "Record not Updated! Please try agan later";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
                statusArray[0] = "206";
                statusArray[1] = "Failed! Record not Updated! Please try agan later";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
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
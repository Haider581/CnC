using CnC.Core;
using CnC.Core.Cards;
using CnC.Core.Exceptions;
using CnC.Core.Payments;
using CnC.Core.TopUps;
using CnC.Data;
using log4net;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnC.Service
{
    public class TopUpService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        #region TopUp Request

        public int AddTopUpRequest(TopUpRequest topUpRequest)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    context.TopUpRequests.Add(topUpRequest);
                    context.SaveChanges();

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return topUpRequest.Id;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }
        public int AddTopUpRequestForm(RequestForm requestForm, List<TopUpRequest> topupRequests, List<Payment> payments)
        {
            if (requestForm == null)
                throw new ArgumentNullException("Request Form is required");

            if (requestForm.TypeId != (int)RequestFormType.TopUp)
                throw new ArgumentException("Request Form Type must be Top Up");

            if (requestForm.CustomerId <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (topupRequests == null || topupRequests.Count == 0)
                throw new ArgumentNullException("Top Up Requests are required");

            if (topupRequests.Any(tr => tr.CardId <= 0 || tr.Amount <= 0 || tr.ServiceFee <= 0))
                throw new ArgumentNullException("Top Up Requests are missing either Card, Amount or Service Fee");

            if (payments == null || payments.Count == 0)
                throw new ArgumentNullException("Payments are required");

            if (payments.Any(p => p.PaymentMethodId <= 0 || p.Amount <= 0))
                throw new ArgumentNullException("Payments are missing either Method, Amount or Request Form");

            if (payments.Any(p => p.PaymentMethodId == (int)PaymentMethod.Bank
            && (string.IsNullOrEmpty(p.TransactionAccountNo) || string.IsNullOrEmpty(p.TransactionName)
            || string.IsNullOrEmpty(p.TransactionNo))))
                throw new ArgumentNullException("Payments are missing Bank Transaction Info");

            decimal totalAmountToPay = 0;

            try
            {
                using (var context = new EntityContext())
                {
                    var customer = (from cust in context.Customers
                                    join user in context.Users on cust.UserId equals user.Id
                                    where user.Status == (int)UserStatus.Active
                                    && user.Id == requestForm.CustomerId
                                    select new { Customer = cust, User = user }).SingleOrDefault();

                    if (customer == null)
                        throw new UserException("Customer not found");

                    var exchangeRate = new ExchangeRateService().GetExchangeRate(customer.Customer.CurrencyId).Rate;

                    if (requestForm.ExchangeRate <= 0 || requestForm.ExchangeRate != exchangeRate)
                        throw new UserException("Exchange Rate is invalid");

                    var cardService = new CardService();

                    foreach (var topUpRequest in topupRequests)
                    {
                        var card = topUpRequest.Card;

                        if (card == null)
                            card = cardService.GetCard(topUpRequest.CardId);

                        if (card == null)
                            throw new UserException("Card " + card.Number + "not found");

                        var cardType = card.CardRequest.CardType;

                        if (!cardType.IsReloadable)
                            throw new UserException("Card " + card.Number + " does not support Top Up");

                        if (topUpRequest.Amount < cardType.MinimumReloadAtATime)
                            throw new UserException("Card " + card.Number + ": Top Up amount must be greater than minimum reload "
                                + cardType.MinimumReloadAtATime);

                        if (topUpRequest.Amount > cardType.MaximumReloadAtATime)
                            throw new UserException("Card " + card.Number + ": Top Up amount must be less than or equal to maximum reload "
                                + cardType.MaximumReloadAtATime);

                        if (topUpRequest.Amount > cardType.ReloadLimit)
                            throw new UserException("Card " + card.Number + ": Top Up amount is greater than supporting limit");

                        if (context.TopUpRequests.Count(tr => tr.CardId == card.CardRequestId) >= cardType.MaximumReload)
                            throw new UserException("Card " + card.Number + " has already reached maximum reload supported");

                        if (context.TopUpRequests.Any(tr => tr.CardId == card.CardRequestId))
                            if (context.TopUpRequests.Where(tr => tr.CardId == card.CardRequestId)
                                .Sum(tr => tr.Amount) + topUpRequest.Amount > cardType.ReloadLimit)
                                throw new UserException("Card " + card.Number + " maximum reload amount supported is voilated");

                        decimal serviceFee =
                            cardType.TopUpServiceFeeMinimum > (topUpRequest.Amount * cardType.TopUpServiceFeePercentage / 100)
                            ? cardType.TopUpServiceFeeMinimum : (topUpRequest.Amount * cardType.TopUpServiceFeePercentage / 100);

                        totalAmountToPay += (topUpRequest.Amount + serviceFee);
                    }

                    // In Customer Currency
                    totalAmountToPay = totalAmountToPay * exchangeRate;
                    var totalPaidAmount = payments.Sum(p => p.Amount);

                    if (totalPaidAmount < totalAmountToPay)
                        throw new UserException("Amount Paid cannot be less than Top Up Amount");

                    context.RequestForms.Add(requestForm);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    foreach (TopUpRequest topupRequestItem in topupRequests)
                    {
                        topupRequestItem.TopUpRequestFormId = requestForm.Id;

                        if (!new SettingService().TopUpApprovalRequired)
                            topupRequestItem.ApprovedOn = requestForm.CreatedOn;

                        context.TopUpRequests.Add(topupRequestItem);
                    }

                    foreach (Payment payment in payments)
                    {
                        payment.RequestFormId = requestForm.Id;
                        context.Payments.Add(payment);
                    }

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    customer.Customer.User = customer.User;
                    new MessageService().SendCustomerNewTopUpRequestMessage(customer.Customer,
                        requestForm, topupRequests, customer.Customer.LanguageId);

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name,
                                                 performedForUserId: customer.Customer.UserId);

                    return requestForm.Id;
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
                throw new UserException("Sorry, we are unable to process your request. Please try again later.");
            }
        }

        /// <summary>
        /// Return Top Up Request Form including all container objects e.g Top Requests
        /// </summary>
        public TopUpRequestForm GetTopUpRequestForm(int topUpRequestFormId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from rf in context.RequestForms
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId
                                  join topUp in context.TopUpRequests on rf.Id equals topUp.TopUpRequestFormId
                                  join cr in context.CardRequests on topUp.CardId equals cr.Id
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  join c in context.Cards on cr.Id equals c.CardRequestId
                                  where rf.Id == topUpRequestFormId
                                  && rf.TypeId == (int)RequestFormType.TopUp
                                  select new
                                  {
                                      TopUpRequest = topUp,
                                      Card = c,
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf,
                                      Customer = customer,
                                      Payment = payment
                                  })
                                  .ToList();

                    if (result != null)
                    {
                        var firstRecord = result.First();
                        TopUpRequestForm topUpRequestForm = new TopUpRequestForm(firstRecord.RequestForm);
                        topUpRequestForm.Customer = firstRecord.Customer;
                        topUpRequestForm.TopUpRequests = new List<TopUpRequest>();
                        topUpRequestForm.Payments = new List<Payment>();

                        foreach (var requestForm in result.DistinctBy(r => r.CardRequest))
                        {
                            requestForm.CardRequest.CardType = requestForm.CardType;
                            requestForm.Card.CardRequest = requestForm.CardRequest;
                            requestForm.TopUpRequest.Card = requestForm.Card;
                            topUpRequestForm.TopUpRequests.Add(requestForm.TopUpRequest);
                        }

                        foreach (var requestForm in result.DistinctBy(r => r.Payment))
                        {
                            topUpRequestForm.Payments.Add(requestForm.Payment);
                        }

                        return topUpRequestForm;
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public TopUpRequest GetTopUpRequest(int TopUpRequestId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.TopUpRequests.Where(x => x.Id == TopUpRequestId).SingleOrDefault();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        /// <summary>
        /// Return Top Up Requests Forms which are either pending for Payment Confirmation or failed to confirm payment
        /// </summary>
        public List<TopUpRequestForm> GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out int totalCount, int? customerId = null
            , string nic = null, string passportNo = null, string cardNumber = null, int? cardTypeId = null
            , DateTime? createdDateFrom = null, DateTime? createdDateTo = null
            , int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        {
            totalCount = 0;
            try
            {
                if (createdDateTo.HasValue)
                    createdDateTo = createdDateTo.Value.AddDays(1);

                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                int skipRows = pageSize.Value * pageIndex;

                using (var context = new EntityContext())
                {
                    var result = (from rf in context.RequestForms
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join user in context.Users on customer.UserId equals user.Id
                                  join topUp in context.TopUpRequests on rf.Id equals topUp.TopUpRequestFormId
                                  join cr in context.CardRequests on topUp.CardId equals cr.Id
                                  join card in context.Cards on topUp.CardId equals card.CardRequestId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
                                  from p in payments
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  where true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic) ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo) ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
                                  && true == (!string.IsNullOrEmpty(cardNumber)
                                  ? card.Number.Equals(cardNumber, StringComparison.OrdinalIgnoreCase) : true)
                                  && ct.Id == (cardTypeId.HasValue ? cardTypeId.Value : ct.Id)
                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && payments.Any(pay => pay.ConfirmedOn == null || pay.ReConfirmedOn
                                  == null
                                  || !string.IsNullOrEmpty(pay.ConfirmationFailureReason))
                                  && rf.TypeId == (int)RequestFormType.TopUp
                                  orderby rf.CreatedOn descending
                                  group
                                  new
                                  {
                                      TopUpRequest = topUp,
                                      Card = card,
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      Payment = p
                                  }
                                  by rf.Id)
                                  .OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);
                    totalCount = result.Count();
                    var topUpRequests = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (topUpRequests != null && topUpRequests.Count() > 0)
                    {
                        List<TopUpRequestForm> topUpRequestForms = new List<TopUpRequestForm>();

                        foreach (var grp in topUpRequests)
                        {
                            var firstRecord = grp.First();
                            TopUpRequestForm topUpRequestForm = new TopUpRequestForm(firstRecord.RequestForm);
                            topUpRequestForm.Customer = firstRecord.Customer;
                            topUpRequestForm.Customer.User = firstRecord.User;
                            topUpRequestForm.Payments = new List<Payment>();
                            topUpRequestForm.TopUpRequests = new List<TopUpRequest>();

                            foreach (var requestForm in grp.DistinctBy(r => r.TopUpRequest))
                            {
                                requestForm.CardRequest.CardType = requestForm.CardType;
                                requestForm.Card.CardRequest = requestForm.CardRequest;
                                requestForm.TopUpRequest.Card = requestForm.Card;
                                requestForm.TopUpRequest.StatusString =
                                   GetTopUpRequestStatusString(requestForm.TopUpRequest, false);
                                topUpRequestForm.TopUpRequests.Add(requestForm.TopUpRequest);
                            }

                            foreach (var requestForm in grp.DistinctBy(r => r.Payment))
                            {
                                topUpRequestForm.Payments.Add(requestForm.Payment);
                            }

                            topUpRequestForms.Add(topUpRequestForm);
                        }

                        return topUpRequestForms;
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        /// <summary>
        /// Return Top Up Request Forms under processing
        /// For Customer
        /// </summary>
        public List<TopUpRequestForm> GetTopUpRequestFormsUnderProcessing(out int totalCount, int? customerId = null
            , string cardNumber = null, int? cardTypeId = null
            , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, bool isForCustomer = false
            , int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        {
            totalCount = 0;
            try
            {
                if (createdDateTo.HasValue)
                    createdDateTo = createdDateTo.Value.AddDays(1);

                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                int skipRows = pageSize.Value * pageIndex;

                using (var context = new EntityContext())
                {
                    var result = (from rf in context.RequestForms
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join user in context.Users on customer.UserId equals user.Id
                                  join topUp in context.TopUpRequests on rf.Id equals topUp.TopUpRequestFormId
                                  join cr in context.CardRequests on topUp.CardId equals cr.Id
                                  join card in context.Cards on topUp.CardId equals card.CardRequestId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
                                  from p in payments
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  where rf.CustomerId == (customerId.HasValue ? customerId.Value : rf.CustomerId)
                                  && true == (!string.IsNullOrEmpty(cardNumber)
                                  ? card.Number.Equals(cardNumber, StringComparison.OrdinalIgnoreCase) : true)
                                  && ct.Id == (cardTypeId.HasValue ? cardTypeId.Value : ct.Id)
                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && (topUp.ApprovedOn == null || topUp.UploadedOn == null
                                  || !string.IsNullOrEmpty(topUp.FailureReason))
                                  && rf.TypeId == (int)RequestFormType.TopUp
                                  group
                                  new
                                  {
                                      TopUpRequest = topUp,
                                      Card = card,
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      Payment = p
                                  }
                                  by rf.Id).OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);
                    totalCount = result.Count();
                    var topUpRequests = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (topUpRequests != null && topUpRequests.Count() > 0)
                    {
                        List<TopUpRequestForm> topUpRequestForms = new List<TopUpRequestForm>();

                        foreach (var grp in topUpRequests)
                        {
                            var firstRecord = grp.First();
                            TopUpRequestForm topUpRequestForm = new TopUpRequestForm(firstRecord.RequestForm);
                            topUpRequestForm.Customer = firstRecord.Customer;
                            topUpRequestForm.Customer.User = firstRecord.User;
                            topUpRequestForm.Payments = new List<Payment>();
                            topUpRequestForm.TopUpRequests = new List<TopUpRequest>();

                            foreach (var requestForm in grp.DistinctBy(r => r.TopUpRequest))
                            {
                                requestForm.CardRequest.CardType = requestForm.CardType;
                                requestForm.Card.CardRequest = requestForm.CardRequest;
                                requestForm.TopUpRequest.Card = requestForm.Card;
                                requestForm.TopUpRequest.StatusString =
                                    GetTopUpRequestStatusString(requestForm.TopUpRequest, isForCustomer);
                                topUpRequestForm.TopUpRequests.Add(requestForm.TopUpRequest);
                            }

                            foreach (var requestForm in grp.DistinctBy(r => r.Payment))
                            {
                                topUpRequestForm.Payments.Add(requestForm.Payment);
                            }

                            topUpRequestForms.Add(topUpRequestForm);
                        }

                        return topUpRequestForms;
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        /// <summary>
        /// Return Card Request Forms pending for processing
        /// For Staff
        /// </summary>
        public List<TopUpRequestForm> GetTopUpRequestFormsPendingForProcessing(out int totalCount, int? customerId = null
            , string nic = null, string passportNo = null, string cardNumber = null, int? cardTypeId = null
            , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, bool isForCustomer = false
            , int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc, bool showFailed = true)
        {
            totalCount = 0;
            try
            {
                if (createdDateTo.HasValue)
                    createdDateTo = createdDateTo.Value.AddDays(1);

                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                int skipRows = pageSize.Value * pageIndex;

                using (var context = new EntityContext())
                {
                    var result = (from rf in context.RequestForms
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join user in context.Users on customer.UserId equals user.Id
                                  join topUp in context.TopUpRequests on rf.Id equals topUp.TopUpRequestFormId
                                  join cr in context.CardRequests on topUp.CardId equals cr.Id
                                  join card in context.Cards on topUp.CardId equals card.CardRequestId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
                                  from p in payments
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  where true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic) ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo) ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase)
                                  : true)
                                  && true == (!string.IsNullOrEmpty(cardNumber)
                                  ? card.Number.Equals(cardNumber, StringComparison.OrdinalIgnoreCase) : true)
                                  && ct.Id == (cardTypeId.HasValue ? cardTypeId.Value : ct.Id)
                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && payments.All(pay => pay.ConfirmedOn != null && pay.ReConfirmedOn != null
                                  && string.IsNullOrEmpty(pay.ConfirmationFailureReason))
                                  && ((topUp.ApprovedOn == null)
                                  || (topUp.UploadedOn == null && string.IsNullOrEmpty(topUp.FailureReason))
                                  || (showFailed && !string.IsNullOrEmpty(topUp.FailureReason)))
                                  && rf.TypeId == (int)RequestFormType.TopUp
                                  orderby rf.CreatedOn descending
                                  group
                                  new
                                  {
                                      TopUpRequest = topUp,
                                      Card = card,
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      Payment = p
                                  }
                                  by rf.Id).OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);

                    totalCount = result.Count();
                    var topUpRequests = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (topUpRequests != null && topUpRequests.Count() > 0)
                    {
                        List<TopUpRequestForm> topUpRequestForms = new List<TopUpRequestForm>();

                        foreach (var grp in topUpRequests)
                        {
                            var firstRecord = grp.First();
                            TopUpRequestForm topUpRequestForm = new TopUpRequestForm(firstRecord.RequestForm);
                            topUpRequestForm.Customer = firstRecord.Customer;
                            topUpRequestForm.Customer.User = firstRecord.User;
                            topUpRequestForm.Payments = new List<Payment>();
                            topUpRequestForm.TopUpRequests = new List<TopUpRequest>();

                            foreach (var requestForm in grp.DistinctBy(r => r.TopUpRequest))
                            {
                                requestForm.CardRequest.CardType = requestForm.CardType;
                                requestForm.Card.CardRequest = requestForm.CardRequest;
                                requestForm.TopUpRequest.Card = requestForm.Card;
                                requestForm.TopUpRequest.StatusString =
                                    GetTopUpRequestStatusString(requestForm.TopUpRequest, isForCustomer);
                                topUpRequestForm.TopUpRequests.Add(requestForm.TopUpRequest);
                            }

                            foreach (var requestForm in grp.DistinctBy(r => r.Payment))
                            {
                                topUpRequestForm.Payments.Add(requestForm.Payment);
                            }

                            topUpRequestForm.PaymentToPay = Convert.ToInt32(topUpRequestForm.TopUpRequests
                                .Sum(tr => (tr.Amount + tr.ServiceFee) * (topUpRequestForm.ExchangeRate
                                + (topUpRequestForm.ExchangeRate * new SettingService().ExchangeRateServiceCharges / 100))));
                            topUpRequestForm.PaymentPaid = topUpRequestForm.Payments.Sum(p => p.Amount);

                            topUpRequestForms.Add(topUpRequestForm);
                        }

                        return topUpRequestForms;
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<TopUpRequestForm> GetTopUpRequestForms(out int totalCount, int? customerId = null, string nic = null, string passportNo = null
            , string cardNumber = null, int? cardTypeId = null, RequestFormStatus? requestFormStatus = null
            , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, TopUpRequestStatus? topUpRequestStatus = null
            , int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        {
            totalCount = 0;
            try
            {
                if (createdDateTo.HasValue)
                    createdDateTo = createdDateTo.Value.AddDays(1);

                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                int skipRows = pageSize.Value * pageIndex;

                using (var context = new EntityContext())
                {
                    var result = (from rf in context.RequestForms
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join user in context.Users on customer.UserId equals user.Id
                                  join topUp in context.TopUpRequests on rf.Id equals topUp.TopUpRequestFormId
                                  join cr in context.CardRequests on topUp.CardId equals cr.Id
                                  join card in context.Cards on topUp.CardId equals card.CardRequestId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
                                  from p in payments
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  where true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic) ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo) ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase)
                                  : true)
                                  && true == (!string.IsNullOrEmpty(cardNumber)
                                  ? card.Number.Equals(cardNumber, StringComparison.OrdinalIgnoreCase) : true)
                                  && ct.Id == (cardTypeId.HasValue ? cardTypeId.Value : ct.Id)
                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && true == (requestFormStatus.HasValue ?
                                  (requestFormStatus.Value == RequestFormStatus.PaymentConfirmed
                                  && payments.All(pay => pay.ConfirmedOn != null && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                                  || (requestFormStatus.Value == RequestFormStatus.PaymentConfirmationFailed
                                  && payments.Any(pay => pay.ConfirmedOn != null && !string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                                  || (requestFormStatus.HasValue && requestFormStatus.Value == RequestFormStatus.Pending
                                  && payments.Any(pay => pay.ConfirmedOn == null && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                                  : true)
                                  && (true == (requestFormStatus.HasValue && requestFormStatus.Value == RequestFormStatus.SentToCardIssuer
                                  ? rf.SentToCardIssuerOn != null : true))
                                  && true == (topUpRequestStatus.HasValue ?
                                  (topUpRequestStatus.Value == TopUpRequestStatus.Pending
                                  && topUp.UploadedOn == null && string.IsNullOrEmpty(topUp.FailureReason))
                                  || (topUpRequestStatus.Value == TopUpRequestStatus.Approved
                                  && topUp.ApprovedOn != null && string.IsNullOrEmpty(topUp.FailureReason))
                                  || (topUpRequestStatus.Value == TopUpRequestStatus.Uploaded
                                  && topUp.UploadedOn != null && string.IsNullOrEmpty(topUp.FailureReason))
                                  || (topUpRequestStatus.Value == TopUpRequestStatus.Failed
                                  && !string.IsNullOrEmpty(topUp.FailureReason))
                                  || (topUpRequestStatus.Value == TopUpRequestStatus.History
                                  && topUp.UploadedOn != null && topUp.ApprovedOn != null
                                  && string.IsNullOrEmpty(topUp.FailureReason))
                                  : true)
                                  && rf.TypeId == (int)RequestFormType.TopUp
                                  orderby rf.CreatedOn descending
                                  group
                                  new
                                  {
                                      TopUpRequest = topUp,
                                      Card = card,
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      Payment = p
                                  }
                                  by rf.Id).OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);
                    totalCount = result.Count();
                    var topUpRequests = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (topUpRequests != null && topUpRequests.Count() > 0)
                    {
                        List<TopUpRequestForm> topUpRequestForms = new List<TopUpRequestForm>();

                        foreach (var grp in topUpRequests)
                        {
                            var firstRecord = grp.First();
                            TopUpRequestForm topUpRequestForm = new TopUpRequestForm(firstRecord.RequestForm);
                            topUpRequestForm.Customer = firstRecord.Customer;
                            topUpRequestForm.Customer.User = firstRecord.User;
                            topUpRequestForm.Payments = new List<Payment>();
                            topUpRequestForm.TopUpRequests = new List<TopUpRequest>();

                            foreach (var requestForm in grp.DistinctBy(r => r.TopUpRequest))
                            {
                                requestForm.CardRequest.CardType = requestForm.CardType;
                                requestForm.Card.CardRequest = requestForm.CardRequest;
                                requestForm.TopUpRequest.Card = requestForm.Card;
                                requestForm.TopUpRequest.StatusString =
                                  GetTopUpRequestStatusString(requestForm.TopUpRequest);
                                topUpRequestForm.TopUpRequests.Add(requestForm.TopUpRequest);
                            }

                            foreach (var requestForm in grp.DistinctBy(r => r.Payment))
                            {
                                topUpRequestForm.Payments.Add(requestForm.Payment);
                            }

                            topUpRequestForms.Add(topUpRequestForm);
                        }

                        return topUpRequestForms;
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<TopUpRequest> GetTopUpRequests(int? customerId = null, string nic = null, string passportNo = null
            , int? cardRequestFormId = null, string cardNumber = null, int? cardTypeId = null
            , DateTime? creationDateFrom = null, DateTime? creationDateTo = null
            , TopUpRequestStatus? topUpRequestStatus = null
            , int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        {
            try
            {
                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                var topUpRequests = new List<TopUpRequest>();
                using (var context = new EntityContext())
                {
                    var query = (from t in context.TopUpRequests
                                 select t);
                    if (customerId != null)
                    {
                        //query = query.Where(q => q.cu == customerId);
                    }
                    if (cardRequestFormId != null)
                    {
                        query = query.Where(q => q.TopUpRequestFormId == cardRequestFormId);
                    }
                    if (cardNumber != null)
                    {
                        query = query.Where(q => q.Card.Number == cardNumber);
                    }
                    if (cardTypeId != null)
                    {
                        // query = query.Where(q => q. == cardNumber);
                    }
                    if (creationDateFrom != null && creationDateTo != null)
                    {
                        query = query.Where(q => q.CreatedOn >= creationDateFrom && q.CreatedOn <= creationDateTo);
                    }
                    if (topUpRequestStatus != null)
                    {
                        //In Case of Hitory
                        if (topUpRequestStatus == TopUpRequestStatus.History)
                        {
                            query = query.Where(x => x.UploadedOn != null && x.ApprovedOn != null);
                        }
                    }
                    var topUpRequestsRes = query.ToList();
                    if (topUpRequestsRes.Count > 0)
                    {
                        foreach (var item in topUpRequestsRes)
                        {
                            var topUpRequest = new TopUpRequest();
                            topUpRequest.Id = item.Id;
                            topUpRequest.TopUpRequestFormId = item.TopUpRequestFormId;
                            topUpRequest.Amount = item.Amount;
                            topUpRequest.CreatedOn = item.CreatedOn;
                            topUpRequest.Card = new CardService().GetCard(item.CardId);
                            topUpRequest.UploadedOn = item.UploadedOn;
                            topUpRequest.ApprovedOn = item.ApprovedOn;
                            topUpRequests.Add(topUpRequest);

                        }

                        if (topUpRequestStatus != null && topUpRequestStatus == TopUpRequestStatus.History)
                            return topUpRequests.Where(r => GetTopUpRequestStatus(r) != TopUpRequestStatus.Pending).ToList();
                        else if (topUpRequestStatus != null)
                            return topUpRequests.Where(r => GetTopUpRequestStatus(r) == topUpRequestStatus).ToList();
                        else
                            return topUpRequests;
                    }

                    return topUpRequests;
                }

            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }

        }

        /// Will use for Dashboard
        /// </summary>
        /// <param name="topUpRequestStatus"></param>
        /// <returns></returns>
        public int GetTopUpRequestsCountByStatus(TopUpRequestStatus topUpRequestStatus)
        {
            try
            {
                return GetTopUpRequests(topUpRequestStatus: topUpRequestStatus).Count();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return -1;
            }
        }

        public List<TopUpRequest> GetTopUpHistory(int? customerId = null, string nic = null, string passportNo = null
            , int? cardRequestFormId = null, string cardNumber = null
            , int? cardTypeId = null, DateTime? creationDateFrom = null, DateTime? creationDateTo = null)
        {
            try
            {
                return GetTopUpRequests(customerId, nic, passportNo, cardRequestFormId, cardNumber, cardTypeId, creationDateFrom, creationDateTo
                    , TopUpRequestStatus.History);
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        /// <summary>
        /// Return Top Up Request Form Status String
        /// </summary>
        public string GetRequestFormStatusString(RequestForm requestForm, bool isForCustomer = false)
        {
            try
            {
                if (isForCustomer)
                {
                    if (requestForm.Payments.All(pay => pay.ConfirmedOn != null && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Processing";
                    else if (requestForm.Payments.Any(pay => pay.ConfirmedOn != null && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Confirmation Failed";
                    else
                        return "Payment Confirmation Underway";
                }
                else
                {
                    if (requestForm.Payments.All(pay => pay.ConfirmedOn != null && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Confirmed";
                    else if (requestForm.Payments.Any(pay => pay.ConfirmedOn != null && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Confirmation Failed";
                    else
                        return "Waiting For Payment Confirmation";
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public TopUpRequestStatus GetTopUpRequestStatus(TopUpRequest topUpRequest)
        {
            try
            {
                if (topUpRequest.UploadedOn != null && !string.IsNullOrEmpty(topUpRequest.FailureReason))
                    return TopUpRequestStatus.Failed;
                else if (topUpRequest.UploadedOn != null)
                    return TopUpRequestStatus.Uploaded;
                else return TopUpRequestStatus.Pending;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return 0;
            }
        }

        public string GetTopUpRequestStatusString(TopUpRequest topUpRequest, bool isForCustomer = false)
        {
            if (topUpRequest == null || topUpRequest.TopUpRequestFormId <= 0)
                throw new ArgumentException("Top Up Request or Form Id is invalid");

            try
            {
                if (topUpRequest.TopUpRequestForm == null)
                    topUpRequest.TopUpRequestForm = GetTopUpRequestForm(topUpRequest.TopUpRequestFormId);

                if (topUpRequest.TopUpRequestForm == null)
                    throw new UserException("Top Up Request not found");

                if (topUpRequest.TopUpRequestForm.Payments == null)
                    topUpRequest.TopUpRequestForm.Payments = new PaymentService().GetPaymentsByRequestFormId(topUpRequest.TopUpRequestFormId);

                if (topUpRequest.TopUpRequestForm.Payments == null)
                    throw new UserException("Request Form Payments not found");


                topUpRequest.TopUpRequestForm.PaymentToPay = Convert.ToInt32(topUpRequest.TopUpRequestForm.TopUpRequests
                    .Sum(tr => (tr.Amount + tr.ServiceFee) * (topUpRequest.TopUpRequestForm.ExchangeRate
                    + (topUpRequest.TopUpRequestForm.ExchangeRate * new SettingService().ExchangeRateServiceCharges / 100))));

                topUpRequest.TopUpRequestForm.PaymentPaid = topUpRequest.TopUpRequestForm.Payments.Sum(p => p.Amount);

                if (topUpRequest.TopUpRequestForm.PaymentToPay > topUpRequest.TopUpRequestForm.PaymentPaid)
                    return "Insufficient Payment";

                if (isForCustomer)
                {
                    if (topUpRequest.TopUpRequestForm.Payments.Any(pay => pay.ConfirmedOn == null
                    && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Confirmation Underway";
                    else if (topUpRequest.TopUpRequestForm.Payments.Any(pay => pay.ReConfirmedOn == null
                    && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Re-confirmation Underway";
                    else if (topUpRequest.TopUpRequestForm.Payments.Any(pay => (pay.ConfirmedOn != null
                   || pay.ReConfirmedOn != null) && !string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Confirmation Failed";
                    else if (!string.IsNullOrEmpty(topUpRequest.FailureReason))
                        return "Failed";
                    else if (topUpRequest.ApprovedOn != null && topUpRequest.UploadedOn != null)
                        return "Uploaded";
                    else
                        return "Processing";
                }
                else
                {
                    if (topUpRequest.TopUpRequestForm.Payments.Any(pay => pay.ConfirmedOn == null
                     && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Waiting For Payment Confirmation";
                    else if (topUpRequest.TopUpRequestForm.Payments.Any(pay => pay.ReConfirmedOn == null
                    && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Waiting For Payment Reconfirmation";
                    else if (topUpRequest.TopUpRequestForm.Payments.Any(pay => (pay.ConfirmedOn != null
                   || pay.ReConfirmedOn != null) && !string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Confirmation Failed";
                    else if (!string.IsNullOrEmpty(topUpRequest.FailureReason))
                        return "Failed";
                    else if (topUpRequest.UploadedOn != null && topUpRequest.ApprovedOn != null)
                        return "Uploaded";
                    else if (topUpRequest.ApprovedOn == null)
                        return "Waiting For Approval";
                    else
                        return "Waiting for processing";
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
                return null;
            }
        }

        /// <summary>
        /// Set Top Up Request Uploaded or Failed
        /// In case of Uploaded pass Failure Reason null
        /// In case of Failed pass message in Failure Reason
        /// </summary>
        public bool SetTopUpRequestUploadedOrFailed(int topUpRequestId, string failureReason)
        {
            if (topUpRequestId <= 0)
                throw new ArgumentNullException("Top Up Request Id is invalid");

            try
            {
                using (var context = new EntityContext())
                {
                    var topUpRequest = (from tr in context.TopUpRequests
                                        join rf in context.RequestForms on tr.TopUpRequestFormId equals rf.Id
                                        join payment in context.Payments on rf.Id equals payment.RequestFormId
                                        into payments
                                        join topUpReq in context.TopUpRequests on rf.Id equals topUpReq.TopUpRequestFormId
                                        into topUpRequests
                                        join customer in context.Customers on rf.CustomerId equals customer.UserId
                                        join user in context.Users on customer.UserId equals user.Id
                                        where tr.Id == topUpRequestId
                                        && (tr.UploadedOn == null
                                        || (tr.UploadedOn != null || tr.ApprovedOn != null
                                        && !string.IsNullOrEmpty(tr.FailureReason)))
                                        && payments.All(pay => pay.ConfirmedOn != null
                                        && string.IsNullOrEmpty(pay.ConfirmationFailureReason))
                                        && payments.Sum(pay => pay.Amount) >= topUpRequests.Sum(
                                            topReq => (topReq.Amount + topReq.ServiceFee) * rf.ExchangeRate)
                                        select new
                                        {
                                            TopUpRequest = tr,
                                            RequestForm = rf,
                                            Customer = customer,
                                            User = user
                                        })
                                        .SingleOrDefault();

                    if (topUpRequest == null)
                        throw new UserException("Either Top Up Request not found, already uploaded or its Payment not confirmed");

                    if (topUpRequest.TopUpRequest.UploadedOn == null ||
                        (topUpRequest.TopUpRequest.UploadedOn != null &&
                        !string.IsNullOrEmpty(topUpRequest.TopUpRequest.FailureReason)
                        && topUpRequest.TopUpRequest.ApprovedOn == null))
                    {
                        topUpRequest.TopUpRequest.UploadedOn = DateTime.Now;

                        // If Top Up Approval is not required
                        if (!new SettingService().TopUpApprovalRequired
                            && topUpRequest.TopUpRequest.ApprovedOn == null)
                            topUpRequest.TopUpRequest.ApprovedOn = DateTime.Now;

                        topUpRequest.TopUpRequest.FailureReason = string.IsNullOrEmpty(failureReason) ?
                            null : failureReason;

                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to save");

                        // If Payment Confirmation failed then send Customer email with reason
                        if (!string.IsNullOrEmpty(failureReason))
                        {
                            topUpRequest.Customer.User = topUpRequest.User;
                            new MessageService().SendCustomerTopUpRequestFailedMessage(topUpRequest.Customer
                                , topUpRequest.RequestForm, topUpRequest.TopUpRequest, failureReason
                                , topUpRequest.Customer.LanguageId);
                        }
                        if (string.IsNullOrEmpty(failureReason))
                        {
                            topUpRequest.Customer.User = topUpRequest.User;
                            new MessageService().SendCustomerTopUpRequestCompletedMessage(topUpRequest.Customer
                                , topUpRequest.RequestForm, topUpRequest.TopUpRequest, topUpRequest.Customer.LanguageId);
                        }
                        new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name,
                                                       performedForUserId: topUpRequest.Customer.UserId);

                        return true;
                    }

                    if (topUpRequest.TopUpRequest.ApprovedOn == null ||
                        (topUpRequest.TopUpRequest.ApprovedOn != null
                        && !string.IsNullOrEmpty(topUpRequest.TopUpRequest.FailureReason)))
                    {
                        topUpRequest.TopUpRequest.ApprovedOn = DateTime.Now;
                        topUpRequest.TopUpRequest.FailureReason = string.IsNullOrEmpty(failureReason) ?
                            null : failureReason;
                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to save");

                        // If Payment Confirmation failed then send Customer email with reason
                        if (!string.IsNullOrEmpty(failureReason))
                        {
                            topUpRequest.Customer.User = topUpRequest.User;
                            new MessageService().SendCustomerTopUpRequestFailedMessage(topUpRequest.Customer, topUpRequest.RequestForm
                                    , topUpRequest.TopUpRequest, failureReason, topUpRequest.Customer.LanguageId);
                        }
                        if (string.IsNullOrEmpty(failureReason))
                        {
                            topUpRequest.Customer.User = topUpRequest.User;
                            new MessageService().SendCustomerTopUpRequestCompletedMessage(topUpRequest.Customer
                                , topUpRequest.RequestForm, topUpRequest.TopUpRequest, topUpRequest.Customer.LanguageId);
                        }
                        new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                        return true;
                    }
                    return false;
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
                throw new UserException("Sorry, we are unable to process your request. Please try again later.");
            }
        }

        #endregion

    }
}

using CnC.Core;
using CnC.Core.Cards;
using CnC.Core.Payments;
using CnC.Data;
using log4net;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using CnC.Core.Exceptions;
using CnC.Core.Customers;
using CnC.Core.Discounts;
using CnC.Core.Affiliates;

namespace CnC.Service
{
    public class CardService
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        private EntityContext context = new EntityContext();

        #region Card Type
        public bool AddCardType(CardType cardType)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);
                    context.CardTypes.Add(cardType);
                    if (context.SaveChanges() <= 0)
                        return false;

                    return true;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }
        public CardType GetCardType(int cardTypeId)
        {
            try
            {
                return context.CardTypes.FirstOrDefault(ct => ct.Id == cardTypeId);
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public List<CardType> GetCardTypes(Customer customer = null, bool? isReloadable = null, bool? isActive = true)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    List<CardType> cardTypes = context.CardTypes.Where(ct =>
                    ct.IsActive == (isActive.HasValue ? isActive.Value : ct.IsActive)
                    && ct.IsReloadable == (isReloadable.HasValue ? isReloadable.Value : ct.IsReloadable))
                    .OrderByDescending(ct => ct.Fee).ToList();

                    var discountsPromotions = new DiscountsPromotionService().GetDiscountsPromotions();

                    if ((discountsPromotions == null || discountsPromotions.Count <= 0)
                        && (customer == null || customer.AffiliateId == null))
                        return cardTypes;

                    if (discountsPromotions.Count > 0 && (customer == null || customer.AffiliateId == null))
                    {
                        return CalculateDiscounts(cardTypes: cardTypes, discountPromotions: discountsPromotions);
                    }

                    if (customer != null && customer.AffiliateId != null)
                    {
                        var customerDiscounts = new CustomerService().GetAffiliateDiscounts(customer: customer);

                        if (customerDiscounts == null || customerDiscounts.Count() <= 0)
                        {
                            if (discountsPromotions == null || discountsPromotions.Count <= 0)
                                return cardTypes;
                            else
                            {
                                return CalculateDiscounts(cardTypes: cardTypes, discountPromotions: discountsPromotions);
                            }
                        }

                        if (customerDiscounts.Count > 0 && discountsPromotions.Count <= 0)
                        {
                            return CalculateDiscounts(cardTypes: cardTypes, affiliateDiscounts: customerDiscounts);
                        }

                        if (customerDiscounts.Count > 0 && discountsPromotions.Count > 0)
                        {
                            return CalculateDiscounts(cardTypes: cardTypes, affiliateDiscounts: customerDiscounts,
                                  discountPromotions: discountsPromotions);
                        }
                    }

                    return cardTypes;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        private List<CardType> CalculateDiscounts(List<CardType> cardTypes,
            List<DiscountPromotion> discountPromotions = null, List<AffiliateDiscount> affiliateDiscounts = null)
        {
            if ((affiliateDiscounts != null && affiliateDiscounts.Count() > 0)
                && (discountPromotions != null && discountPromotions.Count() > 0))
            {
                foreach (var cardType in cardTypes)
                {
                    var customerCardDiscount = affiliateDiscounts
                        .SingleOrDefault(crd => crd.CardTypeId.Equals(cardType.Id));

                    var discountPromotion = discountPromotions
                            .SingleOrDefault(crd => crd.CardTypeId == cardType.Id);

                    if (customerCardDiscount != null && discountPromotion != null)
                    {
                        if (customerCardDiscount.Discount > discountPromotion.Discount)
                        {
                            if (customerCardDiscount.IsPercent)
                                cardType.Fee = Math.Round((cardType.Fee) - ((cardType.Fee / 100)
                                                * customerCardDiscount.Discount), 2);

                            else
                                cardType.Fee = Math.Round(cardType.Fee - customerCardDiscount.Discount, 2);
                        }
                        if (customerCardDiscount.Discount == discountPromotion.Discount)
                        {
                            if (customerCardDiscount.IsPercent)
                                cardType.Fee = Math.Round((cardType.Fee) - ((cardType.Fee / 100)
                                                * customerCardDiscount.Discount), 2);

                            else
                                cardType.Fee = Math.Round(cardType.Fee - customerCardDiscount.Discount, 2);
                        }
                        if (customerCardDiscount.Discount < discountPromotion.Discount)
                        {
                            if (discountPromotion.IsPercent)
                                cardType.Fee = Math.Round((cardType.Fee) - ((cardType.Fee / 100)
                                                * discountPromotion.Discount), 2);

                            else
                                cardType.Fee = Math.Round(cardType.Fee - discountPromotion.Discount, 2);
                        }

                    }
                    if (customerCardDiscount != null && discountPromotion == null)
                    {
                        if (customerCardDiscount.IsPercent)
                            cardType.Fee = Math.Round((cardType.Fee) - ((cardType.Fee / 100)
                                            * customerCardDiscount.Discount), 2);

                        else
                            cardType.Fee = Math.Round(cardType.Fee - customerCardDiscount.Discount, 2);
                    }

                    if (customerCardDiscount == null && discountPromotion != null)
                    {
                        if (discountPromotion.IsPercent)
                            cardType.Fee = Math.Round((cardType.Fee) - ((cardType.Fee / 100)
                                            * discountPromotion.Discount), 2);

                        else
                            cardType.Fee = Math.Round(cardType.Fee - discountPromotion.Discount, 2);
                    }
                }
                return cardTypes;
            }

            if (discountPromotions != null && discountPromotions.Count() > 0 && (affiliateDiscounts == null
                  || affiliateDiscounts.Count <= 0))
            {
                foreach (var cardType in cardTypes)
                {
                    var discountPromotion = discountPromotions
                        .SingleOrDefault(crd => crd.CardTypeId == cardType.Id);

                    if (discountPromotion != null)
                    {
                        if (discountPromotion.IsPercent)
                            cardType.Fee = Math.Round((cardType.Fee) - ((cardType.Fee / 100)
                                            * discountPromotion.Discount), 2);

                        else
                            cardType.Fee = Math.Round(cardType.Fee - discountPromotion.Discount, 2);
                    }
                }

                return cardTypes;
            }

            if (affiliateDiscounts != null && affiliateDiscounts.Count > 0 && (discountPromotions == null
                || discountPromotions.Count() <= 0))
            {
                foreach (var cardType in cardTypes)
                {
                    var customerCardDiscount = affiliateDiscounts
                        .SingleOrDefault(crd => crd.CardTypeId.Equals(cardType.Id));

                    if (customerCardDiscount != null)
                    {
                        if (customerCardDiscount.IsPercent)
                            cardType.Fee = Math.Round((cardType.Fee) - ((cardType.Fee / 100)
                                            * customerCardDiscount.Discount), 2);

                        else
                            cardType.Fee = Math.Round(cardType.Fee - customerCardDiscount.Discount, 2);
                    }
                }
                return cardTypes;
            }

            return cardTypes;
        }

        public List<CardType> GetAllCardTypes(out int totalCount, string name = null, int pageIndex = 0,
            int? pageSize = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var cardTypes = (from query in context.CardTypes
                                     where true == (!string.IsNullOrEmpty(name) ? query.Name.ToLower()
                                     .Contains(name.ToLower()) : true)
                                     select query);

                    totalCount = cardTypes.Count();
                    if (cardTypes != null && cardTypes.Count() > 0)
                    {

                        return cardTypes.OrderBy(c => c.Name).Skip(skipRows).Take(pageSize.Value).ToList();
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
        public bool UpdateCardType(CardType cardType)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (cardType != null)
                    {
                        var cardTypes = context.CardTypes.SingleOrDefault(ct => ct.Id == cardType.Id);
                        cardTypes.Name = cardType.Name;
                        cardTypes.Fee = cardType.Fee;
                        cardTypes.IsReloadable = cardType.IsReloadable;
                        cardTypes.ReloadLimit = cardType.ReloadLimit;
                        cardTypes.ReloadDayLimit = cardType.ReloadDayLimit;
                        cardTypes.MaximumReload = cardType.MaximumReload;
                        cardTypes.MinimumReloadAtATime = cardType.MinimumReloadAtATime;
                        cardTypes.MaximumReloadAtATime = cardType.MaximumReloadAtATime;
                        cardTypes.MaxQuantity = cardType.MaxQuantity;
                        cardTypes.IsProofOfSourceOfFundsRequired = cardType.IsProofOfSourceOfFundsRequired;
                        cardTypes.IsProofOfAddressRequired = cardType.IsProofOfAddressRequired;
                        cardTypes.Requirements = cardType.Requirements;
                        cardTypes.Description = cardType.Description;
                        cardTypes.IsActive = cardType.IsActive;
                        cardTypes.TopUpServiceFeePercentage = cardType.TopUpServiceFeePercentage;
                        cardTypes.TopUpServiceFeeMinimum = cardType.TopUpServiceFeeMinimum;
                        if (context.SaveChanges() <= 0)
                        {
                            return false;
                        }
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }
        #endregion Card Type

        #region Card Request Form

        public int AddCardRequestForm(RequestForm requestForm, List<CardRequest> cardRequests, List<Payment> payments
            , bool isNewCustomer, string proofOfAddressDocType = null, List<string> proofOfAddressDocs = null
            , string proofOfSourceOfFundsDocType = null, List<string> proofOfSourceOfFundsDocs = null)
        {
            if (requestForm == null)
                throw new ArgumentNullException("Request Form is required");

            if (requestForm.TypeId != (int)RequestFormType.Card)
                throw new ArgumentException("Request Form Type must be Card");

            if (requestForm.CustomerId <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (cardRequests == null || cardRequests.Count == 0)
                throw new ArgumentNullException("Card Requests are required");

            if (payments == null || payments.Count == 0)
                throw new ArgumentNullException("Payments are required");

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

                    // If New Customer then its Card Request Form must not exist already
                    var customerCardRequests = GetCardRequests(customerId: customer.Customer.UserId);

                    if (isNewCustomer && customerCardRequests != null && customerCardRequests.Count > 0)
                        throw new UserException("Customer Card Requests already exist");

                    bool isProofOfAddressRequired = false;
                    bool IsProofOfSourceOfFundsRequired = false;

                    List<CardRequest> cardRequestsToAdd = new List<CardRequest>();

                    // Check whether Proof Of Address or Source Of Funds documents required
                    foreach (var cardRequest in cardRequests)
                    {
                        var cardType = cardRequest.CardType != null ? cardRequest.CardType :
                            context.CardTypes.SingleOrDefault(ct => ct.Id == cardRequest.CardTypeId);

                        if (cardType.IsProofOfAddressRequired
                            && (isNewCustomer ||
                            string.IsNullOrEmpty(customer.Customer.ProofOfAddressDocInCustomerLanguage)))
                        {
                            if (string.IsNullOrEmpty(proofOfAddressDocType)
                            || proofOfAddressDocs == null || proofOfAddressDocs.Count == 0)
                                throw new UserException("Proof of Address document and type are required");

                            if (proofOfAddressDocs.Any(d => !(d.ToLower().Contains(".jpeg")
                            || d.ToLower().Contains(".jpg")
                            || d.ToLower().Contains(".png"))))
                                throw new UserException("Only JPEG, JPG or PNG are allowed in Proof of Address Docs");

                            if (proofOfAddressDocs.Any(d => (d.ToLower().Contains(".jpeg") && d.Length < 5)
                                || d.Length < 4))
                                throw new ArgumentException("Proof of Address Docs length is invalid");
                        }

                        if (cardType.IsProofOfSourceOfFundsRequired
                            && (isNewCustomer ||
                            string.IsNullOrEmpty(customer.Customer.ProofOfSourceOfFundsDocInCustomerLanguage)))
                        {
                            if (string.IsNullOrEmpty(proofOfSourceOfFundsDocType)
                            || proofOfSourceOfFundsDocs == null || proofOfSourceOfFundsDocs.Count == 0)
                                throw new UserException("Proof of Source of Funds documents and type are required");

                            if (proofOfSourceOfFundsDocs.Any(d => !(d.ToLower().Contains(".jpeg")
                            || d.ToLower().Contains(".jpg")
                            || d.ToLower().Contains(".png"))))
                                throw new ArgumentException("Only JPEG, JPG or PNG are allowed in Proof of Source of Funds Docs");

                            if (proofOfSourceOfFundsDocs.Any(d => (d.ToLower().Contains(".jpeg") && d.Length < 5)
                            || d.Length < 4))
                                throw new ArgumentException("Proof of Source Of Funds Docs length is invalid");
                        }

                        bool isBlackCardAlreadyExist = false;
                        var blackCardsAlreadyExists = customerCardRequests
                                         .Where(c => c.CardType.IsProofOfAddressRequired).ToList();
                        if (blackCardsAlreadyExists != null && blackCardsAlreadyExists.Count() > 0)
                        {
                            var newCardRequestsContainsBlackCards = cardRequests
                                            .SingleOrDefault(c => c.CardTypeId == blackCardsAlreadyExists[0].CardTypeId);
                            if (newCardRequestsContainsBlackCards != null)
                                isBlackCardAlreadyExist = true;
                        }

                        if (!isBlackCardAlreadyExist)
                            if (cardType.IsProofOfAddressRequired)
                                isProofOfAddressRequired = cardType.IsProofOfAddressRequired;
                        if (!isBlackCardAlreadyExist)
                            if (cardType.IsProofOfSourceOfFundsRequired)
                                IsProofOfSourceOfFundsRequired = cardType.IsProofOfSourceOfFundsRequired;

                        //if ((isBlackCardAlreadyExist) && !string.IsNullOrEmpty(customer.Customer
                        //        .ProofOfAddressDocInCustomerLanguage) && (!string.IsNullOrEmpty(customer.Customer.
                        //            ProofOfSourceOfFundsDocInCustomerLanguage))
                        //            && (proofOfAddressDocs != null && proofOfAddressDocs.Count > 0) &&
                        //            (proofOfSourceOfFundsDocs != null && proofOfSourceOfFundsDocs.Count > 0))
                        //    throw new UserException("Proof of Source and Address Documents already Exist");

                        if (isNewCustomer && cardRequest.CardQty > cardType.MaxQuantity)
                            throw new UserException("Card Requested Quantity cannot be greater than Card Type " + cardType.Name + " Maximum Allowed Quantity");

                        if (!isNewCustomer && customerCardRequests != null && customerCardRequests.Count > 0)
                        {
                            var cardRequestsByCardType = customerCardRequests
                                .Where(cr => cr.CardTypeId == cardRequest.CardTypeId).ToList();

                            if (cardRequestsByCardType != null && cardRequestsByCardType.Count > 0
                                && (cardRequest.CardQty +
                                cardRequestsByCardType.Sum(cr => cr.CardQty) > cardType.MaxQuantity))
                                throw new UserException
                                    ("Card Requested Quantity cannot be greater than Card Type (" + cardType.Name + ") Maximum Allowed Quantity");
                        }

                        cardRequest.Discount = cardType.Fee - cardRequest.Fee;

                        for (int i = 0; i < cardRequest.CardQty; i++)
                        {
                            cardRequestsToAdd.Add(new CardRequest()
                            {
                                CardQty = 1,
                                CardTypeId = cardRequest.CardTypeId,
                                CreatedOn = cardRequest.CreatedOn,
                                Description = cardRequest.Description,
                                Fee = cardRequest.Fee,
                                Discount = cardRequest.Discount
                            });
                        }
                    }

                    if (isProofOfAddressRequired)
                    {
                        customer.Customer.ProofOfAddressDocType = proofOfAddressDocType;
                        customer.Customer.ProofOfAddressDocInCustomerLanguage =
                            string.Join(";", proofOfAddressDocs.ToArray());
                    }

                    if (IsProofOfSourceOfFundsRequired)
                    {
                        customer.Customer.ProofOfSourceOfFundsDocType = proofOfSourceOfFundsDocType;
                        customer.Customer.ProofOfSourceOfFundsDocInCustomerLanguage =
                            string.Join(";", proofOfSourceOfFundsDocs.ToArray());
                    }

                    context.RequestForms.Add(requestForm);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    foreach (CardRequest cardRequest in cardRequestsToAdd)
                    {
                        cardRequest.CardRequestFormId = requestForm.Id;
                        context.CardRequests.Add(cardRequest);
                    }

                    foreach (Payment payment in payments)
                    {
                        payment.RequestFormId = requestForm.Id;
                        context.Payments.Add(payment);
                    }

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
                                                , performedForUserId: customer.User.Id);

                    customer.Customer.User = customer.User;
                    new MessageService().SendCustomerNewCardRequestMessage(customer.Customer,
                        requestForm, cardRequests, new SettingService().CustomerDefaultLanguage.Id);

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
        /// Return Card Request Form including all container objects e.g Card Requests
        /// </summary>
        public CardRequestForm GetCardRequestForm(string applicationNumber)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from rf in context.RequestForms
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId
                                  join cr in context.CardRequests on rf.Id equals cr.CardRequestFormId
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  join c in context.Cards on cr.Id equals c.CardRequestId into cards
                                  from c in cards.DefaultIfEmpty()
                                  where rf.CreatedOn.ToString("yyyyMMddHHmmss") + rf.Id == applicationNumber
                                  && rf.TypeId == (int)RequestFormType.Card
                                  select new
                                  {
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
                        CardRequestForm cardRequestForm = new CardRequestForm(firstRecord.RequestForm);
                        cardRequestForm.Customer = firstRecord.Customer;
                        cardRequestForm.CardRequests = new List<CardRequest>();
                        cardRequestForm.Payments = new List<Payment>();

                        foreach (var requestForm in result.DistinctBy(r => r.CardRequest))
                        {
                            requestForm.CardRequest.CardType = requestForm.CardType;
                            requestForm.CardRequest.Card = requestForm.Card;
                            cardRequestForm.CardRequests.Add(requestForm.CardRequest);
                        }

                        foreach (var requestForm in result.DistinctBy(r => r.Payment))
                        {
                            cardRequestForm.Payments.Add(requestForm.Payment);
                        }

                        return cardRequestForm;
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
        /// Return Card Request Form including all container objects e.g Card Requests
        /// </summary>
        /// <param name="cardRequestFormId"></param>
        /// <returns></returns>
        public CardRequestForm GetCardRequestForm(int cardRequestFormId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from rf in context.RequestForms
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId
                                  join cr in context.CardRequests on rf.Id equals cr.CardRequestFormId
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  join c in context.Cards on cr.Id equals c.CardRequestId into cards
                                  from c in cards.DefaultIfEmpty()
                                  where rf.Id == cardRequestFormId
                                  && rf.TypeId == (int)RequestFormType.Card
                                  select new
                                  {
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
                        CardRequestForm cardRequestForm = new CardRequestForm(firstRecord.RequestForm);
                        cardRequestForm.Customer = firstRecord.Customer;
                        cardRequestForm.CardRequests = new List<CardRequest>();
                        cardRequestForm.Payments = new List<Payment>();

                        foreach (var requestForm in result.DistinctBy(r => r.CardRequest))
                        {
                            requestForm.CardRequest.CardType = requestForm.CardType;
                            requestForm.CardRequest.Card = requestForm.Card;
                            cardRequestForm.CardRequests.Add(requestForm.CardRequest);
                        }

                        foreach (var requestForm in result.DistinctBy(r => r.Payment))
                        {
                            cardRequestForm.Payments.Add(requestForm.Payment);
                        }

                        return cardRequestForm;
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



        public List<CardRequestForm> GetCardRequestForms(out int totalCount, int? customerId = null,
            string nic = null, string passportNo = null, RequestFormStatus? requestFormStatus = null
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
                                  join cr in context.CardRequests on rf.Id equals cr.CardRequestFormId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
                                  from p in payments
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  join c in context.Cards on cr.Id equals c.CardRequestId into cards
                                  from c in cards.DefaultIfEmpty()
                                  where true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase)
                                  : true)
                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && true == (requestFormStatus.HasValue ?
                                  (requestFormStatus.Value == RequestFormStatus.PaymentConfirmed &&
                                  payments.All(pay => pay.ConfirmedOn != null
                                  && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                                  || (requestFormStatus.Value == RequestFormStatus.PaymentConfirmationFailed
                                  && payments.Any(pay => pay.ConfirmedOn != null
                                  && !string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                                  || (requestFormStatus.Value == RequestFormStatus.Pending
                                  && payments.Any(pay => pay.ConfirmedOn == null
                                  && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                                  || (requestFormStatus.Value == RequestFormStatus.SentToCardIssuer
                                  && rf.SentToCardIssuerOn != null)
                                  : true)
                                  && rf.TypeId == (int)RequestFormType.Card
                                  orderby rf.CreatedOn descending
                                  group
                                  new
                                  {
                                      Card = c,
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      Payment = p
                                  }
                                  by rf.Id).OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);
                    totalCount = result.Count();
                    var customerCards = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (customerCards != null && customerCards.Count() > 0)
                    {
                        List<CardRequestForm> cardRequestForms = new List<CardRequestForm>();

                        foreach (var grp in customerCards)
                        {
                            var firstRecord = grp.First();
                            CardRequestForm cardRequestForm = new CardRequestForm(firstRecord.RequestForm);
                            cardRequestForm.Customer = firstRecord.Customer;
                            cardRequestForm.Customer.User = firstRecord.User;
                            cardRequestForm.Payments = new List<Payment>();
                            cardRequestForm.CardRequests = new List<CardRequest>();

                            foreach (var requestForm in grp.DistinctBy(r => r.Payment))
                            {
                                cardRequestForm.Payments.Add(requestForm.Payment);
                            }

                            foreach (var requestForm in grp.DistinctBy(r => r.CardRequest))
                            {
                                requestForm.CardRequest.CardType = requestForm.CardType;
                                requestForm.CardRequest.Card = requestForm.Card;
                                requestForm.CardRequest.CardRequestForm = cardRequestForm;
                                requestForm.CardRequest.StatusString =
                                    GetCardRequestStatusString(requestForm.CardRequest, isForCustomer);
                                cardRequestForm.CardRequests.Add(requestForm.CardRequest);
                            }

                            cardRequestForms.Add(cardRequestForm);
                        }

                        return cardRequestForms;
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
        /// Return Card Request Forms which are pending for processing e.g Validation, Translation or Authority Signed Form
        /// </summary>
        public List<CardRequestForm> GetPendingCardRequestForms(out int totalCount, int? customerId = null
            , string nic = null, string passportNo = null, DateTime? createdDateFrom = null
            , DateTime? createdDateTo = null, bool isForCustomer = false, int pageIndex = 0, int? pageSize = null
            , OrderBy orderBy = OrderBy.Asc)
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
                                  join cr in context.CardRequests on rf.Id equals cr.CardRequestFormId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
                                  from p in payments
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  join c in context.Cards on cr.Id equals c.CardRequestId into cards
                                  from c in cards.DefaultIfEmpty()
                                  where
                                  (customer.ValidatedOn == null
                                  || !string.IsNullOrEmpty(customer.ValidationFailureReason)
                                     || (string.IsNullOrEmpty(customer.CustomerSignedForm)
                                     || string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
                                     || (!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
                                     //&& string.IsNullOrEmpty(customer.ProofOfAddressDoc)
                                     )
                                     || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
                                     //&& string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)
                                     )
                                     )
                                  //|| (string.IsNullOrEmpty(customer.AuthoritySignedForm)
                                  //|| string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage))
                                  //|| (customer.SentToCardIssuerOn == null))
                                  && true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)

                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && rf.TypeId == (int)RequestFormType.Card
                                  orderby rf.CreatedOn descending
                                  group
                                  new
                                  {
                                      Card = c,
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      Payment = p
                                  }
                                  by rf.Id).OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);
                    totalCount = result.Count();
                    var customerCards = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (customerCards != null && customerCards.Count() > 0)
                    {
                        List<CardRequestForm> cardRequestForms = new List<CardRequestForm>();

                        foreach (var grp in customerCards)
                        {
                            var firstRecord = grp.First();
                            CardRequestForm cardRequestForm = new CardRequestForm(firstRecord.RequestForm);
                            cardRequestForm.Customer = firstRecord.Customer;
                            cardRequestForm.Customer.User = firstRecord.User;
                            cardRequestForm.Payments = new List<Payment>();
                            cardRequestForm.CardRequests = new List<CardRequest>();

                            foreach (var requestForm in grp.DistinctBy(r => r.CardRequest))
                            {
                                requestForm.CardRequest.CardType = requestForm.CardType;
                                requestForm.CardRequest.Card = requestForm.Card;
                                requestForm.CardRequest.StatusString =
                                    GetCardRequestStatusString(requestForm.CardRequest, isForCustomer);
                                cardRequestForm.CardRequests.Add(requestForm.CardRequest);
                            }

                            foreach (var requestForm in grp.DistinctBy(r => r.Payment))
                            {
                                cardRequestForm.Payments.Add(requestForm.Payment);
                            }

                            cardRequestForms.Add(cardRequestForm);
                        }

                        return cardRequestForms;
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
        /// Return Card Request Forms which are either waiting for Card Issuer Response or were Declined
        /// </summary>
        public List<CardRequestForm> GetCardRequestFormsWaitingForCardIssuerResponseOrDeclined(out int totalCount,
             int? customerId = null, string nic = null, string passportNo = null, DateTime? createdDateFrom = null
             , DateTime? createdDateTo = null, bool isForCustomer = false, int pageIndex = 0, int? pageSize = null
             , OrderBy orderBy = OrderBy.Asc, bool showFailed = true)
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
                                  join cr in context.CardRequests on rf.Id equals cr.CardRequestFormId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
                                  from p in payments
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  where (cr.CardIssuerRespondedOn == null
                                  || (showFailed && !string.IsNullOrEmpty(cr.DeclinedReason)))
                                  && (customer.CardIssuerRespondedOn != null
                                  && string.IsNullOrEmpty(customer.DeclinedReason))
                                  //&& customer.SentToCardIssuerOn != null
                                  //&& !string.IsNullOrEmpty(customer.AuthoritySignedForm)
                                  //&& !string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage)
                                  && (
                                     (string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
                                     || (!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
                                     //&& !string.IsNullOrEmpty(customer.ProofOfAddressDoc))
                                     ))
                                     || (string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
                                     || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
                                     //&& !string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc))
                                     )
                                     )
                                     )
                                     &&
                                     (p.ConfirmationFailureReason == null && p.ConfirmedOn != null)
                                  && (customer.ValidatedOn != null
                                  && string.IsNullOrEmpty(customer.ValidationFailureReason))
                                  && !string.IsNullOrEmpty(customer.CustomerSignedForm)
                                  && !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage)
                                  && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
                                  && true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && rf.TypeId == (int)RequestFormType.Card
                                  group
                                  new
                                  {
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      Payment = p
                                  }
                                  by rf.Id).OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);
                    totalCount = result.Count();
                    var customerCards = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (customerCards != null && customerCards.Count() > 0)
                    {
                        List<CardRequestForm> cardRequestForms = new List<CardRequestForm>();

                        foreach (var grp in customerCards)
                        {
                            var firstRecord = grp.First();
                            CardRequestForm cardRequestForm = new CardRequestForm(firstRecord.RequestForm);
                            cardRequestForm.Customer = firstRecord.Customer;
                            cardRequestForm.Customer.User = firstRecord.User;
                            cardRequestForm.Payments = new List<Payment>();
                            cardRequestForm.CardRequests = new List<CardRequest>();

                            foreach (var requestForm in grp.DistinctBy(r => r.CardRequest))
                            {
                                requestForm.CardRequest.CardType = requestForm.CardType;
                                requestForm.CardRequest.StatusString =
                                    GetCardRequestStatusString(requestForm.CardRequest, isForCustomer);

                                cardRequestForm.CardRequests.Add(requestForm.CardRequest);

                            }

                            foreach (var requestForm in grp.DistinctBy(r => r.Payment))
                            {
                                cardRequestForm.Payments.Add(requestForm.Payment);
                            }

                            cardRequestForms.Add(cardRequestForm);
                        }

                        return cardRequestForms;
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
        /// Return Card Request Forms which are ready to deliver to customer
        /// </summary>
        //public List<CardRequestForm> GetCardRequestsPendingToDeliverToCustomer(out int totalCount
        //    , int? customerId = null, string nic = null, string passportNo = null, DateTime? createdDateFrom = null
        //    , DateTime? createdDateTo = null, bool isForCustomer = false, int pageIndex = 0, int? pageSize = null
        //    , OrderBy orderBy = OrderBy.Asc)
        //{
        //    totalCount = 0;
        //    try
        //    {
        //        if (createdDateTo.HasValue)
        //            createdDateTo = createdDateTo.Value.AddDays(1);

        //        if (!pageSize.HasValue)
        //            pageSize = new SettingService().ResultPageSize;

        //        int skipRows = pageSize.Value * pageIndex;

        //        using (var context = new EntityContext())
        //        {
        //            var result = (from rf in context.RequestForms
        //                          join customer in context.Customers on rf.CustomerId equals customer.UserId
        //                          join user in context.Users on customer.UserId equals user.Id
        //                          join cr in context.CardRequests on rf.Id equals cr.CardRequestFormId
        //                          join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
        //                          from p in payments
        //                          join ct in context.CardTypes on cr.CardTypeId equals ct.Id
        //                          join c in context.Cards on cr.Id equals c.CardRequestId
        //                          where cr.DispatchedToCustomerOn == null
        //                          && cr.DispatchedToTBOOn != null
        //                          && cr.CardIssuerRespondedOn != null && string.IsNullOrEmpty(cr.DeclinedReason)
        //                          && (customer.CardIssuerRespondedOn != null
        //                          && string.IsNullOrEmpty(customer.DeclinedReason))
        //                          //&& customer.SentToCardIssuerOn != null
        //                          //&& !string.IsNullOrEmpty(customer.AuthoritySignedForm)
        //                          //&& !string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage)
        //                          && (
        //                             (string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                             || (!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                             //&& !string.IsNullOrEmpty(customer.ProofOfAddressDoc)
        //                             )
        //                             )
        //                             || (string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                             || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                             //&& !string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)
        //                             )
        //                             )
        //                             )
        //                          && (customer.ValidatedOn != null
        //                          && string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedForm)
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage)
        //                          && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
        //                          && true == (customerId.HasValue ? customer.UserId == customerId :
        //                          !string.IsNullOrEmpty(nic)
        //                          ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
        //                          !string.IsNullOrEmpty(passportNo)
        //                          ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
        //                          && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
        //                          && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
        //                          && rf.TypeId == (int)RequestFormType.Card
        //                          group
        //                          new
        //                          {
        //                              Card = c,
        //                              CardRequest = cr,
        //                              CardType = ct,
        //                              RequestForm = rf,
        //                              Customer = customer,
        //                              User = user,
        //                              Payment = p
        //                          }
        //                          by rf.Id).OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);
        //            totalCount = result.Count();
        //            var customerCards = result.Skip(skipRows).Take(pageSize.Value).ToList();

        //            if (customerCards != null && customerCards.Count() > 0)
        //            {
        //                List<CardRequestForm> cardRequestForms = new List<CardRequestForm>();

        //                foreach (var grp in customerCards)
        //                {
        //                    var firstRecord = grp.First();
        //                    CardRequestForm cardRequestForm = new CardRequestForm(firstRecord.RequestForm);
        //                    cardRequestForm.Customer = firstRecord.Customer;
        //                    cardRequestForm.Customer.User = firstRecord.User;
        //                    cardRequestForm.Payments = new List<Payment>();
        //                    cardRequestForm.CardRequests = new List<CardRequest>();

        //                    foreach (var requestForm in grp.DistinctBy(r => r.CardRequest))
        //                    {
        //                        requestForm.CardRequest.CardType = requestForm.CardType;
        //                        requestForm.CardRequest.Card = requestForm.Card;
        //                        requestForm.CardRequest.StatusString =
        //                            GetCardRequestStatusString(requestForm.CardRequest, isForCustomer);
        //                        cardRequestForm.CardRequests.Add(requestForm.CardRequest);
        //                    }

        //                    foreach (var requestForm in grp.DistinctBy(r => r.Payment))
        //                    {
        //                        cardRequestForm.Payments.Add(requestForm.Payment);
        //                    }

        //                    cardRequestForms.Add(cardRequestForm);
        //                }

        //                return cardRequestForms;
        //            }

        //            return null;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        log.Error(exception);
        //        return null;
        //    }
        //}

        public RequestFormStatus GetRequestFormStatus(RequestForm requestForm)
        {
            try
            {
                if (requestForm.SentToCardIssuerOn != null)
                    return RequestFormStatus.SentToCardIssuer;
                else if (requestForm.Payments.All(p => p.ConfirmedOn != null
                && string.IsNullOrEmpty(p.ConfirmationFailureReason)))
                    return RequestFormStatus.PaymentConfirmed;
                else if (requestForm.Payments.Any(p => p.ConfirmedOn != null
                && !string.IsNullOrEmpty(p.ConfirmationFailureReason)))
                    return RequestFormStatus.PaymentConfirmationFailed;
                else
                    return RequestFormStatus.Pending;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return 0;
            }
        }

        /// <summary>
        /// Return Card Request Form Status To Show To Customer
        /// </summary>
        public RequestFormStatusForCustomer GetRequestFormStatusForCustomer(RequestForm requestForm)
        {
            try
            {
                if (requestForm.SentToCardIssuerOn != null
                    //|| !string.IsNullOrEmpty(requestForm.Customer.AuthoritySignedForm)
                    || requestForm.Payments.TrueForAll(p => p.ConfirmedOn != null
                    && string.IsNullOrEmpty(p.ConfirmationFailureReason))
                    || requestForm.Payments.TrueForAll(p => p.PaymentMethod == PaymentMethod.RAHYAB
                    || p.PaymentMethod == PaymentMethod.Admin))
                    return RequestFormStatusForCustomer.Processing;
                else if (requestForm.Payments.Any(p => p.ConfirmedOn != null
                && string.IsNullOrEmpty(p.ConfirmationFailureReason)))
                    return RequestFormStatusForCustomer.PaymentConfirmationFailed;
                else
                    return RequestFormStatusForCustomer.WaitingForPaymentConfirmation;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return 0;
            }
        }

        /// <summary>
        /// Return Card Request Form Status String
        /// </summary>
        public string GetRequestFormStatusString(RequestForm requestForm, bool isForCustomer = false)
        {
            try
            {
                if (isForCustomer)
                {
                    if (requestForm.SentToCardIssuerOn != null
                        //|| !string.IsNullOrEmpty(requestForm.Customer.AuthoritySignedForm)
                        || requestForm.Payments.All(pay => pay.ConfirmedOn != null && string.IsNullOrEmpty(pay.ConfirmationFailureReason))
                        || requestForm.Payments.All(p => p.PaymentMethod == PaymentMethod.RAHYAB
                        || p.PaymentMethod == PaymentMethod.Admin))
                        return "Processing";
                    else if (requestForm.Payments.Any(pay => pay.ConfirmedOn != null
                    && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Confirmation Failed";
                    else
                        return "Waiting For Payment Confirmation";
                }
                else
                {
                    if (requestForm.SentToCardIssuerOn != null)
                        return "Sent To Card Issuer";
                    //else if (!string.IsNullOrEmpty(requestForm.Customer.AuthoritySignedForm))
                    //    return "Waiting For Signature";
                    else if (requestForm.Payments.All(pay => pay.ConfirmedOn != null
                    && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Confirmed";
                    else if (requestForm.Payments.Any(pay => pay.ConfirmedOn != null
                    && string.IsNullOrEmpty(pay.ConfirmationFailureReason)))
                        return "Payment Confirmation Failed";
                    else
                        return "Pending";
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
        /// Will use for Dashboard
        /// </summary>
        /// <param name="cardRequestStatus"></param>
        /// <returns></returns>
        public int GetCardRequestFormsCountByStatus(RequestFormStatus requestFormStatus)
        {
            int totalCount = 0;
            try
            {
                return GetCardRequestForms(out totalCount, requestFormStatus: requestFormStatus).Count();
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }

        }

        /// <summary>
        /// For new Customer whose Card Request is pending
        /// </summary>
        public bool IsCustomerCardRequestFormExist(int customerId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.RequestForms.Count(rf => rf.CustomerId == customerId) > 0;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Return Top Up Requests Forms which are either pending for Payment Confirmation or failed to confirm payment
        /// </summary>
        public List<CardRequestForm> GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out int totalCount,
            int? customerId = null, string nic = null, string passportNo = null, int? cardTypeId = null
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
                                  join cr in context.CardRequests on rf.Id equals cr.CardRequestFormId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
                                  from p in payments
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  where true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
                                  && ct.Id == (cardTypeId.HasValue ? cardTypeId.Value : ct.Id)
                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && payments.Any(pay => pay.ConfirmedOn == null || pay.ReConfirmedOn == null
                                  || !string.IsNullOrEmpty(pay.ConfirmationFailureReason))
                                  && rf.TypeId == (int)RequestFormType.Card
                                  orderby rf.CreatedOn descending
                                  group
                                  new
                                  {
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
                    var customerCards = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (customerCards != null && customerCards.Count() > 0)
                    {
                        List<CardRequestForm> cardRequestForms = new List<CardRequestForm>();

                        foreach (var grp in customerCards)
                        {
                            var firstRecord = grp.First();
                            CardRequestForm cardRequestForm = new CardRequestForm(firstRecord.RequestForm);
                            cardRequestForm.Customer = firstRecord.Customer;
                            cardRequestForm.Customer.User = firstRecord.User;
                            cardRequestForm.Payments = new List<Payment>();
                            cardRequestForm.CardRequests = new List<CardRequest>();

                            foreach (var requestForm in grp.DistinctBy(r => r.CardRequest))
                            {
                                requestForm.CardRequest.CardType = requestForm.CardType;
                                requestForm.CardRequest.StatusString =
                                   GetCardRequestStatusString(requestForm.CardRequest, false);
                                cardRequestForm.CardRequests.Add(requestForm.CardRequest);
                            }

                            foreach (var requestForm in grp.DistinctBy(r => r.Payment))
                            {
                                cardRequestForm.Payments.Add(requestForm.Payment);
                            }

                            cardRequestForms.Add(cardRequestForm);
                        }

                        return cardRequestForms;
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

        #region Card Request

        public int AddCardRequest(CardRequest cardRequest)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    context.CardRequests.Add(cardRequest);
                    context.SaveChanges();

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return cardRequest.Id;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        /// <summary>
        /// Will use for Dashboard
        /// 
        /// </summary>
        /// <param name="cardRequestStatus"></param>
        /// <returns></returns>
        public int GetCardRequestsCountByStatus(CardRequestStatus cardRequestStatus)
        {
            try
            {
                return GetCardRequests(cardRequestStatus: cardRequestStatus).Count();
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
            }
            return -1;
        }

        public CardRequest GetCardRequest(int cardRequestId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var cardRequest = (from cr in context.CardRequests
                                       join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                       join rf in context.RequestForms on cr.CardRequestFormId equals rf.Id
                                       join c in context.Cards on cr.Id equals c.CardRequestId into cards
                                       from c in cards.DefaultIfEmpty()
                                       where cr.Id == cardRequestId
                                       && rf.TypeId == (int)RequestFormType.Card
                                       select new { Card = c, CardRequest = cr, CardType = ct, RequestForm = rf })
                                       .FirstOrDefault();

                    if (cardRequest != null)
                    {
                        cardRequest.CardRequest.CardType = cardRequest.CardType;
                        cardRequest.CardRequest.CardRequestForm = cardRequest.RequestForm;
                        cardRequest.CardRequest.Card = cardRequest.Card;
                        return cardRequest.CardRequest;
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

        public List<CardRequest> GetCardRequests(int? customerId = null, string nic = null, string passportNo = null
            , int? cardRequestFormId = null, CardRequestStatus? cardRequestStatus = null, int? cardTypeId = null
            , DateTime? createdDateFrom = null, DateTime? createdDateTo = null
            , int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        {
            try
            {
                if (createdDateTo.HasValue)
                    createdDateTo = createdDateTo.Value.AddDays(1);

                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                int skipRows = pageSize.Value * pageIndex;

                using (var context = new EntityContext())
                {
                    var cardRequests = (from cr in context.CardRequests
                                        join c in context.Cards on cr.Id equals c.CardRequestId into cards
                                        from c in cards.DefaultIfEmpty()
                                        join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                        join rf in context.RequestForms on cr.CardRequestFormId equals rf.Id
                                        join customer in context.Customers on rf.CustomerId equals customer.UserId
                                        where true == (customerId.HasValue ? customer.UserId == customerId :
                                        !string.IsNullOrEmpty(nic)
                                        ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                        !string.IsNullOrEmpty(passportNo)
                                        ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
                                        && rf.Id == (cardRequestFormId.HasValue ? cardRequestFormId.Value : rf.Id)
                                        && ct.Id == (cardTypeId.HasValue ? cardTypeId.Value : ct.Id)
                                        && rf.CreatedOn >= (createdDateFrom.HasValue
                                        ? createdDateFrom.Value : rf.CreatedOn)
                                        && rf.CreatedOn <= (createdDateTo.HasValue
                                        ? createdDateTo.Value : rf.CreatedOn)
                                        &&
                                        (
                                            //(
                                            //true == (cardRequestStatus.HasValue
                                            //&& cardRequestStatus.Value == CardRequestStatus.PackageDeliveredToCustomer)
                                            //? cr.DeliveredToCustomerOn != null : true
                                            //)
                                            //||
                                            //(
                                            //true == (cardRequestStatus.HasValue
                                            //&& cardRequestStatus.Value ==
                                            //CardRequestStatus.WaitingToDeliverPackageToCustomer)
                                            //? cr.DispatchedToCustomerOn != null && cr.DeliveredToCustomerOn == null
                                            //: true
                                            //)
                                            //||
                                            //(
                                            //true == (cardRequestStatus.HasValue
                                            //&& cardRequestStatus.Value == CardRequestStatus.DispatchPackageToTBO)
                                            //? cr.DispatchedToTBOOn != null && cr.DeliveredToCustomerOn == null
                                            //&& cr.DispatchedToCustomerOn == null : true
                                            //)
                                            //||
                                            (
                                            true == (cardRequestStatus.HasValue
                                            && cardRequestStatus.Value == CardRequestStatus.ApprovedByCardIssuer)
                                            ? cr.CardIssuerRespondedOn != null && string.IsNullOrEmpty(cr.DeclinedReason)
                                            //&& cr.DeliveredToCustomerOn == null && cr.DispatchedToCustomerOn == null
                                            && cr.DispatchedToTBOOn == null : true
                                            )
                                            ||
                                            (
                                            true == (cardRequestStatus.HasValue
                                            && cardRequestStatus.Value == CardRequestStatus.RejectedByCardIssuer)
                                            ? cr.CardIssuerRespondedOn != null && !string.IsNullOrEmpty(cr.DeclinedReason)
                                            //&& cr.DeliveredToCustomerOn == null && cr.DispatchedToCustomerOn == null
                                            && cr.DispatchedToTBOOn == null : true
                                            )
                                        //||
                                        //(
                                        //true == (cardRequestStatus.HasValue
                                        //&& cardRequestStatus.Value == CardRequestStatus.Pending)
                                        //? cr.CardIssuerRespondedOn == null && cr.DeliveredToCustomerOn == null
                                        //&& cr.DispatchedToCustomerOn == null && cr.DispatchedToTBOOn == null
                                        //&& cr.CardIssuerRespondedOn == null : true
                                        //)
                                        )
                                        && rf.TypeId == (int)RequestFormType.Card
                                        orderby (orderBy == OrderBy.Asc) ? rf.CreatedOn : rf.CreatedOn descending
                                        select new { Customer = customer, Card = c, RequestForm = rf, CardRequest = cr, CardType = ct })
                                        .OrderByDescending(r => r.RequestForm.CreatedOn)
                                        .ToList();

                    if (cardRequests != null && cardRequests.Count > 0)
                    {
                        foreach (var cardRequest in cardRequests)
                        {
                            cardRequest.CardRequest.CardRequestForm = cardRequest.RequestForm;
                            cardRequest.CardRequest.CardRequestForm.Customer = cardRequest.Customer;
                            cardRequest.CardRequest.Card = cardRequest.Card;
                            cardRequest.CardRequest.CardType = cardRequest.CardType;
                        }
                    }
                    return cardRequests.Select(cr => cr.CardRequest).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public CardRequestStatus GetCardRequestStatus(CardRequest cardRequest)
        {
            try
            {
                //if (cardRequest.DeliveredToCustomerOn != null)
                //    return CardRequestStatus.PackageDeliveredToCustomer;
                //else if (cardRequest.DispatchedToCustomerOn != null)
                //    return CardRequestStatus.WaitingToDeliverPackageToCustomer;
                if (cardRequest.DispatchedToTBOOn != null)
                    return CardRequestStatus.DispatchPackageToTBO;
                else if (cardRequest.CardIssuerRespondedOn != null
                    && string.IsNullOrEmpty(cardRequest.DeclinedReason))
                    return CardRequestStatus.ApprovedByCardIssuer;
                else if (cardRequest.CardIssuerRespondedOn != null
                    && !string.IsNullOrEmpty(cardRequest.DeclinedReason))
                    return CardRequestStatus.RejectedByCardIssuer;
                else
                    return CardRequestStatus.Pending;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return 0;
            }
        }

        /// <summary>
        /// For customer screen
        /// To search card requests by customer on base of status
        /// </summary>
        /// <param name="cardRequest"></param>
        /// <returns></returns>
        public CardRequestStatusForCustomer GetCardRequestStatusForCustomer(CardRequest cardRequest)
        {
            try
            {
                //if (cardRequest.DeliveredToCustomerOn != null)
                //    return CardRequestStatusForCustomer.Delivered;
                //else if (cardRequest.DispatchedToCustomerOn != null)
                //    return CardRequestStatusForCustomer.Delivered;
                if (cardRequest.DispatchedToTBOOn != null)
                    return CardRequestStatusForCustomer.Processing;
                else if (cardRequest.CardIssuerRespondedOn != null
                    && string.IsNullOrEmpty(cardRequest.DeclinedReason))
                    return CardRequestStatusForCustomer.Processing;
                else if (cardRequest.CardIssuerRespondedOn != null
                    && !string.IsNullOrEmpty(cardRequest.DeclinedReason))
                    return CardRequestStatusForCustomer.Failed;
                else
                    return CardRequestStatusForCustomer.Processing;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return 0;
            }

        }

        /// <summary>
        /// For BO screen
        /// </summary>
        /// <param name="cardRequest"></param>
        /// <param name="isForCustomer"></param>
        /// <returns></returns>
        public string GetCardRequestStatusString(CardRequest cardRequest, bool isForCustomer = false)
        {
            if (cardRequest == null || cardRequest.CardRequestFormId <= 0)
                throw new ArgumentException("Card Request or Form Id is invalid");
            try
            {
                var customerService = new CustomerService();

                if (cardRequest.CardRequestForm == null)
                    cardRequest.CardRequestForm = GetCardRequestForm(cardRequest.CardRequestFormId);

                if (cardRequest.CardRequestForm == null)
                    throw new UserException("Card Request not found");

                if (cardRequest.CardRequestForm.Customer == null)
                    cardRequest.CardRequestForm.Customer =
                            customerService.GetCustomerByRequestFormId(cardRequest.CardRequestForm.Id);

                if (cardRequest.CardRequestForm.Customer == null)
                    throw new UserException("Customer not found");

                if (cardRequest.CardRequestForm.Customer.CardRequestForms == null
                    || cardRequest.CardRequestForm.Customer.CardRequestForms.Count == 0)
                {
                    cardRequest.CardRequestForm.Customer.CardRequestForms = new List<CardRequestForm>();
                    cardRequest.CardRequestForm.Customer.CardRequestForms.Add(
                        new CardRequestForm(cardRequest.CardRequestForm));
                }

                CustomerStatus customerStatus = customerService.GetCustomerStatus(cardRequest.CardRequestForm.Customer);
                RequestFormStatus requestFormStatus = GetRequestFormStatus(cardRequest.CardRequestForm);

                if (customerStatus != CustomerStatus.Approved)
                {
                    if (string.IsNullOrEmpty(cardRequest.CardRequestForm.Customer.StatusString))
                        customerService.GetCustomerStatusString(cardRequest.CardRequestForm.Customer, isForCustomer);

                    return cardRequest.CardRequestForm.Customer.StatusString;
                }

                if (isForCustomer)
                {
                    if (cardRequest.DeliveredToCustomerOn != null)
                        return "Approved and Delivered";
                    //else if (cardRequest.DispatchedToCustomerOn != null)
                    //    return "Dispatched To Customer";
                    if (cardRequest.DispatchedToTBOOn != null)
                        return "Ready To Pick";
                    else if (cardRequest.CardIssuerRespondedOn != null
                        && string.IsNullOrEmpty(cardRequest.DeclinedReason))
                        return "Processing";
                    else if (cardRequest.CardIssuerRespondedOn != null
                        && !string.IsNullOrEmpty(cardRequest.DeclinedReason))
                        return "Rejected";
                    else
                        return "Processing";
                }
                else
                {
                    //if (cardRequest.DeliveredToCustomerOn != null)
                    //    return "Delivered";
                    //else if (cardRequest.DispatchedToCustomerOn != null)
                    //    return "Dispatched To Customer";
                    if (cardRequest.DeliveredToCustomerOn != null)
                        return "Approved and Delivered";
                    else if (cardRequest.CardIssuerRespondedOn != null
                        && string.IsNullOrEmpty(cardRequest.DeclinedReason))
                        return "Waiting To Receive From Card Issuer";
                    else if (cardRequest.CardIssuerRespondedOn != null
                        && !string.IsNullOrEmpty(cardRequest.DeclinedReason))
                        return "Rejected";
                    else
                        return "Pending";
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

        #endregion Card Request

        #region Card

        public int AddCard(Card card)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    context.Cards.Add(card);
                    context.SaveChanges();

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return card.CardRequestId;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        /// <summary>
        /// Set Card Request Approved on base of Card Issuer response
        /// </summary>
        public int SetCardRequestApproved(int cardRequestFormId, int cardRequestId, string cardNumber, string cardTitle
            , string cardExpiry, string cardServiceProviderCardId, string cardServiceProviderClientId)
        {
            if (cardRequestFormId <= 0)
                throw new ArgumentNullException("card Request Form Id is invalid");

            if (cardRequestId <= 0)
                throw new ArgumentNullException("card Request Id is invalid");

            if (string.IsNullOrEmpty(cardNumber))
                throw new ArgumentNullException("Card Number is required");

            if (string.IsNullOrEmpty(cardTitle))
                throw new ArgumentNullException("Card Title is required");

            if (string.IsNullOrEmpty(cardExpiry))
                throw new ArgumentNullException("Card Expiry is required");

            if (string.IsNullOrEmpty(cardServiceProviderCardId))
                throw new ArgumentNullException("Card Service Provider Card Id is required");

            if (IsCardExist(cardNumber, cardServiceProviderCardId))
                throw new ArgumentNullException("CardId or Card Number already exists.");

            try
            {
                using (var context = new EntityContext())
                {
                    var cardRequest = context.CardRequests.SingleOrDefault(cr => cr.Id == cardRequestId);

                    if (cardRequest == null)
                        throw new UserException("Card Request not found");

                    cardRequest.CardIssuerRespondedOn = DateTime.Now;
                    cardRequest.DispatchedToTBOOn = DateTime.Now;
                    cardRequest.DeclinedReason = null;
                    cardRequest.DeliveredToCustomerOn = DateTime.Now;

                    var card = new Card()
                    {
                        CardRequestId = cardRequestId,
                        Number = cardNumber,
                        Title = cardTitle,
                        Status = (int)CardStatus.Active,
                        ExpiryDate = cardExpiry,
                        CardServiceProviderCardId = cardServiceProviderCardId,
                        CardServiceProviderClientId = cardServiceProviderClientId
                    };
                    context.Cards.Add(card);
                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    var customer = (from cust in context.Customers
                                    join user in context.Users on cust.UserId equals user.Id
                                    join rf in context.RequestForms on cust.UserId equals rf.CustomerId
                                    where rf.Id == cardRequestFormId
                                    && rf.TypeId == (int)RequestFormType.Card
                                    select new { Customer = cust, User = user }).SingleOrDefault();

                    customer.Customer.User = customer.User;

                    new MessageService().SendCustomerCardRequestCompletedMessage(customer.Customer, card
                            , new SettingService().CustomerDefaultLanguage.Id);

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
                        , performedForUserId: customer.User.Id);

                    return card.CardRequestId;
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
        /// In case of failure
        /// </summary>
        /// <param name="cardRequestFormId"></param>
        /// <param name="cardRequestId"></param>
        /// <returns></returns>
        public int SetCardRequestDeclined(int cardRequestFormId, int cardRequestId, string declinedReason)
        {
            if (cardRequestFormId <= 0)
                throw new ArgumentNullException("card Request Form Id is invalid");

            if (cardRequestId <= 0)
                throw new ArgumentNullException("card Request Id is invalid");

            if (string.IsNullOrEmpty(declinedReason))
                throw new ArgumentNullException("Declined Reason is required");

            try
            {
                using (var context = new EntityContext())
                {
                    var cardRequest = context.CardRequests.SingleOrDefault(x => x.CardRequestFormId == cardRequestFormId
                                                                            && x.Id == cardRequestId);

                    if (cardRequest == null)
                        throw new UserException("Card Request not found");

                    cardRequest.CardIssuerRespondedOn = DateTime.Now;
                    cardRequest.DeclinedReason = !string.IsNullOrEmpty(declinedReason) ? declinedReason : "Failed";

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    var customer = (from cust in context.Customers
                                    join r in context.RequestForms on cust.UserId equals r.CustomerId
                                    where r.Id == cardRequestFormId
                                    select cust).SingleOrDefault();

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    new MessageService().SendCustomerCardRequestDeclinedMessage(customer, cardRequest, declinedReason
                        , customer.LanguageId);

                    return cardRequestId;
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

        //public bool SetCardRequestDeliveredToCustomer(int cardRequestId)
        //{
        //    if (cardRequestId <= 0)
        //        throw new ArgumentNullException("card Request Id is invalid");

        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var cardRequest = context.CardRequests.SingleOrDefault(cr => cr.Id == cardRequestId);

        //            if (cardRequest == null)
        //                throw new UserException("Card Request not found");

        //            var card = context.Cards.SingleOrDefault(c => c.CardRequestId == cardRequestId);

        //            if (card == null)
        //                throw new UserException("Card not found");

        //            if (cardRequest.DeliveredToCustomerOn != null)
        //                throw new UserException("Card already Delivered!");
        //            if (cardRequest.DispatchedToCustomerOn != null)
        //                throw new UserException("Card already Dispatched!");

        //            cardRequest.DispatchedToCustomerOn = DateTime.Now;
        //            cardRequest.DeliveredToCustomerOn = DateTime.Now;

        //            card.Status = (int)CardStatus.Active;

        //            if (context.SaveChanges() <= 0)
        //                throw new UserException("Unable to save");

        //            new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name, performedForUserId
        //                                        : cardRequestId);

        //            return true;
        //        }
        //    }
        //    catch (UserException exception)
        //    {
        //        throw exception;
        //    }
        //    catch (Exception exception)
        //    {
        //        log.Error(exception);
        //        throw new UserException("Sorry, we are unable to process your request. Please try again later.");
        //    }
        //}

        public Card GetCardByRequestId(int cardRequestId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var card = (from c in context.Cards
                                join cr in context.CardRequests on c.CardRequestId equals cr.Id
                                join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                join rf in context.RequestForms on cr.CardRequestFormId equals rf.Id
                                where c.CardRequestId == cardRequestId
                                && rf.TypeId == (int)RequestFormType.Card
                                select new { Card = c, CardRequest = cr, CardType = ct, RequestForm = rf })
                                .FirstOrDefault();

                    if (card != null)
                    {
                        card.CardRequest.CardType = card.CardType;
                        card.CardRequest.CardRequestForm = card.RequestForm;
                        card.Card.CardRequest = card.CardRequest;
                        return card.Card;
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

        public Card GetCard(int cardRequestId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from c in context.Cards
                                  join cr in context.CardRequests on c.CardRequestId equals cr.Id
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  join rf in context.RequestForms on cr.CardRequestFormId equals rf.Id
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  where cr.Id == cardRequestId
                                  && rf.TypeId == (int)RequestFormType.Card
                                  select new
                                  {
                                      Card = c,
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf
                                  ,
                                      Customer = customer
                                  })
                                  .FirstOrDefault();

                    if (result != null)
                    {
                        result.CardRequest.CardType = result.CardType;
                        result.CardRequest.CardRequestForm = result.RequestForm;
                        result.CardRequest.CardRequestForm.Customer = result.Customer;
                        result.Card.CardRequest = result.CardRequest;
                        return result.Card;
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

        public Card GetCard(string cardNumber)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from c in context.Cards
                                  join cr in context.CardRequests on c.CardRequestId equals cr.Id
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  join rf in context.RequestForms on cr.CardRequestFormId equals rf.Id
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  where c.Number == cardNumber
                                  && rf.TypeId == (int)RequestFormType.Card
                                  select new
                                  {
                                      Card = c,
                                      CardRequest = cr,
                                      CardType = ct,
                                      RequestForm = rf,
                                      Customer = customer
                                  }).FirstOrDefault();

                    if (result != null)
                    {
                        result.CardRequest.CardType = result.CardType;
                        result.CardRequest.CardRequestForm = result.RequestForm;
                        result.CardRequest.CardRequestForm.Customer = result.Customer;
                        result.Card.CardRequest = result.CardRequest;
                        return result.Card;
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

        public bool IsCardExist(string cardNumber, string cardServiceProviderCardId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.Cards.Count(c => c.Number == cardNumber
                    || c.CardServiceProviderCardId == cardServiceProviderCardId) > 0;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        public bool IsCardExistWithCardNumber(string cardNumber)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (context.Cards.Count(c => c.Number == cardNumber) > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        public bool IsCardExistWithCardServiceProviderCardId(string cardId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (context.Cards.Count(c => c.CardServiceProviderCardId == cardId) > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        /// <summary>
        /// Return Customer Cards which are active, support Top Up, not reached maximum reload and amount
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<Card> GetCardsReloadable(int customerId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var cardRequests = (from c in context.Cards
                                        join cr in context.CardRequests on c.CardRequestId equals cr.Id
                                        join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                        join rf in context.RequestForms on cr.CardRequestFormId equals rf.Id
                                        join cust in context.Customers on rf.CustomerId equals cust.UserId
                                        join user in context.Users on cust.UserId equals user.Id
                                        where ct.IsReloadable == true
                                        && c.Status == (int)CardStatus.Active
                                        && cust.UserId == customerId
                                        && context.TopUpRequests.Count(tr => tr.CardId == c.CardRequestId)
                                        < ct.MaximumReload
                                        && true == (context.TopUpRequests.Any(tr => tr.CardId == c.CardRequestId) ?
                                        context.TopUpRequests.Where(tr => tr.CardId == c.CardRequestId)
                                        .Sum(tr => tr.Amount) < ct.ReloadLimit - ct.MinimumReloadAtATime : true)
                                        orderby ct.ReloadLimit descending
                                        select new
                                        {
                                            Card = c,
                                            CardRequest = cr,
                                            RequestForm = rf,
                                            Customer = cust,
                                            User = user,
                                            CardType = ct
                                        })
                                        .ToList();

                    if (cardRequests.Count > 0)
                    {
                        foreach (var cardRequest in cardRequests)
                        {
                            cardRequest.Card.CardRequest = cardRequest.CardRequest;
                            cardRequest.CardRequest.CardRequestForm = cardRequest.RequestForm;
                            cardRequest.CardRequest.CardRequestForm.Customer = cardRequest.Customer;
                            cardRequest.CardRequest.CardRequestForm.Customer.User = cardRequest.User;
                            cardRequest.CardRequest.CardType = cardRequest.CardType;
                        }
                    }
                    return cardRequests.Select(cr => cr.Card).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<Card> GetCards(out int totalCount, int? cardTypeId = null, int? customerId = null
            , string cardNumber = null, string nic = null, string email = null, string passportNo = null
            , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, CardStatus? status = null
            , bool? isReloadable = null, int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        {
            if (createdDateTo.HasValue)
                createdDateTo = createdDateTo.Value.AddDays(1);

            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from c in context.Cards
                                  join cr in context.CardRequests on c.CardRequestId equals cr.Id
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  join rf in context.RequestForms on cr.CardRequestFormId equals rf.Id
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join user in context.Users on customer.UserId equals user.Id
                                  where ct.Id == (cardTypeId.HasValue ? cardTypeId.Value : ct.Id)
                                  && ct.IsReloadable == (isReloadable.HasValue ? isReloadable.Value : ct.IsReloadable)
                                  && true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
                                  && c.Number == (!string.IsNullOrEmpty(cardNumber) ? cardNumber : c.Number)
                                  && user.Email.Contains(!string.IsNullOrEmpty(email) ? email : user.Email)
                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && rf.TypeId == (int)RequestFormType.Card
                                  orderby (orderBy == OrderBy.Asc) ? rf.CreatedOn : rf.CreatedOn descending
                                  select new
                                  {
                                      Card = c,
                                      CardRequest = cr,
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      CardType = ct
                                  })
                                        .OrderByDescending(r => r.RequestForm.CreatedOn);
                    totalCount = result.Count();
                    var cardRequests = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (cardRequests.Count > 0)
                    {
                        foreach (var cardRequest in cardRequests)
                        {
                            cardRequest.Card.CardRequest = cardRequest.CardRequest;
                            cardRequest.CardRequest.Card = cardRequest.Card;
                            cardRequest.CardRequest.CardRequestForm = cardRequest.RequestForm;
                            cardRequest.CardRequest.CardRequestForm.Customer = cardRequest.Customer;
                            cardRequest.CardRequest.CardRequestForm.Customer.User = cardRequest.User;
                            cardRequest.CardRequest.CardType = cardRequest.CardType;
                        }
                    }
                    return cardRequests.Select(cr => cr.Card).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<Card> GetCustomerWithDeliveredCards(out int totalCount, int? customerId = null, string nic = null
            , string email = null, string passportNo = null, DateTime? createdDateFrom = null
            , DateTime? createdDateTo = null, int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        {
            if (createdDateTo.HasValue)
                createdDateTo = createdDateTo.Value.AddDays(1);

            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from c in context.Cards
                                  join cr in context.CardRequests on c.CardRequestId equals cr.Id
                                  join ct in context.CardTypes on cr.CardTypeId equals ct.Id
                                  join rf in context.RequestForms on cr.CardRequestFormId equals rf.Id
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join user in context.Users on customer.UserId equals user.Id
                                  where true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
                                  && user.Email.Contains(!string.IsNullOrEmpty(email) ? email : user.Email)
                                  && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
                                  && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
                                  && cr.DeliveredToCustomerOn != null
                                  orderby (orderBy == OrderBy.Asc) ? rf.CreatedOn : rf.CreatedOn descending
                                  select new
                                  {
                                      Card = c,
                                      CardRequest = cr,
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      CardType = ct
                                  }).DistinctBy(c => c.User.Id).OrderByDescending(r => r.RequestForm.CreatedOn);
                    totalCount = result.Count();
                    var cardRequests = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (cardRequests.Count > 0)
                    {
                        foreach (var cardRequest in cardRequests)
                        {
                            cardRequest.Card.CardRequest = cardRequest.CardRequest;
                            cardRequest.CardRequest.CardRequestForm = cardRequest.RequestForm;
                            cardRequest.CardRequest.CardRequestForm.Customer = cardRequest.Customer;
                            cardRequest.CardRequest.CardRequestForm.Customer.User = cardRequest.User;
                        }
                    }
                    return cardRequests.Select(cr => cr.Card).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        #endregion

        #region Card Payment Transaction

        /// <summary>
        /// This method will get the CARD info from LAMDA web API
        /// </summary>
        /// <param name="CardNumber"></param>
        /// <returns></returns>
        public decimal GetCardBalance(string CardNumber)
        {
            throw new NotImplementedException();
        }

        public Card GetCardBalanceAndPaymentTransactions(string cardNumber
            , DateTime? dateFrom = null, DateTime? dateTo = null, int maxRows = 500)
        {
            if (string.IsNullOrEmpty(cardNumber))
                throw new ArgumentNullException("Card Number is required");

            if (!dateFrom.HasValue || (dateFrom.HasValue && dateFrom.Value > DateTime.Now))
                dateFrom = DateTime.Now.AddDays(-30);

            if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value > dateTo.Value)
            {
                var dateFromTemp = dateFrom;
                dateFrom = dateTo;
                dateTo = dateFromTemp;
            }

            if (!dateTo.HasValue)
                dateTo = DateTime.Now;

            try
            {
                var card = GetCard(cardNumber);

                if (card == null)
                    throw new UserException("Card not found");

                string startDate = "1" + dateFrom.Value.ToString("yyMMdd");
                string endDate = "1" + dateTo.Value.ToString("yyMMdd");

                if (dateFrom.HasValue)
                    startDate = "1" + dateFrom.Value.ToString("yyMMdd");

                if (dateTo.HasValue)
                    endDate = "1" + dateTo.Value.ToString("yyMMdd");

                ICardServiceProvider cardServiceProvider = new Lamda.LamdaCardServiceProvider();
                var cardInfo = cardServiceProvider.GetCardInfo(card.CardServiceProviderClientId
                    , card.CardServiceProviderCardId, card.ExpiryDate
                    , card.CardRequest.CardRequestForm.Customer.DateOfBirth.ToString("yyyyMMdd")
                    , maxRows, startDate, endDate);

                if (cardInfo == null)
                    throw new UserException("Card info not found");

                card.Balance = cardInfo.Balance;
                card.PaymentTransactions = cardInfo.PaymentTransactions.OrderByDescending(p => p.CreatedOn).ToList();

                return card;
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

        public List<CardPaymentTransaction> GetCardPaymentTransactions(string cardServiceProviderClientId, string cardId
            , string expiryDate, string dateOfBirth, DateTime? dateFrom = null, DateTime? dateTo = null
            , int maxRows = 10)
        {
            try
            {
                if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value > dateTo.Value)
                    throw new ArgumentException("From Date cannot greater than To Date");

                if (dateFrom.HasValue && dateFrom.Value > DateTime.Now)
                    dateFrom = DateTime.Now;

                string startDate = "1" + DateTime.Now.AddDays(-180).ToString("YYMMDD");
                string endDate = "1" + DateTime.Now.ToString("YYMMDD");

                if (dateFrom.HasValue)
                    startDate = "1" + dateFrom.Value.ToString("YYMMDD");

                if (dateTo.HasValue)
                    endDate = "1" + dateTo.Value.ToString("YYMMDD");

                ICardServiceProvider cardServiceProvider = new Lamda.LamdaCardServiceProvider();
                var card = cardServiceProvider.GetCardInfo(cardServiceProviderClientId, cardId, expiryDate, dateOfBirth
                    , maxRows, startDate, endDate);

                if (card != null)
                    return card.PaymentTransactions;

                return null;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public bool UpdateCardRequests(List<CardRequest> cardRequest, Payment payment, Customer customer)
        {
            if (cardRequest == null && cardRequest.Count() < 0)
                throw new ArgumentNullException("Card Request cannot be null");

            if (payment == null)
                throw new ArgumentNullException("Payment cannot be null");

            if (customer == null)
                throw new ArgumentNullException("Customer is Required");

            try
            {
                using (var context = new EntityContext())
                {
                    var cust = (from query in context.Customers
                                join user in context.Users on query.UserId equals user.Id
                                where user.Status == (int)UserStatus.Active
                                && user.Id == customer.Id
                                select new { Customer = query, User = user }).SingleOrDefault();

                    if (customer == null)
                        throw new UserException("Customer not found");

                    int cardRequestFormId = cardRequest[0].CardRequestFormId;
                    var customerCardRequests = context.CardRequests.Where(cr => cr.CardRequestFormId ==
                                                                    cardRequestFormId).Select(cr => cr).ToList();

                    if (customerCardRequests == null && customerCardRequests.Count() < 0)
                        throw new UserException("Customer Card Requests does not exists");


                    for (int i = 0; i < customerCardRequests.Count(); i++)
                    {
                        customerCardRequests[i].CardRequestFormId = cardRequest[0].CardRequestFormId;
                        customerCardRequests[i].CardQty = cardRequest[i].CardQty;
                        customerCardRequests[i].CardTypeId = cardRequest[i].CardTypeId;
                        customerCardRequests[i].Fee = cardRequest[i].Fee;

                        context.SaveChanges();
                    }

                    var payments = context.Payments.Where(p => p.RequestFormId == cardRequestFormId).Select(p => p)
                                                   .ToList();

                    foreach (var item in payments)
                    {
                        if (payment.Amount != item.Amount)
                        {
                            item.Amount = payment.Amount;

                            if (context.SaveChanges() <= 0)
                                throw new UserException("Record cannot be updated, please try again later");
                        }
                    }

                    return true;
                }
            }
            catch (UserException exception)
            {
                log.Error(exception.Message);
                return false;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception.Message);
                return false;
            }
        }

        public bool UpdateCustomerCardDetails(Card card)
        {
            if (card == null)
                throw new UserException("Card details are required");

            if (string.IsNullOrEmpty(card.Number))
                throw new ArgumentNullException("Card Number is required");

            if (string.IsNullOrEmpty(card.Title))
                throw new ArgumentNullException("Card Title is required");

            if (string.IsNullOrEmpty(card.ExpiryDate))
                throw new ArgumentNullException("Card Expiry is required");

            if (string.IsNullOrEmpty(card.CardServiceProviderCardId))
                throw new ArgumentNullException("Card Service Provider Card Id is required");

            try
            {
                using (var context = new EntityContext())
                {
                    var result = context.Cards.SingleOrDefault(crd => crd.CardRequestId == card.CardRequestId);

                    if (result != null)
                    {
                        result.CardRequestId = card.CardRequestId;
                        result.CardServiceProviderCardId = card.CardServiceProviderCardId;
                        result.CardServiceProviderClientId = card.CardServiceProviderClientId;
                        result.Number = card.Number;

                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to save, Please try again later");

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
                throw exception;
            }
        }

        #endregion Card Payment Transaction

        #region Reports

        //public void CardRequestFinancialEvaluation()
        //{
        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var result = (from cardRequest in context.CardRequests
        //                          join cardType in context.CardTypes on cardRequest.CardTypeId equals cardType.Id
        //                          select new
        //                          {
        //                              CardRequests = cardRequest
        //                          }).GroupBy(cr => cr.CardRequests.CardTypeId).ToList();

        //            var result2 = (from rf in context.RequestForms
        //                           join topUp in context.TopUpRequests on rf.Id equals topUp.TopUpRequestFormId
        //                           join cr in context.CardRequests on rf.Id equals cr.CardRequestFormId
        //                           select new
        //                           {
        //                               TopUpRequest = topUp,
        //                               CardRequest = cr,
        //                               RequestForm = rf
        //                           }).GroupBy(r => r.RequestForm.TypeId).ToList();

        //            //foreach (var group in result)
        //            //{
        //            //    var quantity = group.Sum(c => c.CardRequests.CardQty);
        //            //    var cardTypeName = GetCardType(group.First().CardRequests.CardTypeId).Name;
        //            //}

        //            foreach (var group in result2)
        //            {
        //                var quantity = group.Sum(c => c.CardRequest.CardQty);
        //                var cardTypeName = GetCardType(group.First().CardRequest.CardTypeId).Name;
        //            }

        //        }
        //    }
        //    catch (Exception exception)
        //    {

        //    }
        //}

        #endregion
    }
}

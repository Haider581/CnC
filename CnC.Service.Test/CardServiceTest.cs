using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CnC.Core;
using CnC.Core.Cards;
using System.Collections.Generic;
using CnC.Data;
using CnC.Core.Exceptions;
using System.Reflection;
using CnC.Core.Payments;
using System.Linq;

namespace CnC.Service.Test
{
    [TestClass]
    public class CardServiceTest
    {
        [TestMethod]
        public void GetCardRequestFormsTotalCount_Test()
        {
            int totalCount = 0;
            if (new CardService().GetCardRequestForms(out totalCount) == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetCardRequestForms_TestWithPassport()
        {
            int totalCount = 0;
            string passportNo = "";
            if (new CardService().GetCardRequestForms(out totalCount, passportNo: passportNo) == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetCardRequestFormsWithNic_Test()
        {
            int totalCount = 0;
            string nic = "";
            new CardService().GetCardRequestForms(out totalCount, nic: nic);
        }

        //[TestMethod]
        //public void GetCardRequestForms_Test()
        //{
        //    int totalCount = 0;
        //    string nic = "";
        //    string passportNo = "";
        //    new CardService().GetCardRequestForms(out totalCount, nic: nic, passportNo: passportNo);
        //}

        [TestMethod]
        public void GetCardRequestFormsByCustomerId_Test()
        {
            int totalCount = 0;
            if (new CardService().GetCardRequestForms(out totalCount, customerId: 1131) == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetCardRequestFormsWithDateNicPassport_Test()
        {
            int totalCount = 0;
            string passportNo = "";
            string nic = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            if (new CardService().GetCardRequestForms(out totalCount, customerId: 1131, nic: nic, passportNo: passportNo
                                        , createdDateFrom: createdDateFrom, createdDateTo: createdDateTo) == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetCardRequestFormsByParameters_Test()
        {
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            string nic = "5554757547";
            int totalCount = 0;
            RequestFormStatus? reqStatus = null;

            reqStatus = RequestFormStatus.Pending;

            if (new CardService().GetCardRequestForms(out totalCount, nic: nic, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo
                , requestFormStatus: reqStatus) == null)
                Assert.Fail();
        }

        //[TestMethod]
        //public void GetCards_Test()
        //{
        //    int totalCount = 0;
        //    if (new CardService().GetCards(out totalCount) == null)
        //        Assert.Fail();
        //}

        [TestMethod]
        public void GetCardsReloadable_Test()
        {
            if (new CardService().GetCardsReloadable(80) == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetCardBalanceAndPaymentTransactions_Test()
        {
            if (new CardService().GetCardBalanceAndPaymentTransactions("255698") == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetCards_Test()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            string cardNumber = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int customerId = 23;
            var result = new CardService().GetCards(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithCustomerId = new CardService().GetCards(out totalCount, customerId: 23);
            if (resultWithCustomerId == null)
                Assert.Fail();
            var resultWithCardNumber = new CardService().GetCards(out totalCount, cardNumber: cardNumber);
            if (resultWithCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNic = new CardService().GetCards(out totalCount, nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new CardService().GetCards(out totalCount, passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new CardService().GetCards(out totalCount, createdDateFrom: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerId = new CardService().GetCards(out totalCount, customerId: customerId, nic: nic);
            if (resultWithNicCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerId = new CardService().GetCards(out totalCount, customerId: customerId, passportNo: passportNo);
            if (resultWithPassportCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCustomerIdCardNumber = new CardService().GetCards(out totalCount, customerId: customerId, cardNumber: cardNumber);
            if (resultWithCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCardNumberPassport = new CardService().GetCards(out totalCount, passportNo: passportNo, cardNumber: cardNumber);
            if (resultWithCardNumberPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCardNumberNic = new CardService().GetCards(out totalCount, nic: nic, cardNumber: cardNumber);
            if (resultWithCardNumberNic == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassportCardNumber = new CardService().GetCards(out totalCount, nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassport = new CardService().GetCards(out totalCount, cardNumber: cardNumber, nic: nic, passportNo: passportNo);
            if (resultWithNicPassport == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerIdCardNumber = new CardService().GetCards(out totalCount, cardNumber: cardNumber, nic: nic, customerId: customerId);
            if (resultWithNicCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerIdCardNumber = new CardService().GetCards(out totalCount, cardNumber: cardNumber, passportNo: passportNo, customerId: customerId);
            if (resultWithPassportCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassCustomer = new CardService().GetCards(out totalCount, customerId: customerId, nic: nic, passportNo: passportNo);
            if (resultWithNicPassCustomer == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromNic = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new CardService().GetCards(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToNic = new CardService().GetCards(out totalCount, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithCreatedDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCustomerId = new CardService().GetCards(out totalCount, createdDateTo: createdDateTo, cardNumber: cardNumber);
            if (resultWithCreatedDateToCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCardNumber = new CardService().GetCards(out totalCount, createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCustomerId = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, customerId: customerId);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCardnumber = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, cardNumber: cardNumber);
            if (resultWithCreatedDateFromCardnumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassportNic = new CardService().GetCards(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCardNumber = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, cardNumber: cardNumber);
            if (resultWithDateFromDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerId = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithDateFromDateToCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerIdCardNumber = new CardService().GetCards(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId, cardNumber: cardNumber);
            if (resultWithDateFromDateToCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithAllParamteters = new CardService().GetCards(out totalCount, customerId: customerId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic, passportNo: passportNo, cardNumber: cardNumber);
            if (resultWithAllParamteters == null)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void GetCardRequestsFormsPendingConfirmationOrFailed()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int customerId = 23;
            var result = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithCustomerId = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: 23);
            if (resultWithCustomerId == null)
                Assert.Fail();
            var resultWithNic = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerId = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: customerId, nic: nic);
            if (resultWithNicCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerId = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: customerId, passportNo: passportNo);
            if (resultWithPassportCustomerId == null)
            {
                Assert.Fail();
            }

            var resultWithNicPassportCardNumber = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }

            var resultWithNicPassCustomer = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: customerId, nic: nic, passportNo: passportNo);
            if (resultWithNicPassCustomer == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromNic = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToNic = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithCreatedDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCardNumber = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCustomerId = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, customerId: customerId);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassportNic = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerId = new CardService().GetCardRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithDateFromDateToCustomerId == null)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void GetCardRequestForms_Test()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int customerId = 23;
            var result = new CardService().GetCardRequestForms(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithCustomerId = new CardService().GetCardRequestForms(out totalCount, customerId: 23);
            if (resultWithCustomerId == null)
                Assert.Fail();
            var resultWithNic = new CardService().GetCardRequestForms(out totalCount, nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new CardService().GetCardRequestForms(out totalCount, passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerId = new CardService().GetCardRequestForms(out totalCount, customerId: customerId, nic: nic);
            if (resultWithNicCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerId = new CardService().GetCardRequestForms(out totalCount, customerId: customerId, passportNo: passportNo);
            if (resultWithPassportCustomerId == null)
            {
                Assert.Fail();
            }

            var resultWithNicPassportCardNumber = new CardService().GetCardRequestForms(out totalCount, nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }

            var resultWithNicPassCustomer = new CardService().GetCardRequestForms(out totalCount, customerId: customerId, nic: nic, passportNo: passportNo);
            if (resultWithNicPassCustomer == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromNic = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new CardService().GetCardRequestForms(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToNic = new CardService().GetCardRequestForms(out totalCount, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithCreatedDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCardNumber = new CardService().GetCardRequestForms(out totalCount, createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCustomerId = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateFrom, customerId: customerId);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassportNic = new CardService().GetCardRequestForms(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerId = new CardService().GetCardRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithDateFromDateToCustomerId == null)
            {
                Assert.Fail();
            }
        }

        //public int AddCardRequestForm_Test(RequestForm requestForm, List<CardRequest> cardRequests, List<Payment> payments
        // , bool isNewCustomer, string proofOfAddressDocType = null, List<string> proofOfAddressDocs = null
        // , string proofOfSourceOfFundsDocType = null, List<string> proofOfSourceOfFundsDocs = null)
        //{
        //    if (requestForm == null)
        //        throw new ArgumentNullException("Request Form is required");

        //    if (requestForm.TypeId != (int)RequestFormType.Card)
        //        throw new ArgumentException("Request Form Type must be Card");

        //    if (requestForm.CustomerId <= 0)
        //        throw new ArgumentException("Customer Id is invalid");

        //    if (cardRequests == null || cardRequests.Count == 0)
        //        throw new ArgumentNullException("Card Requests are required");

        //    if (payments == null || payments.Count == 0)
        //        throw new ArgumentNullException("Payments are required");

        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var customer = (from cust in context.Customers
        //                            join user in context.Users on cust.UserId equals user.Id
        //                            where user.Status == (int)UserStatus.Active
        //                            && user.Id == requestForm.CustomerId
        //                            select new { Customer = cust, User = user }).SingleOrDefault();

        //            if (customer == null)
        //                throw new UserException("Customer not found");

        //            // If New Customer then its Card Request Form must not exist already
        //            var customerCardRequests = GetCardRequests_Test(customerId: customer.Customer.UserId);

        //            if (isNewCustomer && customerCardRequests != null && customerCardRequests.Count > 0)
        //                throw new UserException("Customer Card Requests already exist");

        //            bool isProofOfAddressRequired = false;
        //            bool IsProofOfSourceOfFundsRequired = false;

        //            List<CardRequest> cardRequestsToAdd = new List<CardRequest>();

        //            // Check whether Proof Of Address or Source Of Funds documents required
        //            foreach (var cardRequest in cardRequests)
        //            {
        //                var cardType = cardRequest.CardType != null ? cardRequest.CardType :
        //                    context.CardTypes.SingleOrDefault(ct => ct.Id == cardRequest.CardTypeId);

        //                if (cardType.IsProofOfAddressRequired
        //                    && (isNewCustomer || string.IsNullOrEmpty(customer.Customer.ProofOfAddressDocInCustomerLanguage)))
        //                {
        //                    if (string.IsNullOrEmpty(proofOfAddressDocType)
        //                    || proofOfAddressDocs == null || proofOfAddressDocs.Count == 0)
        //                        throw new UserException("Proof of Address document and type are required");

        //                    if (proofOfAddressDocs.Any(d => !(d.ToLower().Contains(".jpeg") || d.ToLower().Contains(".jpg")
        //                    || d.ToLower().Contains(".png"))))
        //                        throw new UserException("Only JPEG, JPG or PNG are allowed in Proof of Address Docs");

        //                    if (proofOfAddressDocs.Any(d => (d.ToLower().Contains(".jpeg") && d.Length < 5) || d.Length < 4))
        //                        throw new ArgumentException("Proof of Address Docs length is invalid");
        //                }

        //                if (cardType.IsProofOfSourceOfFundsRequired
        //                    && (isNewCustomer || string.IsNullOrEmpty(customer.Customer.ProofOfSourceOfFundsDocInCustomerLanguage)))
        //                {
        //                    if (string.IsNullOrEmpty(proofOfSourceOfFundsDocType)
        //                    || proofOfSourceOfFundsDocs == null || proofOfSourceOfFundsDocs.Count == 0)
        //                        throw new UserException("Proof of Source of Funds documents and type are required");

        //                    if (proofOfSourceOfFundsDocs.Any(d => !(d.ToLower().Contains(".jpeg") || d.ToLower().Contains(".jpg")
        //                    || d.ToLower().Contains(".png"))))
        //                        throw new ArgumentException("Only JPEG, JPG or PNG are allowed in Proof of Source of Funds Docs");

        //                    if (proofOfSourceOfFundsDocs.Any(d => (d.ToLower().Contains(".jpeg") && d.Length < 5) || d.Length < 4))
        //                        throw new ArgumentException("Proof of Source Of Funds Docs length is invalid");
        //                }

        //                if (cardType.IsProofOfAddressRequired)
        //                    isProofOfAddressRequired = cardType.IsProofOfAddressRequired;

        //                if (cardType.IsProofOfSourceOfFundsRequired)
        //                    IsProofOfSourceOfFundsRequired = cardType.IsProofOfSourceOfFundsRequired;

        //                if (isNewCustomer && cardRequest.CardQty > cardType.MaxQuantity)
        //                    throw new UserException("Card Requested Quantity cannot be greater than Card Type Maximum Allowed Quantity");

        //                if (!isNewCustomer && customerCardRequests != null && customerCardRequests.Count > 0)
        //                {
        //                    var cardRequestsByCardType = customerCardRequests
        //                        .Where(cr => cr.CardTypeId == cardRequest.CardTypeId).ToList();

        //                    if (cardRequestsByCardType != null && cardRequestsByCardType.Count > 0
        //                        && (cardRequest.CardQty + cardRequestsByCardType.Sum(cr => cr.CardQty) > cardType.MaxQuantity))
        //                        throw new UserException
        //                            ("Card Requested Quantity cannot be greater than Card Type Maximum Allowed Quantity");
        //                }

        //                for (int i = 0; i < cardRequest.CardQty; i++)
        //                {
        //                    cardRequestsToAdd.Add(new CardRequest()
        //                    {
        //                        CardQty = 1,
        //                        CardTypeId = cardRequest.CardTypeId,
        //                        CreatedOn = cardRequest.CreatedOn,
        //                        Description = cardRequest.Description,
        //                        Fee = cardRequest.Fee
        //                    });
        //                }
        //            }

        //            if (isProofOfAddressRequired)
        //            {
        //                customer.Customer.ProofOfAddressDocType = proofOfAddressDocType;
        //                customer.Customer.ProofOfAddressDocInCustomerLanguage = string.Join(";", proofOfAddressDocs.ToArray());
        //            }

        //            if (IsProofOfSourceOfFundsRequired)
        //            {
        //                customer.Customer.ProofOfSourceOfFundsDocType = proofOfSourceOfFundsDocType;
        //                customer.Customer.ProofOfSourceOfFundsDocInCustomerLanguage = string.Join(";", proofOfSourceOfFundsDocs.ToArray());
        //            }

        //            context.RequestForms.Add(requestForm);

        //            if (context.SaveChanges() <= 0)
        //                throw new UserException("Unable to save");

        //            foreach (CardRequest cardRequest in cardRequestsToAdd)
        //            {
        //                cardRequest.CardRequestFormId = requestForm.Id;
        //                context.CardRequests.Add(cardRequest);
        //            }

        //            foreach (Payment payment in payments)
        //            {
        //                payment.RequestFormId = requestForm.Id;
        //                context.Payments.Add(payment);
        //            }

        //            if (context.SaveChanges() <= 0)
        //                throw new UserException("Unable to save");

        //            new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

        //            customer.Customer.User = customer.User;
        //            new MessageService().SendCustomerNewCardRequestMessage(customer.Customer,
        //                requestForm, cardRequests, new SettingService().CustomerDefaultLanguage.Id);

        //            return requestForm.Id;
        //        }
        //    }
        //    catch (UserException exception)
        //    {
        //        throw exception;
        //    }
        //    catch (Exception exception)
        //    {
        //        //log.Error(exception);
        //        throw new UserException("Sorry, we are unable to process your request. Please try again later.");
        //    }
        //}

        //public List<CardRequest> GetCardRequests_Test(int? customerId = null, string nic = null, string passportNo = null
        //   , int? cardRequestFormId = null, CardRequestStatus? cardRequestStatus = null, int? cardTypeId = null
        //   , DateTime? createdDateFrom = null, DateTime? createdDateTo = null
        //   , int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        //{
        //    try
        //    {
        //        if (createdDateTo.HasValue)
        //            createdDateTo = createdDateTo.Value.AddDays(1);

        //        if (!pageSize.HasValue)
        //            pageSize = new SettingService().ResultPageSize;

        //        int skipRows = pageSize.Value * pageIndex;

        //        using (var context = new EntityContext())
        //        {
        //            var cardRequests = (from cr in context.CardRequests
        //                                join c in context.Cards on cr.Id equals c.CardRequestId into cards
        //                                from c in cards.DefaultIfEmpty()
        //                                join ct in context.CardTypes on cr.CardTypeId equals ct.Id
        //                                join rf in context.RequestForms on cr.CardRequestFormId equals rf.Id
        //                                join customer in context.Customers on rf.CustomerId equals customer.UserId
        //                                where true == (customerId.HasValue ? customer.UserId == customerId :
        //                                !string.IsNullOrEmpty(nic) ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
        //                                !string.IsNullOrEmpty(passportNo) ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
        //                                && rf.Id == (cardRequestFormId.HasValue ? cardRequestFormId.Value : rf.Id)
        //                                && ct.Id == (cardTypeId.HasValue ? cardTypeId.Value : ct.Id)
        //                                && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
        //                                && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
        //                                &&
        //                                (
        //                                    (
        //                                    true == (cardRequestStatus.HasValue
        //                                    && cardRequestStatus.Value == CardRequestStatus.PackageDeliveredToCustomer)
        //                                    ? cr.DeliveredToCustomerOn != null : true
        //                                    )
        //                                    ||
        //                                    (
        //                                    true == (cardRequestStatus.HasValue
        //                                    && cardRequestStatus.Value == CardRequestStatus.WaitingToDeliverPackageToCustomer)
        //                                    ? cr.DispatchedToCustomerOn != null && cr.DeliveredToCustomerOn == null : true
        //                                    )
        //                                    ||
        //                                    (
        //                                    true == (cardRequestStatus.HasValue
        //                                    && cardRequestStatus.Value == CardRequestStatus.DispatchPackageToTBO)
        //                                    ? cr.DispatchedToTBOOn != null && cr.DeliveredToCustomerOn == null
        //                                    && cr.DispatchedToCustomerOn == null : true
        //                                    )
        //                                    ||
        //                                    (
        //                                    true == (cardRequestStatus.HasValue
        //                                    && cardRequestStatus.Value == CardRequestStatus.ApprovedByCardIssuer)
        //                                    ? cr.CardIssuerRespondedOn != null && string.IsNullOrEmpty(cr.DeclinedReason)
        //                                    && cr.DeliveredToCustomerOn == null && cr.DispatchedToCustomerOn == null
        //                                    && cr.DispatchedToTBOOn == null : true
        //                                    )
        //                                    ||
        //                                    (
        //                                    true == (cardRequestStatus.HasValue
        //                                    && cardRequestStatus.Value == CardRequestStatus.RejectedByCardIssuer)
        //                                    ? cr.CardIssuerRespondedOn != null && !string.IsNullOrEmpty(cr.DeclinedReason)
        //                                    && cr.DeliveredToCustomerOn == null && cr.DispatchedToCustomerOn == null
        //                                    && cr.DispatchedToTBOOn == null : true
        //                                    )
        //                                    ||
        //                                    (
        //                                    true == (cardRequestStatus.HasValue
        //                                    && cardRequestStatus.Value == CardRequestStatus.Pending)
        //                                    ? cr.CardIssuerRespondedOn == null && cr.DeliveredToCustomerOn == null
        //                                    && cr.DispatchedToCustomerOn == null && cr.DispatchedToTBOOn == null
        //                                    && cr.CardIssuerRespondedOn == null : true
        //                                    )
        //                                )
        //                                && rf.TypeId == (int)RequestFormType.Card
        //                                orderby (orderBy == OrderBy.Asc) ? rf.CreatedOn : rf.CreatedOn descending
        //                                select new { Customer = customer, Card = c, RequestForm = rf, CardRequest = cr, CardType = ct })
        //                                .OrderByDescending(r => r.RequestForm.CreatedOn)
        //                                .Skip(skipRows).Take(pageSize.Value).ToList();

        //            if (cardRequests != null && cardRequests.Count > 0)
        //            {
        //                foreach (var cardRequest in cardRequests)
        //                {
        //                    cardRequest.CardRequest.CardRequestForm = cardRequest.RequestForm;
        //                    cardRequest.CardRequest.CardRequestForm.Customer = cardRequest.Customer;
        //                    cardRequest.CardRequest.Card = cardRequest.Card;
        //                    cardRequest.CardRequest.CardType = cardRequest.CardType;
        //                }
        //            }
        //            return cardRequests.Select(cr => cr.CardRequest).ToList();
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        //log.Error(exception);
        //        return null;
        //    }
        //}

    }
}

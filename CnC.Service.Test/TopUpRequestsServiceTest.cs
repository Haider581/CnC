using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CnC.Service.Test
{
    [TestClass]
    public class TopUpRequestsServiceTest
    {
        public void GetTopUpRequestFormsUnderProcessing_Test()
        {
            int totalCount = 0;
            if (new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount) == null)
                Assert.Fail();
        }
        public void GetTopUpRequestFormsUnderProcessingByCustomerId_Test()
        {
            int totalCount = 0;
            if (new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, customerId: 1131) == null)
                Assert.Fail();
        }
        public void GetCardRequestFormsByParameters_Test()
        {
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int totalCount = 0;

            if (new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo) == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetTopUpRequestForms_Test()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            string cardNumber = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int customerId = 23;
            var result = new TopUpService().GetTopUpRequestForms(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithCustomerId = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: 23);
            if (resultWithCustomerId == null)
                Assert.Fail();
            var resultWithCardNumber = new TopUpService().GetTopUpRequestForms(out totalCount, cardNumber: cardNumber);
            if (resultWithCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNic = new TopUpService().GetTopUpRequestForms(out totalCount, nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new TopUpService().GetTopUpRequestForms(out totalCount, passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerId = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: customerId, nic: nic);
            if (resultWithNicCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerId = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: customerId, passportNo: passportNo);
            if (resultWithPassportCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCustomerIdCardNumber = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: customerId, cardNumber: cardNumber);
            if (resultWithCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCardNumberPassport = new TopUpService().GetTopUpRequestForms(out totalCount, passportNo: passportNo, cardNumber: cardNumber);
            if (resultWithCardNumberPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCardNumberNic = new TopUpService().GetTopUpRequestForms(out totalCount, nic: nic, cardNumber: cardNumber);
            if (resultWithCardNumberNic == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassportCardNumber = new TopUpService().GetTopUpRequestForms(out totalCount, nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassport = new TopUpService().GetTopUpRequestForms(out totalCount, cardNumber: cardNumber, nic: nic, passportNo: passportNo);
            if (resultWithNicPassport == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerIdCardNumber = new TopUpService().GetTopUpRequestForms(out totalCount, cardNumber: cardNumber, nic: nic, customerId: customerId);
            if (resultWithNicCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerIdCardNumber = new TopUpService().GetTopUpRequestForms(out totalCount, cardNumber: cardNumber, passportNo: passportNo, customerId: customerId);
            if (resultWithPassportCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassCustomer = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: customerId, nic: nic, passportNo: passportNo);
            if (resultWithNicPassCustomer == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromNic = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToNic = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithCreatedDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCustomerId = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateTo: createdDateTo, cardNumber: cardNumber);
            if (resultWithCreatedDateToCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCardNumber = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCustomerId = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, customerId: customerId);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCardnumber = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, cardNumber: cardNumber);
            if (resultWithCreatedDateFromCardnumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassportNic = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCardNumber = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, cardNumber: cardNumber);
            if (resultWithDateFromDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerId = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithDateFromDateToCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerIdCardNumber = new TopUpService().GetTopUpRequestForms(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId, cardNumber: cardNumber);
            if (resultWithDateFromDateToCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithAllParamteters = new TopUpService().GetTopUpRequestForms(out totalCount, customerId: customerId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic, passportNo: passportNo, cardNumber: cardNumber);
            if (resultWithAllParamteters == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetTopUpRequestFormsProcessing_Test()
        {
            int totalCount = 0;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int customerId = 23;
            var result = new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithCustomerId = new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, customerId: 23);
            if (resultWithCustomerId == null)
                Assert.Fail();
            var resultWithCreatedDateFrom = new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, createdDateFrom: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateToNic = new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithCreatedDateToNic == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateFromCustomerId = new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, createdDateFrom: createdDateFrom, customerId: customerId);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }

            var resultWithDateFromDateToCustomerId = new TopUpService().GetTopUpRequestFormsUnderProcessing(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithDateFromDateToCustomerId == null)
            {
                Assert.Fail();
            }

        }

        [TestMethod]
        public void PaymentConfirmation()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            string cardNumber = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int customerId = 23;
            var result = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithCustomerId = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: 23);
            if (resultWithCustomerId == null)
                Assert.Fail();
            var resultWithCardNumber = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, cardNumber: cardNumber);
            if (resultWithCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNic = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerId = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: customerId, nic: nic);
            if (resultWithNicCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerId = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: customerId, passportNo: passportNo);
            if (resultWithPassportCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCustomerIdCardNumber = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: customerId, cardNumber: cardNumber);
            if (resultWithCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCardNumberPassport = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, passportNo: passportNo, cardNumber: cardNumber);
            if (resultWithCardNumberPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCardNumberNic = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, nic: nic, cardNumber: cardNumber);
            if (resultWithCardNumberNic == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassportCardNumber = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassport = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, cardNumber: cardNumber, nic: nic, passportNo: passportNo);
            if (resultWithNicPassport == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerIdCardNumber = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, cardNumber: cardNumber, nic: nic, customerId: customerId);
            if (resultWithNicCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerIdCardNumber = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, cardNumber: cardNumber, passportNo: passportNo, customerId: customerId);
            if (resultWithPassportCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassCustomer = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: customerId, nic: nic, passportNo: passportNo);
            if (resultWithNicPassCustomer == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromNic = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToNic = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithCreatedDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCustomerId = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateTo: createdDateTo, cardNumber: cardNumber);
            if (resultWithCreatedDateToCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCardNumber = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCustomerId = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, customerId: customerId);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCardnumber = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, cardNumber: cardNumber);
            if (resultWithCreatedDateFromCardnumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassportNic = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCardNumber = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, cardNumber: cardNumber);
            if (resultWithDateFromDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerId = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithDateFromDateToCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerIdCardNumber = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId, cardNumber: cardNumber);
            if (resultWithDateFromDateToCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithAllParamteters = new TopUpService().GetTopUpRequestFormsPendingForPaymentConfirmationOrFailed(out totalCount, customerId: customerId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic, passportNo: passportNo, cardNumber: cardNumber);
            if (resultWithAllParamteters == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetTopUpRequestFormsPendingForProcessing()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            string cardNumber = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int customerId = 23;
            var result = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithCustomerId = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, customerId: 23);
            if (resultWithCustomerId == null)
                Assert.Fail();
            var resultWithCardNumber = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, cardNumber: cardNumber);
            if (resultWithCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNic = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerId = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, customerId: customerId, nic: nic);
            if (resultWithNicCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerId = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, customerId: customerId, passportNo: passportNo);
            if (resultWithPassportCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCustomerIdCardNumber = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, customerId: customerId, cardNumber: cardNumber);
            if (resultWithCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCardNumberPassport = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, passportNo: passportNo, cardNumber: cardNumber);
            if (resultWithCardNumberPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCardNumberNic = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, nic: nic, cardNumber: cardNumber);
            if (resultWithCardNumberNic == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassportCardNumber = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassport = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, cardNumber: cardNumber, nic: nic, passportNo: passportNo);
            if (resultWithNicPassport == null)
            {
                Assert.Fail();
            }
            var resultWithNicCustomerIdCardNumber = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, cardNumber: cardNumber, nic: nic, customerId: customerId);
            if (resultWithNicCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithPassportCustomerIdCardNumber = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, cardNumber: cardNumber, passportNo: passportNo, customerId: customerId);
            if (resultWithPassportCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithNicPassCustomer = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, customerId: customerId, nic: nic, passportNo: passportNo);
            if (resultWithNicPassCustomer == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromNic = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToNic = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithCreatedDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCustomerId = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateTo: createdDateTo, cardNumber: cardNumber);
            if (resultWithCreatedDateToCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToCardNumber = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCustomerId = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, customerId: customerId);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromCardnumber = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, cardNumber: cardNumber);
            if (resultWithCreatedDateFromCardnumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassportNic = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCardNumber = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, cardNumber: cardNumber);
            if (resultWithDateFromDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerId = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId);
            if (resultWithDateFromDateToCustomerId == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToCustomerIdCardNumber = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, customerId: customerId, cardNumber: cardNumber);
            if (resultWithDateFromDateToCustomerIdCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithAllParamteters = new TopUpService().GetTopUpRequestFormsPendingForProcessing(out totalCount, customerId: customerId, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic, passportNo: passportNo, cardNumber: cardNumber);
            if (resultWithAllParamteters == null)
            {
                Assert.Fail();
            }
        }

    }
}

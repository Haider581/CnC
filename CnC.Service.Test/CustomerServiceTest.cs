using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CnC.Core.Cards;
using CnC.Data;
using CnC.Core.Payments;
using CnC.Core;
using CnC.Core.Accounts;
using System.Linq;
using CnC.Core.Customers;
using CnC.Core.Exceptions;
using System.Reflection;
using System.Collections.Generic;

namespace CnC.Service.Test
{
    [TestClass]
    public class CustomerServiceTest
    {
        [TestMethod]
        public void GetCustomersPendingForCardRequests_Test()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            var result = new CustomerService().GetCustomersPendingForCardRequests(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithNic = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateFrom: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }

            var resultWithNicPassportCardNumber = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateFromNic = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateToCardNumber = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateToPassportNic = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithDateFromDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetCustomersPendingForSignedForm_Test()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            var result = new CustomerService().GetCustomersPendingForSignedForm(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithNic = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateTo: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }

            var resultWithNicPassportCardNumber = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateFromNic = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateToCardNumber = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateToPassportNic = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithDateFromDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetCustomersPendingForValidatationOrFailedTotalCount_Test()
        {
            int totalCount = 0;
            new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount);
        }

        [TestMethod]
        public void GetCustomersPendingForValidatationOrFailed_Test()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            var result = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithNic = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateTo: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }

            var resultWithNicPassportCardNumber = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateFromNic = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateToCardNumber = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateToPassportNic = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithDateFromDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
        }

        //[TestMethod]
        //public void GetCustomersPendingForTranslation_Test()
        //{
        //    int totalCount = 0;
        //    string nic = null;
        //    string passportNo = "";
        //    DateTime? createdDateFrom = null;
        //    DateTime? createdDateTo = null;

        //    var result = new CustomerService().GetCustomersPendingForTranslation(out totalCount);
        //    if (result == null)
        //        Assert.Fail();

        //    var resultWithNic = new CustomerService().GetCustomersPendingForTranslation(out totalCount, nic: nic);
        //    if (resultWithNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithPassport = new CustomerService().GetCustomersPendingForTranslation(out totalCount, passportNo: passportNo);
        //    if (resultWithPassport == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateFrom = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateFrom: createdDateFrom);
        //    if (resultWithCreatedDateFrom == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateTo = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateTo: createdDateTo);
        //    if (resultWithCreatedDateTo == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithNicPassportCardNumber = new CustomerService().GetCustomersPendingForTranslation(out totalCount, nic: nic, passportNo: passportNo);
        //    if (resultWithNicPassportCardNumber == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithCreatedDateFromNic = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
        //    if (resultWithCreatedDateFromNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateFromPassport = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
        //    if (resultWithCreatedDateFromPassport == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateToPassport = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
        //    if (resultWithCreatedDateToPassport == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithCreatedDateToCardNumber = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateTo: createdDateTo, nic: nic);
        //    if (resultWithCreatedDateToCardNumber == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateFromPassportNic = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
        //    if (resultWithCreatedDateFromPassportNic == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithCreatedDateToPassportNic = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
        //    if (resultWithCreatedDateToPassportNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithDateFromDateToPassportNic = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
        //    if (resultWithDateFromDateToPassportNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithDateFromDateToPassport = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
        //    if (resultWithDateFromDateToPassport == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithDateFromDateToNic = new CustomerService().GetCustomersPendingForTranslation(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
        //    if (resultWithDateFromDateToNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //}

        [TestMethod]
        public void GetCustomersTotalCount_Test()
        {
            int totalCount = 0;
            string nic = null;
            string passportNo = "";
            string firstName = "";
            string lastName = "";
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            var result = new CustomerService().GetCustomers(out totalCount);
            if (result == null)
                Assert.Fail();

            var resultWithNic = new CustomerService().GetCustomers(out totalCount,
                nic: nic);
            if (resultWithNic == null)
            {
                Assert.Fail();
            }
            var resultWithPassport = new CustomerService().GetCustomers(out totalCount,
                passportNo: passportNo);
            if (resultWithPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFrom = new CustomerService().GetCustomers(out totalCount,
                createdDateFrom: createdDateFrom);
            if (resultWithCreatedDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateTo = new CustomerService().GetCustomers(out totalCount,
                createdDateTo: createdDateTo);
            if (resultWithCreatedDateTo == null)
            {
                Assert.Fail();
            }

            var resultWithNicPassportCardNumber = new CustomerService().GetCustomers(out totalCount,
                nic: nic, passportNo: passportNo);
            if (resultWithNicPassportCardNumber == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateFromNic = new CustomerService().GetCustomers(out totalCount,
                createdDateFrom: createdDateFrom, nic: nic);
            if (resultWithCreatedDateFromNic == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassport = new CustomerService().GetCustomers(out totalCount,
                createdDateFrom: createdDateFrom, passportNo: passportNo);
            if (resultWithCreatedDateFromPassport == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateToPassport = new CustomerService().GetCustomers(out totalCount,
                createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithCreatedDateToPassport == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateToCardNumber = new CustomerService().GetCustomers(out totalCount,
                createdDateTo: createdDateTo, nic: nic);
            if (resultWithCreatedDateToCardNumber == null)
            {
                Assert.Fail();
            }
            var resultWithCreatedDateFromPassportNic = new CustomerService()
                .GetCustomers(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo,
                nic: nic);
            if (resultWithCreatedDateFromPassportNic == null)
            {
                Assert.Fail();
            }

            var resultWithCreatedDateToPassportNic = new CustomerService().GetCustomers(out totalCount,
                createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
            if (resultWithCreatedDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassportNic = new CustomerService().GetCustomers(out totalCount,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo,
                nic: nic);
            if (resultWithDateFromDateToPassportNic == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToPassport = new CustomerService().GetCustomers(out totalCount,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
            if (resultWithDateFromDateToPassport == null)
            {
                Assert.Fail();
            }
            var resultWithDateFromDateToNic = new CustomerService().GetCustomers(out totalCount,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
            if (resultWithDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithFirstName = new CustomerService().GetCustomers(out totalCount,
                firstName: firstName);
            if (resultWithFirstName == null)
            {
                Assert.Fail();
            }
            var resultWithFirstNameNic = new CustomerService().GetCustomers(out totalCount,
                firstName: firstName, nic: nic);
            if (resultWithFirstNameNic == null)
            {
                Assert.Fail();
            }
            var resultWithFirstNamePassport = new CustomerService().GetCustomers(out totalCount,
                firstName: firstName, passportNo: passportNo);
            if (resultWithFirstNamePassport == null)
            {
                Assert.Fail();
            }
            var resultWithFirstNameDateTo = new CustomerService().GetCustomers(out totalCount,
                firstName: firstName, createdDateTo: createdDateTo);
            if (resultWithFirstNameDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithFirstNameDateFrom = new CustomerService().GetCustomers(out totalCount,
                firstName: firstName, createdDateFrom: createdDateFrom);
            if (resultWithFirstNameDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithFirstNameDateFromDateTo = new CustomerService().GetCustomers(out totalCount,
                firstName: firstName, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);
            if (resultWithFirstNameDateFromDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithFirstNameDateFromDateToNic = new CustomerService().GetCustomers(out totalCount,
                nic: nic, firstName: firstName, createdDateFrom: createdDateFrom,
                createdDateTo: createdDateTo);
            if (resultWithFirstNameDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithFirstNameDateFromDateToNicPass = new CustomerService().
                GetCustomers(out totalCount, nic: nic, passportNo: passportNo, firstName: firstName,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);
            if (resultWithFirstNameDateFromDateToNic == null)
            {
                Assert.Fail();
            }

            var resultWithlastName = new CustomerService().GetCustomers(out totalCount,
                lastName: lastName);
            if (resultWithlastName == null)
            {
                Assert.Fail();
            }
            var resultWithlastNameNic = new CustomerService().GetCustomers(out totalCount,
                lastName: lastName, nic: nic);
            if (resultWithlastNameNic == null)
            {
                Assert.Fail();
            }
            var resultWithlastNamePassport = new CustomerService().GetCustomers(out totalCount,
                lastName: lastName, passportNo: passportNo);
            if (resultWithlastNamePassport == null)
            {
                Assert.Fail();
            }
            var resultWithlastNameDateTo = new CustomerService().GetCustomers(out totalCount,
                lastName: lastName, createdDateTo: createdDateTo);
            if (resultWithFirstNameDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithlastNameDateFrom = new CustomerService().GetCustomers(out totalCount,
                lastName: lastName, createdDateFrom: createdDateFrom);
            if (resultWithFirstNameDateFrom == null)
            {
                Assert.Fail();
            }
            var resultWithlastNameDateFromDateTo = new CustomerService().GetCustomers(out totalCount,
                lastName: lastName, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);
            if (resultWithlastNameDateFromDateTo == null)
            {
                Assert.Fail();
            }
            var resultWithlastNameDateFromDateToNic = new CustomerService().GetCustomers(out totalCount,
                nic: nic, lastName: lastName, createdDateFrom: createdDateFrom,
                createdDateTo: createdDateTo);
            if (resultWithlastNameDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithlastNameDateFromDateToNicPass = new CustomerService().
                GetCustomers(out totalCount, nic: nic, passportNo: passportNo, lastName: lastName,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);
            if (resultWithlastNameDateFromDateToNic == null)
            {
                Assert.Fail();
            }
            var resultWithAllParameters = new CustomerService().
                GetCustomers(out totalCount, nic: nic, passportNo: passportNo, firstName: firstName
                , lastName: lastName, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);
            if (resultWithAllParameters == null)
            {
                Assert.Fail();
            }
        }

        //[TestMethod]
        //public void GetCustomersPendingToSendToCardIssuerTotalCount_Test()
        //{
        //    int totalCount = 0;
        //    string nic = null;
        //    string passportNo = "";
        //    DateTime? createdDateFrom = null;
        //    DateTime? createdDateTo = null;

        //    var result = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount);
        //    if (result == null)
        //        Assert.Fail();

        //    var resultWithNic = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, nic: nic);
        //    if (resultWithNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithPassport = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, passportNo: passportNo);
        //    if (resultWithPassport == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateFrom = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateFrom: createdDateFrom);
        //    if (resultWithCreatedDateFrom == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateTo = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateTo: createdDateTo);
        //    if (resultWithCreatedDateTo == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithNicPassportCardNumber = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, nic: nic, passportNo: passportNo);
        //    if (resultWithNicPassportCardNumber == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithCreatedDateFromNic = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
        //    if (resultWithCreatedDateFromNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateFromPassport = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
        //    if (resultWithCreatedDateFromPassport == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateToPassport = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
        //    if (resultWithCreatedDateToPassport == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithCreatedDateToCardNumber = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateTo: createdDateTo, nic: nic);
        //    if (resultWithCreatedDateToCardNumber == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateFromPassportNic = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
        //    if (resultWithCreatedDateFromPassportNic == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithCreatedDateToPassportNic = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
        //    if (resultWithCreatedDateToPassportNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithDateFromDateToPassportNic = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
        //    if (resultWithDateFromDateToPassportNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithDateFromDateToPassport = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
        //    if (resultWithDateFromDateToPassport == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithDateFromDateToNic = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
        //    if (resultWithDateFromDateToNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void GetCustomersWaitingForCardIssuerResponseOrDeclined_Test()
        //{
        //    int totalCount = 0;
        //    string nic = null;
        //    string passportNo = "";
        //    DateTime? createdDateFrom = null;
        //    DateTime? createdDateTo = null;

        //    var result = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount);
        //    if (result == null)
        //        Assert.Fail();

        //    var resultWithNic = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, nic: nic);
        //    if (resultWithNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithPassport = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, passportNo: passportNo);
        //    if (resultWithPassport == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateFrom = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateFrom: createdDateFrom);
        //    if (resultWithCreatedDateFrom == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateTo = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateTo: createdDateTo);
        //    if (resultWithCreatedDateTo == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithNicPassportCardNumber = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, nic: nic, passportNo: passportNo);
        //    if (resultWithNicPassportCardNumber == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithCreatedDateFromNic = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateFrom: createdDateFrom, nic: nic);
        //    if (resultWithCreatedDateFromNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateFromPassport = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo);
        //    if (resultWithCreatedDateFromPassport == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateToPassport = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo);
        //    if (resultWithCreatedDateToPassport == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithCreatedDateToCardNumber = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateTo: createdDateTo, nic: nic);
        //    if (resultWithCreatedDateToCardNumber == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithCreatedDateFromPassportNic = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateFrom: createdDateFrom, passportNo: passportNo, nic: nic);
        //    if (resultWithCreatedDateFromPassportNic == null)
        //    {
        //        Assert.Fail();
        //    }

        //    var resultWithCreatedDateToPassportNic = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
        //    if (resultWithCreatedDateToPassportNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithDateFromDateToPassportNic = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo, nic: nic);
        //    if (resultWithDateFromDateToPassportNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithDateFromDateToPassport = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, passportNo: passportNo);
        //    if (resultWithDateFromDateToPassport == null)
        //    {
        //        Assert.Fail();
        //    }
        //    var resultWithDateFromDateToNic = new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, nic: nic);
        //    if (resultWithDateFromDateToNic == null)
        //    {
        //        Assert.Fail();
        //    }
        //}

        [TestMethod]
        public int AddCustomer_Test(Customer customer, User user, Role role = null)
        {
            bool IsOnline = false;
            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            if (user == null)
                throw new ArgumentNullException("User is required");

            var userService_test = new UserService_Test();

            if (role == null)
            {
                role = userService_test.GetRole_Test("customer");
            }

            user.RoleId = role.Id;
            userService_test.ValidateUser_Test(user, true);
            ValidateCustomer_Test(customer);

            try
            {
                using (var context = new EntityContext())
                {
                    var checkUser = (from u in context.Users where u.Username == user.Username || u.Email == user.Email select u)
                        .SingleOrDefault();

                    if (checkUser != null)
                        return 0;

                    var checkCustomer = (from c in context.Customers
                                         where c.NIC == customer.NIC || c.PassportNo == customer.PassportNo
                                         select c).SingleOrDefault();
                    if (checkCustomer != null)
                        return 0;
                    if (!string.IsNullOrEmpty(user.HashedPassword))
                    {
                        IsOnline = true;
                        user.Status = (int)UserStatus.Inactive;
                    }

                    else
                    {
                        user.ChangePasswordRequired = true;
                    }

                    var commonService = new CommonService();
                    var password = string.IsNullOrEmpty(user.HashedPassword) ? commonService.GeneratePassword(10) : user.HashedPassword;

                    var hashKey = commonService.GeneratePassword(6);
                    user.HashKey = hashKey;

                    user.HashedPassword = commonService.EncodeText(password, hashKey);
                    //user.HashedPassword = commonService.EncodeText(string.IsNullOrEmpty(user.HashedPassword) ? password : user.HashedPassword, hashKey);

                    context.Users.Add(user);

                    if (context.SaveChanges() <= 0)
                        return 0;

                    customer.UserId = user.Id;
                    context.Customers.Add(customer);

                    if (context.SaveChanges() <= 0)
                        return 0;

                    //new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    customer.User = user;

                    // Send Customer Welcome Message
                    new MessageService().SendCustomerWelcomeMessage(customer, password,
                        new SettingService().CustomerDefaultLanguage.Id
                        , IsOnline);


                    return customer.UserId;
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                //log.Error(exception);
                throw new UserException("Sorry, we are unable to process your request. Please try again later.");
            }

        }

        public void ValidateCustomer_Test(Customer customer)
        {
            var settingService = new SettingService();

            if (string.IsNullOrEmpty(customer.Address))
                // throw new ArgumentNullException("Address is required");
                throw new UserException("Address is required");

            int customerAddressMinimumLength = settingService.CustomerAddressMinimumLength;
            int customerAddressMaximumLength = 500;

            if (customer.Address.Length < customerAddressMinimumLength || customer.Address.Length > customerAddressMaximumLength)
                throw new UserException(string.Format("Address Length must be between {0} and {1}"
                    , customerAddressMinimumLength, customerAddressMaximumLength));

            if (string.IsNullOrEmpty(customer.AddressInCustomerLanguage))
                throw new UserException("Address in Customer Language is required");

            if (customer.AddressInCustomerLanguage.Length < customerAddressMinimumLength
                || customer.AddressInCustomerLanguage.Length > customerAddressMaximumLength)
                throw new UserException(string.Format("Address Length must be between {0} and {1}"
                    , customerAddressMinimumLength, customerAddressMaximumLength));

            if (customer.CityId <= 0)
                throw new UserException("City is required");

            if (string.IsNullOrEmpty(customer.ContactNo))
                throw new UserException("Contact Number is required");

            if (customer.ContactNo.Length < 8 || customer.ContactNo.Length > 15)
                ////    throw new ArgumentException(string.Format("@Contact Number length must be between {0} and {1}"
                //       , 8, 15));
                throw new UserException(string.Format("Contact Number length must be between {0} and {1}"
                        , 8, 15));

            if (customer.CountryId <= 0)
                throw new UserException("Country is required");

            if (customer.CurrencyId <= 0)
                throw new UserException("Currency is required");

            int customerMinimumAge = settingService.CustomerMinimumAge;

            if (customer.DateOfBirth <= DateTime.MinValue || customer.DateOfBirth >= DateTime.MaxValue
                || DateTime.Now.AddYears(customerMinimumAge * -1) < customer.DateOfBirth)
                throw new UserException("Date of Birth is invalid. Customer age must be at least " + customerMinimumAge);

            Gender gender;
            if (Enum.TryParse<Gender>(customer.GenderId.ToString(), out gender) == false)
                throw new UserException("Gender is required");

            if (customer.LanguageId <= 0)
                throw new UserException("Language is required");

            MaritalStatus maritalStatus;
            if (Enum.TryParse<MaritalStatus>(customer.MaritalStatusId.ToString(), out maritalStatus) == false)
                throw new UserException("Marital Status is required");

            if (string.IsNullOrEmpty(customer.Nationality))
                throw new UserException("Nationality is required");

            if (customer.Nationality.Length < 4 || customer.Nationality.Length > 100)
                throw new UserException(string.Format("Nationality length must be between {0} and {1}"
                    , 4, 100));

            if (string.IsNullOrEmpty(customer.NIC))
                throw new UserException("NIN is required");

            int ninLength = settingService.NINLength;
            if (customer.NIC.Length != ninLength)
                throw new UserException("NIN Length must be " + ninLength);

            if (string.IsNullOrEmpty(customer.NICDocInCustomerLanguage))
                throw new UserException("NIN Document in Customer Language is required");

            if (customer.NICDocInCustomerLanguage.Length < 4 || customer.NICDocInCustomerLanguage.Length > 100)
                throw new UserException(string.Format("NIN Document In Customer Language length must be between {0} and {1}"
                    , 4, 100));

            if (string.IsNullOrEmpty(customer.PassportNo))
                throw new UserException("Passport Number is required");

            if (customer.PassportNo.Length != settingService.PassportNoLength)
                throw new UserException("Passport Number Length is invalid");

            if (string.IsNullOrEmpty(customer.PassportDocInCustomerLanguage))
                throw new UserException("Passport Document in Customer Language is required");

            if (customer.PassportDocInCustomerLanguage.Length < 4 || customer.PassportDocInCustomerLanguage.Length > 100)
                throw new UserException(string.Format("Passport Document length must be between {0} and {1}"
                    , 4, 100));

            if (string.IsNullOrEmpty(customer.PostalCode))
                throw new UserException("Postal Code is required");

            if (customer.PostalCode.Length < 5 || customer.PostalCode.Length > 10)
                throw new UserException(string.Format("Postal Code length must be between {0} and {1}"
                    , 5, 10));

            //if (customer.UserId <= 0)
            //    throw new ArgumentNullException("User is required");
        }

        public int UpdateCustomer_Test(Customer customer, User user, Role role = null)
        {

            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            if (user == null)
                throw new ArgumentNullException("User is required");

            var userService_Test = new UserService_Test();

            if (role == null)
            {
                role = userService_Test.GetRole_Test("customer");
            }

            user.RoleId = role.Id;

            userService_Test.ValidateUser_Test(user, true);

            ValidateCustomer_Test(customer);

            try
            {
                using (var context = new EntityContext())
                {
                    var checkUser = (from u in context.Users where u.Id == user.Id select u)
                        .SingleOrDefault();

                    if (checkUser != null)
                    {

                        checkUser.Email = user.Email;
                        checkUser.Username = user.Username;
                        checkUser.FirstName = user.FirstName;
                        checkUser.LastName = user.LastName;
                        checkUser.FirstNameInCustomerLanguage = user.FirstNameInCustomerLanguage;
                        checkUser.LastNameInCustomerLanguage = user.LastNameInCustomerLanguage;
                    }

                    var checkCustomer = (from c in context.Customers
                                         where c.UserId == user.Id
                                         select c).SingleOrDefault();

                    if (checkCustomer != null)
                    {
                        checkCustomer.DateOfBirth = customer.DateOfBirth;
                        checkCustomer.Address = customer.Address;
                        checkCustomer.Address2 = customer.Address2;

                        checkCustomer.DateOfBirthInCustomerLanguage = customer.DateOfBirthInCustomerLanguage;
                        checkCustomer.AddressInCustomerLanguage = customer.AddressInCustomerLanguage;
                        checkCustomer.Address2InCustomerLanguage = customer.Address2InCustomerLanguage;

                        checkCustomer.CityId = customer.CityId;
                        checkCustomer.ContactNo = customer.ContactNo;
                        checkCustomer.CountryId = customer.CountryId;
                        checkCustomer.GenderId = customer.GenderId;
                        checkCustomer.LanguageId = customer.LanguageId;
                        checkCustomer.MaritalStatusId = customer.MaritalStatusId;
                        checkCustomer.Nationality = customer.Nationality;
                        checkCustomer.NIC = customer.NIC;
                        checkCustomer.PassportNo = customer.PassportNo;
                        checkCustomer.PostalCode = customer.PostalCode;
                        checkCustomer.CurrencyId = customer.CurrencyId;

                        checkCustomer.PassportDocInCustomerLanguage = customer.PassportDocInCustomerLanguage;
                        checkCustomer.NICDocInCustomerLanguage = customer.NICDocInCustomerLanguage;

                        checkCustomer.CustomerSignedFormInCustomerLanguage = customer.CustomerSignedFormInCustomerLanguage;
                        checkCustomer.CustomerSignedForm = customer.CustomerSignedForm;
                    }

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    customer.UserId = user.Id;
                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);
                    customer.User = user;

                    return customer.UserId;
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                //log.Error(exception);
                throw new UserException("Sorry, we are unable to process your request. Please try again later.");
            }
        }

        public bool UploadCustomerSignedForm_Test(int customerId, string signedForm, string signedFormInCustomerLanguage)
        {
            if (customerId <= 0)
                throw new ArgumentNullException("Customer Id is invalid");

            if (string.IsNullOrEmpty(signedForm))
                throw new ArgumentNullException("Customer Signed Form is required");

            if (string.IsNullOrEmpty(signedFormInCustomerLanguage))
                throw new ArgumentNullException("Customer Signed Form In Customer Language is required");

            signedForm = signedForm.ToLower();
            signedFormInCustomerLanguage = signedFormInCustomerLanguage.ToLower();

            if (!(signedForm.Contains(".jpeg") || signedForm.Contains(".jpg") || signedForm.Contains(".png")))
                throw new ArgumentException("Only JPEG, JPG or PNG are only allowed in Customer Signed Form");

            if (!(signedFormInCustomerLanguage.Contains(".jpeg") || signedFormInCustomerLanguage.Contains(".jpg")
                || signedFormInCustomerLanguage.Contains(".png")))
                throw new ArgumentException("Only JPEG, JPG or PNG are only allowed in Customer Signed Form In Customer Language");

            if ((signedForm.Contains(".jpeg") && signedForm.Length < 5) || signedForm.Length < 4)
                throw new ArgumentException("Customer Signed Form length is invalid");

            if ((signedFormInCustomerLanguage.Contains(".jpeg") && signedFormInCustomerLanguage.Length < 5)
                || signedFormInCustomerLanguage.Length < 4)
                throw new ArgumentException("Customer Signed Form In Customer Language length is invalid");

            try
            {
                using (var context = new EntityContext())
                {
                    var customer = (from cust in context.Customers
                                    join rf in context.RequestForms on cust.UserId equals rf.CustomerId
                                    where cust.UserId == customerId
                                    && rf.TypeId == (int)RequestFormType.Card
                                    && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
                                    select cust).SingleOrDefault();

                    if (customer == null)
                        throw new UserException("Either Customer or their Card Requests not found");

                    if (!string.IsNullOrEmpty(customer.CustomerSignedForm)
                        || !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
                        //|| !string.IsNullOrEmpty(customer.AuthoritySignedForm)
                        //|| !string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage)
                        //|| customer.SentToCardIssuerOn != null || customer.CardIssuerRespondedOn != null)
                        throw new UserException("Customer either already have customer signed form uploaded or processed");

                    customer.CustomerSignedForm = signedForm;
                    customer.CustomerSignedFormInCustomerLanguage = signedFormInCustomerLanguage;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name, performedForUserId: customerId);

                    return true;
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                //log.Error(exception);
                throw new UserException("Sorry, we are unable to process your request. Please try again later.");
            }
        }
        //public Customer GetCustomer_Test(int customerId, bool isForCustomer = false)
        //{
        //    if (customerId <= 0)
        //        return null;

        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var result = (from customer in context.Customers
        //                          join user in context.Users on customer.UserId equals user.Id
        //                          join country in context.Countries on customer.CountryId equals country.Id
        //                          join city in context.Cities on customer.CityId equals city.Id
        //                          join language in context.Languages on customer.LanguageId equals language.Id
        //                          join currency in context.Currencies on customer.CurrencyId equals currency.Id
        //                          where customer.UserId == customerId
        //                          select new
        //                          {
        //                              Customer = customer,
        //                              User = user,
        //                              Country = country,
        //                              City = city,
        //                              Language = language,
        //                              Currency = currency
        //                          }).SingleOrDefault();

        //            if (result != null)
        //            {
        //                result.Customer.User = result.User;
        //                result.Customer.Country = result.Country;
        //                result.Customer.City = result.City;
        //                result.Customer.Language = result.Language;
        //                result.Customer.Currency = result.Currency;
        //                result.Customer.Status = GetCustomerStatus_Test(result.Customer);
        //                GetCustomerStatusString_Test(result.Customer, isForCustomer);
        //                return result.Customer;
        //            }

        //            return null;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        //log.Error(exception);
        //        return null;
        //    }
        //}

        //public void GetCustomerStatusString_Test(Customer customer, bool isForCustomer)
        //{
        //    int totalCount = 0;
        //    try
        //    {
        //        if (isForCustomer)
        //        {

        //            if (customer.CardRequestForms == null)
        //                customer.CardRequestForms = new CardService().GetCardRequestForms(out totalCount, customer.UserId);

        //            if (customer.CardRequestForms == null || customer.CardRequestForms.Count == 0)
        //                customer.StatusString = "Pending Card Requests";
        //            else if (string.IsNullOrEmpty(customer.CustomerSignedForm))
        //                customer.StatusString = "Pending For Customer Signed Form";
        //            else if (customer.ValidatedOn != null && !string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                customer.StatusString = "Validation Failed";
        //            else if ((!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                      && string.IsNullOrEmpty(customer.ProofOfAddressDoc))
        //                     || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                         && string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)))
        //                customer.StatusString = "Processing";
        //            //else if (string.IsNullOrEmpty(customer.AuthoritySignedForm))
        //            //    customer.StatusString = "Processing";
        //            //else if (customer.SentToCardIssuerOn == null)
        //                //customer.StatusString = "Processing";
        //            else if (customer.CardIssuerRespondedOn != null && string.IsNullOrEmpty(customer.DeclinedReason))
        //                customer.StatusString = "Approved";
        //            else if (customer.CardIssuerRespondedOn != null && !string.IsNullOrEmpty(customer.DeclinedReason))
        //                customer.StatusString = "Declined";
        //            //else if (customer.SentToCardIssuerOn != null)
        //            //    customer.StatusString = "Processing";
        //            else
        //                customer.StatusString = "Unknown";
        //        }
        //        else
        //        {
        //            if (customer.CardRequestForms == null)
        //                customer.CardRequestForms = new CardService().GetCardRequestForms(out totalCount, customer.UserId);

        //            if (customer.CardRequestForms == null || customer.CardRequestForms.Count == 0)
        //                customer.StatusString = "Waiting For Card Requests";
        //            else if (string.IsNullOrEmpty(customer.CustomerSignedForm))
        //                customer.StatusString = "Waiting For Customer Signed Form";
        //            else if (customer.CardRequestForms != null && customer.CardRequestForms[0].Payments.Any(pay => pay.ConfirmedOn == null && string.IsNullOrEmpty(pay.ConfirmationFailureReason) && pay.PaymentMethod != PaymentMethod.RAHYAB && pay.PaymentMethod != PaymentMethod.Admin))
        //                customer.StatusString = "Waiting For Payment Confirmation";
        //            else if (customer.ValidatedOn == null)
        //                customer.StatusString = "Waiting For Validation";
        //            else if (customer.ValidatedOn != null && !string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                customer.StatusString = "Validation Failed";
        //            else if ((!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                      && string.IsNullOrEmpty(customer.ProofOfAddressDoc))
        //                     || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                         && string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)))
        //                customer.StatusString = "Waiting For Translation";
        //            else if (string.IsNullOrEmpty(customer.AuthoritySignedForm))
        //                customer.StatusString = "Waiting For Authority Signed Form";
        //            else if (customer.SentToCardIssuerOn == null)
        //                customer.StatusString = "Waiting To Send To Card Issuer";
        //            else if (customer.CardIssuerRespondedOn != null && string.IsNullOrEmpty(customer.DeclinedReason))
        //                customer.StatusString = "Approved";
        //            else if (customer.CardIssuerRespondedOn != null && !string.IsNullOrEmpty(customer.DeclinedReason))
        //                customer.StatusString = "Declined";
        //            else if (customer.SentToCardIssuerOn != null)
        //                customer.StatusString = "Waiting For Card Issuer Response";
        //            else
        //                customer.StatusString = "Unknown";
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        //log.Error(exception);
        //    }
        //}

        public CustomerStatus GetCustomerStatus_Test(Customer customer)
        {
            try
            {
                if (customer.CardIssuerRespondedOn != null && string.IsNullOrEmpty(customer.DeclinedReason))
                    return CustomerStatus.Approved;
                else if (customer.CardIssuerRespondedOn != null && !string.IsNullOrEmpty(customer.DeclinedReason))
                    return CustomerStatus.Failed;
                //if (customer.SentToCardIssuerOn != null)
                //    return CustomerStatus.SentToCardIssuer;
                //else if (!string.IsNullOrEmpty(customer.AuthoritySignedForm))
                //    return CustomerStatus.WaitingToSendToCardIssuer;
                else if (customer.ValidatedOn != null && string.IsNullOrEmpty(customer.ValidationFailureReason))
                    return CustomerStatus.Validated;
                else if (customer.ValidatedOn != null && !string.IsNullOrEmpty(customer.ValidationFailureReason))
                    return CustomerStatus.ValidationFailed;
                else
                    return CustomerStatus.Pending;
            }
            catch (Exception exception)
            {
                //log.Error(exception);
                return 0;
            }
        }
    }
}
//[TestMethod]
//public void InsertUser_Test()
//{
//    var context = new EntityContext();
//    var password = new CommonService().GeneratePassword(10);
//    var hashkey = new CommonService().GeneratePassword(6);
//    context.Users.Add(
//        new User
//        {
//            Username = "Haider_m",
//            FirstName = "Zulfikar",
//            LastName = "Mirza",
//            FirstNameInCustomerLanguage = "Zulfiqar",
//            LastNameInCustomerLanguage = "Mirze",
//            Email = "Haider.mustafa581@yahoo.com",
//            HashKey = hashkey,
//            HashedPassword = new CommonService().EncodeText(password, hashkey),
//        }
//        );
//    context.SaveChanges();

//    if (context.SaveChanges() <= 0)
//    {
//        Assert.Fail();
//    }
//}


//[TestMethod]
//public void InsertCardRequest_Test()
//{
//    var context = new EntityContext();
//    context.CardRequests.Add
//        (
//        new CardRequest
//        {
//            CardRequestFormId = 1974,
//            CardTypeId = 2,
//            CardQty = 1,
//            ServiceExchangeFee = 10002,
//            Fee = 20,
//            CardIssuerRespondedOn = DateTime.Now,
//            DispatchedToTBOOn = DateTime.Now,
//            DispatchedToCustomerOn = DateTime.Now,
//            DeliveredToCustomerOn = DateTime.Now,
//            Description = "Its My Card"
//        }
//        );
//    context.SaveChanges();
//}

//[TestMethod]
//public void InsertCardRequests_Test()
//{
//    var context = new EntityContext();

//    context.CardRequests.Add(
//        new CardRequest
//        {
//            CardRequestFormId = 2000,
//            CardTypeId = 1,
//            CardQty = 1,
//            Fee = 20,
//            CardIssuerRespondedOn = DateTime.Now,
//            DispatchedToTBOOn = DateTime.Now,
//            DispatchedToCustomerOn = DateTime.Now,
//            DeliveredToCustomerOn = DateTime.Now,
//            Description = "Test Method Entry"
//        }
//        );
//    context.SaveChanges();

//    if (context.SaveChanges() <= 0)
//    {
//        Assert.Fail();
//    }
//}

//[TestMethod]
//public void InsertPayment_Test()
//{
//    var context = new EntityContext();

//    context.Payments.Add(
//        new Payment
//        {
//            RequestFormId = 2000,
//            PaymentMethodId = 2,
//            Amount = 540750,
//            CreatedOn = DateTime.Now,
//            TransactionAccountNo = "324234234",
//            TransactionNo = "12324234",
//            TransactionName = "Test Method Entry"
//        }
//        );
//    context.SaveChanges();

//    if (context.SaveChanges() <= 0)
//    {
//        Assert.Fail();
//    }
//}

//[TestMethod]
//public void InsertRequestForms_Test()
//{
//    var context = new EntityContext();
//    context.RequestForms.Add(
//                    new RequestForm
//                    {
//                        TypeId = 10,
//                        CustomerId = 1131,
//                        ExchangeRate = 35000,
//                        Description = "Test Method Description",
//                        IsCancelled = false,
//                        SentToCardIssuerOn = DateTime.Now
//                    }
//                    );
//    context.SaveChanges();
//    if (context.SaveChanges() <= 0)
//    {
//        Assert.Fail();
//    }

//}
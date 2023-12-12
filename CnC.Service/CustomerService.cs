using System;
using System.Collections.Generic;
using System.Linq;
using CnC.Core.Cards;
using CnC.Core.Customers;
using CnC.Core.Payments;
using CnC.Core;
using CnC.Data;
using CnC.Core.Accounts;
using log4net;
using System.Reflection;
using CnC.Core.Exceptions;
using CnC.Core.Affiliates;

namespace CnC.Service
{
    public class CustomerService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        public int AddKycForm(Customer customer, User user, RequestForm requestForm, List<CardRequest> cardRequests
            , List<Payment> payments, Role role = null)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            if (user == null)
                throw new ArgumentNullException("User is required");

            if (requestForm == null)
                throw new ArgumentNullException("Request Form is required");

            if (cardRequests == null || cardRequests.Count == 0)
                throw new ArgumentNullException("Card Requests are required");

            if (payments == null || payments.Count == 0)
                throw new ArgumentNullException("Payments are required");

            var userService = new UserService();

            if (role == null)
            {
                role = userService.GetRole("customer");
            }

            user.RoleId = role.Id;

            userService.ValidateUser(user, true);

            ValidateCustomer(customer);

            try
            {
                using (var context = new EntityContext())
                {
                    var checkUser = (from u in context.Users where u.Username == user.Username || u.Email == user.Email select u)
                        .SingleOrDefault();

                    if (checkUser != null)
                        throw new UserException("User already exist");

                    var checkCustomer = (from cust in context.Customers
                                         where cust.NIC == cust.NIC || cust.PassportNo == cust.PassportNo
                                         select cust).SingleOrDefault();

                    if (checkCustomer != null)
                        throw new UserException("Customer already exist");

                    var commonService = new CommonService();
                    var password = commonService.GeneratePassword(10);
                    var hashKey = commonService.GeneratePassword(6);
                    user.HashedPassword = commonService.EncodeText(password, hashKey);
                    user.HashKey = hashKey;

                    context.Users.Add(user);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    customer.UserId = user.Id;
                    context.Customers.Add(customer);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    requestForm.CustomerId = customer.UserId;
                    new CardService().AddCardRequestForm(requestForm, cardRequests, payments, true);

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    customer.User = user;

                    // Send Customer Welcome Message
                    new MessageService().SendCustomerWelcomeMessage(customer, password, new SettingService().CustomerDefaultLanguage.Id, false);

                    return customer.UserId;
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

        public int AddCustomer(Customer customer, User user, Role role = null, string domain = "")
        {
            bool IsOnline = false;
            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            if (user == null)
                throw new ArgumentNullException("User is required");

            var userService = new UserService();

            if (role == null)
            {
                role = userService.GetRole("customer");
            }

            user.RoleId = role.Id;

            userService.ValidateUser(user, true);

            ValidateCustomer(customer);

            try
            {
                using (var context = new EntityContext())
                {
                    var checkUser = (from u in context.Users
                                     where u.Username == user.Username || u.Email == user.Email
                                     select u)
                        .SingleOrDefault();

                    if (checkUser != null)
                        throw new UserException("Customer with given email already exist");

                    var checkCustomer = (from c in context.Customers
                                         where c.NIC == customer.NIC || c.PassportNo == customer.PassportNo
                                         select c).SingleOrDefault();

                    if (checkCustomer != null)
                        throw new UserException("Customer with given NIN or Passport already exist");

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
                    var password = string.IsNullOrEmpty(user.HashedPassword) ? commonService.GeneratePassword(10)
                        : user.HashedPassword;

                    var hashKey = commonService.GeneratePassword(6);
                    user.HashKey = hashKey;

                    user.HashedPassword = commonService.EncodeText(password, hashKey);

                    context.Users.Add(user);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    customer.UserId = user.Id;
                    context.Customers.Add(customer);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
                                                , performedForUserId: customer.UserId);
                    customer.User = user;

                    // Send Customer Welcome Message
                    new MessageService().SendCustomerWelcomeMessage(customer, password,
                        new SettingService().CustomerDefaultLanguage.Id
                        , IsOnline, domain: domain);

                    return customer.UserId;
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

        public int UpdateCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            ValidateCustomer(customer);

            try
            {
                using (var context = new EntityContext())
                {
                    var checkUser = (from u in context.Users where u.Id == customer.User.Id select u)
                        .SingleOrDefault();

                    if (checkUser != null)
                    {
                        checkUser.Email = customer.User.Email;
                        checkUser.Username = customer.User.Username;
                        checkUser.FirstName = customer.User.FirstName;
                        checkUser.LastName = customer.User.LastName;
                        checkUser.FirstNameInCustomerLanguage = customer.User.FirstNameInCustomerLanguage;
                        checkUser.LastNameInCustomerLanguage = customer.User.LastNameInCustomerLanguage;
                    }

                    var checkCustomer = (from u in context.Customers where u.UserId == customer.User.Id select u)
                        .SingleOrDefault();

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

                    return customer.UserId;
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

        public int UpdateCustomer(Customer customer, User user, Role role = null, string resendPassword = null)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            if (user == null)
                throw new ArgumentNullException("User is required");

            var userService = new UserService();

            if (role == null)
            {
                role = userService.GetRole("customer");
            }

            user.RoleId = role.Id;

            userService.ValidateUser(user, true);

            customer.User = new User();
            customer.User = user;

            ValidateCustomer(customer);

            string password = string.Empty;
            string hashKey = string.Empty;

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
                        if (!string.IsNullOrEmpty(resendPassword))
                        {
                            var commonService = new CommonService();
                            password = commonService.GeneratePassword(10);
                            hashKey = commonService.GeneratePassword(6);
                            checkUser.HashedPassword = commonService.EncodeText(password, hashKey);
                            checkUser.HashKey = hashKey;
                            checkUser.ChangePasswordRequired = true;
                        }
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
                    if (!string.IsNullOrEmpty(resendPassword))
                    {
                        new MessageService().SendCustomerWelcomeMessage(customer: customer, password: password
                            , languageId: new SettingService().CustomerDefaultLanguage.Id, isOnline: false);
                    }

                    return customer.UserId;
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


        private int AddCustomer(string email, string firstName, string lastName, int gender, DateTime dateOfBirth
            , int maritalStatus, string nic, string passportNo, string nationality, string address, string address2
            , int cityid, string postalCode, int countryId, string contactNo, string nicDoc, string passportDoc
            , string proofOfAddressDoc, string proofOfSourceOfFunds, int currencyId, int languageId)
        {
            throw new NotImplementedException();
        }

        private int AddCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            ValidateCustomer(customer);

            try
            {
                using (var context = new EntityContext())
                {
                    if (context.Customers.SingleOrDefault(c => c.UserId == customer.UserId) != null)
                        return -1;

                    context.Customers.Add(customer);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unablle to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return customer.UserId;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int AddKycForm(Customer customer, CardRequestForm cardRequestForm)
        {
            throw new NotImplementedException();
        }

        public int AddKycForm(Customer customer, List<CardRequest> cardRequests, List<Payment> payments)
        {
            throw new NotImplementedException();
        }

        public Customer GetCustomer(int customerId, bool isForCustomer = false)
        {
            if (customerId <= 0)
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join user in context.Users on customer.UserId equals user.Id
                                  join country in context.Countries on customer.CountryId equals country.Id
                                  join city in context.Cities on customer.CityId equals city.Id
                                  join language in context.Languages on customer.LanguageId equals language.Id
                                  join currency in context.Currencies on customer.CurrencyId equals currency.Id
                                  where customer.UserId == customerId
                                  select new
                                  {
                                      Customer = customer,
                                      User = user,
                                      Country = country,
                                      City = city,
                                      Language = language,
                                      Currency = currency
                                  }).SingleOrDefault();

                    if (result != null)
                    {
                        result.Customer.User = result.User;
                        result.Customer.Country = result.Country;
                        result.Customer.City = result.City;
                        result.Customer.Language = result.Language;
                        result.Customer.Currency = result.Currency;
                        result.Customer.Status = GetCustomerStatus(result.Customer);
                        GetCustomerStatusString(result.Customer, isForCustomer);
                        return result.Customer;
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

        public Customer GetCustomerByNic(string nic, bool isForCustomer = false)
        {
            if (string.IsNullOrEmpty(nic))
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join user in context.Users on customer.UserId equals user.Id
                                  join country in context.Countries on customer.CountryId equals country.Id
                                  join city in context.Cities on customer.CityId equals city.Id
                                  join language in context.Languages on customer.LanguageId equals language.Id
                                  join currency in context.Currencies on customer.CurrencyId equals currency.Id
                                  where customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
                                  select new
                                  {
                                      Customer = customer,
                                      User = user,
                                      Country = country,
                                      City = city,
                                      Language = language,
                                      Currency = currency
                                  }).FirstOrDefault();

                    if (result != null)
                    {
                        result.Customer.User = result.User;
                        result.Customer.Country = result.Country;
                        result.Customer.City = result.City;
                        result.Customer.Language = result.Language;
                        result.Customer.Currency = result.Currency;
                        result.Customer.Status = GetCustomerStatus(result.Customer);
                        GetCustomerStatusString(result.Customer, isForCustomer);
                        return result.Customer;
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

        public Customer GetCustomerByPassportNo(string passportNo, bool isForCustomer = false)
        {
            if (string.IsNullOrEmpty(passportNo))
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join user in context.Users on customer.UserId equals user.Id
                                  join country in context.Countries on customer.CountryId equals country.Id
                                  join city in context.Cities on customer.CityId equals city.Id
                                  join language in context.Languages on customer.LanguageId equals language.Id
                                  join currency in context.Currencies on customer.CurrencyId equals currency.Id
                                  where customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase)
                                  select new
                                  {
                                      Customer = customer,
                                      User = user,
                                      Country = country,
                                      City = city,
                                      Language = language,
                                      Currency = currency
                                  }).FirstOrDefault();

                    if (result != null)
                    {
                        result.Customer.User = result.User;
                        result.Customer.Country = result.Country;
                        result.Customer.City = result.City;
                        result.Customer.Language = result.Language;
                        result.Customer.Currency = result.Currency;
                        result.Customer.Status = GetCustomerStatus(result.Customer);
                        GetCustomerStatusString(result.Customer, isForCustomer);
                        return result.Customer;
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

        public Customer GetCustomerByEmail(string email, bool isForCustomer = false)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join user in context.Users on customer.UserId equals user.Id
                                  join country in context.Countries on customer.CountryId equals country.Id
                                  join city in context.Cities on customer.CityId equals city.Id
                                  join language in context.Languages on customer.LanguageId equals language.Id
                                  join currency in context.Currencies on customer.CurrencyId equals currency.Id
                                  where user.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                  select new
                                  {
                                      Customer = customer,
                                      User = user,
                                      Country = country,
                                      City = city,
                                      Language = language,
                                      Currency = currency
                                  }).FirstOrDefault();

                    if (result != null)
                    {
                        result.Customer.User = result.User;
                        result.Customer.Country = result.Country;
                        result.Customer.City = result.City;
                        result.Customer.Language = result.Language;
                        result.Customer.Currency = result.Currency;
                        result.Customer.Status = GetCustomerStatus(result.Customer);
                        GetCustomerStatusString(result.Customer, isForCustomer);
                        return result.Customer;
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

        public Customer GetCustomer(string nic, string securityQuestion, string answer)
        {
            if (string.IsNullOrEmpty(nic) || string.IsNullOrEmpty(securityQuestion) || string.IsNullOrEmpty(answer))
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join user in context.Users on customer.UserId equals user.Id
                                  where customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
                                  select new { Customer = customer, User = user }).SingleOrDefault();

                    if (result != null)
                    {
                        var commonService = new CommonService();
                        if (result.Customer.SecurityQuestion == commonService.EncodeText(securityQuestion.ToUpper(), result.User.HashKey)
                            && result.Customer.Answer == commonService.EncodeText(answer.ToUpper(), result.User.HashKey))
                        {
                            result.Customer.User = result.User;
                            return result.Customer;
                        }
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

        public Customer GetCustomerByRequestFormId(int requestFormId)
        {
            if (requestFormId <= 0)
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    return (from customer in context.Customers
                            join rf in context.RequestForms on customer.UserId equals rf.Id
                            where rf.Id == requestFormId
                            && rf.TypeId == (int)RequestFormType.Card
                            select customer).SingleOrDefault();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<Customer> GetCustomers(out int totalCount, string nic = null, string passportNo = null, string firstName = null, string lastName = null
            , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, UserStatus? userStatus = null
           , string contactNo = null, CustomerStatus? customerStatus = null, string emailAddress = null, string clientId = null
            , bool isForCustomer = false, int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
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
                    var result = (from customer in context.Customers
                                  join user in context.Users on customer.UserId equals user.Id
                                  join rf in
                                  (from reqF in context.RequestForms
                                   join payment in context.Payments on reqF.Id equals payment.RequestFormId
                                   where reqF.TypeId == (int)RequestFormType.Card
                                   group reqF by reqF.CustomerId into requestForms
                                   select new { CustomerId = requestForms.Key, RequestForm = requestForms.FirstOrDefault() }
                                    ) on user.Id equals rf.CustomerId into requestForms
                                  from rf in requestForms.DefaultIfEmpty()
                                      // join payment in context.Payments on rf.RequestForm.Id equals payment.RequestFormId
                                  where
                                  (true == (!string.IsNullOrEmpty(firstName) ? user.FirstName.Contains(firstName.ToUpper()) : true))
                                  && (true == (!string.IsNullOrEmpty(lastName) ? user.LastName.Contains(lastName.ToUpper()) : true))
                                  && (true == (!string.IsNullOrEmpty(emailAddress)
                                  ? user.Email.Contains(emailAddress.ToUpper()) : true))
                                  //&& (true == (!string.IsNullOrEmpty(clientId) ? customer.CardServiceProviderClientId.Contains(clientId.ToUpper()) : true))
                                  && (true == (!string.IsNullOrEmpty(contactNo) ? customer.ContactNo.Equals(contactNo.ToUpper()) : true))
                                  && true == (!string.IsNullOrEmpty(nic)
                                     ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
                                     : (!string.IsNullOrEmpty(passportNo)
                                     ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true))
                                  && user.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : user.CreatedOn)
                                  && user.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : user.CreatedOn)
                                  && user.Status == (userStatus.HasValue ? (int)userStatus.Value : user.Status)
                                  &&
                                  (true ==
                                  customerStatus.HasValue && customerStatus.Value == CustomerStatus.Approved
                                  ? customer.CardIssuerRespondedOn != null && string.IsNullOrEmpty(customer.DeclinedReason)

                                  : customerStatus.HasValue && customerStatus.Value == CustomerStatus.Failed
                                  ? customer.CardIssuerRespondedOn != null && !string.IsNullOrEmpty(customer.DeclinedReason)

                                  //: customerStatus.HasValue && customerStatus.Value == CustomerStatus.SentToCardIssuer
                                  //? customer.SentToCardIssuerOn != null && customer.CardIssuerRespondedOn == null 
                                  //: customerStatus.HasValue && customerStatus.Value == CustomerStatus.WaitingToSendToCardIssuer
                                  //? !string.IsNullOrEmpty(customer.AuthoritySignedForm) && customer.ValidatedOn != null

                                  //&& string.IsNullOrEmpty(customer.ValidationFailureReason)
                                  //&& customer.CardIssuerRespondedOn == null && string.IsNullOrEmpty(customer.DeclinedReason)
                                  //&& customer.SentToCardIssuerOn == null

                                  : customerStatus.HasValue && customerStatus.Value == CustomerStatus.Validated
                                  ? customer.ValidatedOn != null && string.IsNullOrEmpty(customer.ValidationFailureReason)
                                  && customer.CardIssuerRespondedOn == null && string.IsNullOrEmpty(customer.DeclinedReason)

                                  : customerStatus.HasValue && customerStatus.Value == CustomerStatus.ValidationFailed
                                  ? customer.ValidatedOn != null && !string.IsNullOrEmpty(customer.ValidationFailureReason)

                                  : customerStatus.HasValue && customerStatus.Value == CustomerStatus.Pending
                                  ? customer.ValidatedOn == null && string.IsNullOrEmpty(customer.ValidationFailureReason) : true)
                                  select new { Customer = customer, User = user, RequestForm = rf })
                                  .OrderByDescending(r => r.User.CreatedOn);

                    totalCount = result.Count();
                    var customers = result.Skip(skipRows).Take(pageSize.Value).ToList();
                    if (customers != null && customers.Count > 0)
                    {
                        var paymentService = new PaymentService();
                        foreach (var customerUser in customers)
                        {
                            customerUser.Customer.User = customerUser.User;
                            if (customerUser.RequestForm != null)
                            {
                                customerUser.Customer.CardRequestForms = new List<CardRequestForm>();
                                customerUser.RequestForm.RequestForm.Payments =
                                    paymentService.GetPaymentsByRequestFormId(customerUser.RequestForm.RequestForm.Id);
                                customerUser.Customer.CardRequestForms.Add(
                                    new CardRequestForm(customerUser.RequestForm.RequestForm));
                            }
                            customerUser.Customer.Status = GetCustomerStatus(customerUser.Customer);
                            GetCustomerStatusString(customerUser.Customer, isForCustomer);
                        }
                        return customers.Select(r => r.Customer).ToList();
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

        public bool IsCustomerExist(int customerId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return (from user in context.Users
                            join customer in context.Customers on user.Id equals customer.UserId
                            where user.Id == customerId
                            && user.Status == (int)UserStatus.Active
                            select customer).Count() > 0;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        public bool IsCustomerExist(string emailAddress)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from user in context.Users
                                  join customer in context.Customers on user.Id equals customer.UserId
                                  where user.Email.Equals(emailAddress, StringComparison.OrdinalIgnoreCase)
                                  && user.Status == (int)UserStatus.Active
                                  select customer).Count() > 0;
                    return result;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        public Customer GetCustomerWithNIC(string nic)
        {
            if (string.IsNullOrEmpty(nic))
                throw new UserException("NIC is required");

            try
            {
                using (var context = new EntityContext())
                {
                    var customer = (from cust in context.Customers
                                    where cust.NIC.Equals(nic)
                                    select cust).SingleOrDefault();

                    return customer;
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

        public bool IsShoppingEmailExist(string emailForShopping)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from user in context.Users
                                  join customer in context.Customers on user.Id equals customer.UserId
                                  where customer.EmailForShopping.Equals(emailForShopping
                                  , StringComparison.OrdinalIgnoreCase)
                                  select customer).Count() > 0;
                    return result;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        public bool IsCustomerChangePasswordRequired(string emailAddress)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return (from user in context.Users
                            join customer in context.Customers on user.Id equals customer.UserId
                            where user.Email.Equals(emailAddress, StringComparison.OrdinalIgnoreCase)
                            && !user.ChangePasswordRequired
                            select customer).Count() > 0;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }
        //public bool IsCustomerExist(string cardServiceProviderClientId, string shopingEmailAddress)
        //{
        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            return (from user in context.Users
        //                    join customer in context.Customers on user.Id equals customer.UserId
        //                    where (customer.CardServiceProviderClientId.Equals(cardServiceProviderClientId
        //                    , StringComparison.OrdinalIgnoreCase)
        //                    || customer.EmailForShopping.Equals(shopingEmailAddress, StringComparison.OrdinalIgnoreCase))
        //                    && user.Status == (int)UserStatus.Active
        //                    select customer).Count() > 0;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        log.Error(exception);
        //        return false;
        //    }
        //}

        public bool IsCustomerValidated(int customerId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.Customers.Count(c => c.UserId == customerId && c.ValidatedOn != null
                    && string.IsNullOrEmpty(c.ValidationFailureReason)) > 0;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        #region Pending (Customers With Incomplete Info or Under Processing)

        #region Get

        /// <summary>
        /// Return Customers who did not make any Card Request yet
        /// </summary>
        /// <returns></returns>
        public List<Customer> GetCustomersPendingForCardRequests(out int totalCount, string nic = null, string passportNo = null
            , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, int pageIndex = 0, int? pageSize = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;

            if (createdDateTo.HasValue)
                createdDateTo = createdDateTo.Value.AddDays(1);
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join u in context.Users on customer.UserId equals u.Id
                                  join rf in context.RequestForms on u.Id equals rf.CustomerId into requestForms
                                  from reqF in requestForms.DefaultIfEmpty()
                                  where reqF == null
                                  && u.Status != (int)UserStatus.Deleted
                                  && true == (!string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
                                  : (!string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true))
                                  && u.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : u.CreatedOn)
                                  && u.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : u.CreatedOn)
                                  select new { Customer = customer, User = u })
                                     .OrderByDescending(r => r.User.CreatedOn);
                    totalCount = result.Count();
                    var customers = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    foreach (var cust in customers)
                    {
                        cust.Customer.User = cust.User;
                    }

                    return customers.Select(c => c.Customer).ToList();
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
        /// Return Custmers who did not upload their Signed Copy yet
        /// </summary>
        /// <returns></returns>
        public List<Customer> GetCustomersPendingForSignedForm(out int totalCount, string nic = null, string passportNo = null
            , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, int pageIndex = 0, int? pageSize = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;

            if (createdDateTo.HasValue)
                createdDateTo = createdDateTo.Value.AddDays(1);
            totalCount = 0;
            //context.RequestForms.Any(rf => rf.CustomerId == user.Id
            //                         && rf.TypeId == (int)RequestFormType.Card)
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join user in context.Users on customer.UserId equals user.Id
                                  join rf in context.RequestForms on user.Id equals rf.CustomerId
                                  where (string.IsNullOrEmpty(customer.CustomerSignedForm)
                                  || string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
                                  && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
                                  && true == (!string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
                                  : (!string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true))
                                  && user.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : user.CreatedOn)
                                  && user.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : user.CreatedOn)
                                  && rf.TypeId == (int)RequestFormType.Card
                                  select new { Customer = customer, User = user })
                                     .OrderByDescending(r => r.User.CreatedOn);

                    totalCount = result.Count();
                    var customers = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    foreach (var cust in customers)
                    {
                        cust.Customer.User = cust.User;
                    }

                    return customers.Select(c => c.Customer).ToList();
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
        /// Return Custmers who either are not validated yet or were failed to validate
        /// </summary>
        /// <returns></returns>
        public List<Customer> GetCustomersPendingForValidatationOrFailed(out int totalCount, string nic = null
            , string passportNo = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null
            , PaymentMethod? paymentMethod = null, bool CanViewOnlineRecords = false, bool CanViewBankRecords = false
            , int pageIndex = 0, int? pageSize = null, bool showFailed = true)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;

            if (createdDateTo.HasValue)
                createdDateTo = createdDateTo.Value.AddDays(1);
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join user in context.Users on customer.UserId equals user.Id
                                  join rf in context.RequestForms on user.Id equals rf.CustomerId
                                  join payment in context.Payments on rf.Id equals payment.RequestFormId
                                  where
                                   (customer.ValidatedOn == null
                                  || (showFailed && !string.IsNullOrEmpty(customer.ValidationFailureReason)))
                                  && !string.IsNullOrEmpty(customer.CustomerSignedForm)
                                  && !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage)
                                  && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
                                  && true == (!string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
                                  : (!string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true))
                                  && user.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value
                                  : user.CreatedOn)
                                  && user.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : user.CreatedOn)
                                  && rf.TypeId == (int)RequestFormType.Card
                                  && true == (!paymentMethod.HasValue
                                  ? true : CanViewOnlineRecords && payment.PaymentMethodId == (int)PaymentMethod.Online
                                  ? true : CanViewBankRecords && payment.PaymentMethodId == (int)PaymentMethod.Bank
                                  ? true : payment.PaymentMethodId == (int)paymentMethod.Value)
                                  && payment.ConfirmedOn != null && payment.ReConfirmedOn != null
                                  && string.IsNullOrEmpty(payment.ConfirmationFailureReason)
                                  select new { Customer = customer, User = user })
                                     .OrderByDescending(r => r.User.CreatedOn);

                    totalCount = result.Count();
                    var customers = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    foreach (var cust in customers)
                    {
                        cust.Customer.User = cust.User;
                    }

                    return customers.Select(c => c.Customer).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        //public List<Customer> GetCustomersPendingForInfoCorrection(out int totalCount, string nic = null
        //   , string passportNo = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null
        //    , int pageIndex = 0, int? pageSize = null)
        //{
        //    if (!pageSize.HasValue)
        //        pageSize = new SettingService().ResultPageSize;

        //    int skipRows = pageSize.Value * pageIndex;

        //    if (createdDateTo.HasValue)
        //        createdDateTo = createdDateTo.Value.AddDays(1);
        //    totalCount = 0;
        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var result = (from customer in context.Customers
        //                          join user in context.Users on customer.UserId equals user.Id
        //                          where ((customer.ValidatedOn == null &&
        //                          !string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                          || (customer.ValidatedOn == null
        //                          && string.IsNullOrEmpty(customer.ValidationFailureReason)))
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedForm)
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage)
        //                          && context.RequestForms.Any(cr => cr.CustomerId == customer.UserId)
        //                          && true == (!string.IsNullOrEmpty(nic)
        //                          ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
        //                          : (!string.IsNullOrEmpty(passportNo)
        //                          ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true))
        //                          && user.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value
        //                          : user.CreatedOn)
        //                          && user.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : user.CreatedOn)
        //                          select new { Customer = customer, User = user })
        //                             .OrderByDescending(r => r.User.CreatedOn);

        //            totalCount = result.Count();
        //            var customers = result.Skip(skipRows).Take(pageSize.Value).ToList();

        //            foreach (var cust in customers)
        //            {
        //                cust.Customer.User = cust.User;
        //            }

        //            return customers.Select(c => c.Customer).ToList();
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        new MessageService().SendExceptionMessage(exception);
        //        log.Error(exception);
        //        return null;
        //    }
        //}

        //public List<CardRequestForm> GetCustomerCardRequestPendingForValidatationOrFailed(out int totalCount
        //    , string nic = null, string passportNo = null, int? customerId = null, DateTime? createdDateFrom = null
        //    , DateTime? createdDateTo = null, PaymentMethod? paymentMethod = null, bool CanViewOnlineRecords = false
        //    , bool CanViewBankRecords = false, int pageIndex = 0, int? pageSize = null, bool showFailed = true
        //    , bool isForCustomer = false)
        //{
        //    if (!pageSize.HasValue)
        //        pageSize = new SettingService().ResultPageSize;

        //    int skipRows = pageSize.Value * pageIndex;

        //    if (createdDateTo.HasValue)
        //        createdDateTo = createdDateTo.Value.AddDays(1);
        //    totalCount = 0;
        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var result = (from rf in context.RequestForms
        //                          join customer in context.Customers on rf.CustomerId equals customer.UserId
        //                          join user in context.Users on customer.UserId equals user.Id
        //                          join cr in context.CardRequests on rf.Id equals cr.CardRequestFormId
        //                          join payment in context.Payments on rf.Id equals payment.RequestFormId into payments
        //                          from p in payments
        //                          join ct in context.CardTypes on cr.CardTypeId equals ct.Id
        //                          join c in context.Cards on cr.Id equals c.CardRequestId into cards
        //                          from c in cards.DefaultIfEmpty()
        //                          where true == (customerId.HasValue ? customer.UserId == customerId :
        //                          !string.IsNullOrEmpty(nic)
        //                          ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
        //                          !string.IsNullOrEmpty(passportNo)
        //                          ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase)
        //                          : true)
        //                          && rf.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : rf.CreatedOn)
        //                          && rf.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : rf.CreatedOn)
        //                          && rf.TypeId == (int)RequestFormType.Card
        //                          orderby rf.CreatedOn descending
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

        //                    foreach (var requestForm in grp.DistinctBy(r => r.Payment))
        //                    {
        //                        cardRequestForm.Payments.Add(requestForm.Payment);
        //                    }

        //                    foreach (var requestForm in grp.DistinctBy(r => r.CardRequest))
        //                    {
        //                        requestForm.CardRequest.CardType = requestForm.CardType;
        //                        requestForm.CardRequest.Card = requestForm.Card;
        //                        requestForm.CardRequest.CardRequestForm = cardRequestForm;
        //                        requestForm.CardRequest.StatusString =
        //                           new CardService().GetCardRequestStatusString(requestForm.CardRequest, isForCustomer);
        //                        cardRequestForm.CardRequests.Add(requestForm.CardRequest);
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

        /// <summary>
        /// Return Custmers who did not make any Card Request yet
        /// </summary>
        /// <returns></returns>
        //public List<Customer> GetCustomersPendingForTranslation(out int totalCount, string nic = null, string passportNo = null
        //    , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, int pageIndex = 0, int? pageSize = null)
        //{
        //    if (!pageSize.HasValue)
        //        pageSize = new SettingService().ResultPageSize;

        //    int skipRows = pageSize.Value * pageIndex;

        //    if (createdDateTo.HasValue)
        //        createdDateTo = createdDateTo.Value.AddDays(1);
        //    totalCount = 0;
        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var result = (from customer in context.Customers
        //                          join user in context.Users on customer.UserId equals user.Id
        //                          join rf in context.RequestForms on user.Id equals rf.CustomerId
        //                          where (
        //                          (!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                          //&& string.IsNullOrEmpty(customer.ProofOfAddressDoc)
        //                          )
        //                          || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                          //&& string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)
        //                          )
        //                          )
        //                          && (customer.ValidatedOn != null && string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedForm)
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage)
        //                          && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
        //                          && true == (!string.IsNullOrEmpty(nic)
        //                          ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
        //                          : (!string.IsNullOrEmpty(passportNo)
        //                          ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true))
        //                          && user.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : user.CreatedOn)
        //                          && user.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : user.CreatedOn)
        //                          && rf.TypeId == (int)RequestFormType.Card
        //                          select new { Customer = customer, User = user })
        //                             .OrderByDescending(r => r.User.CreatedOn);
        //            totalCount = result.Count();
        //            var customers = result.Skip(skipRows).Take(pageSize.Value).ToList();

        //            foreach (var cust in customers)
        //            {
        //                cust.Customer.User = cust.User;
        //            }

        //            return customers.Select(c => c.Customer).ToList();
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        log.Error(exception);
        //        return null;
        //    }
        //}

        /// <summary>
        /// Return Custmers whose KYC Form is not signed by Authority yet
        /// </summary>
        /// <returns></returns>
        //public List<Customer> GetCustomersPendingForAuthoritySignature(out int totalCount, string nic = null, string passportNo = null
        //    , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, int pageIndex = 0, int? pageSize = null)
        //{
        //    if (!pageSize.HasValue)
        //        pageSize = new SettingService().ResultPageSize;

        //    int skipRows = pageSize.Value * pageIndex;

        //    if (createdDateTo.HasValue)
        //        createdDateTo = createdDateTo.Value.AddDays(1);
        //    totalCount = 0;
        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var result = (from customer in context.Customers
        //                          join user in context.Users on customer.UserId equals user.Id
        //                          join rf in context.RequestForms on user.Id equals rf.CustomerId
        //                          where 
        //                          //(string.IsNullOrEmpty(customer.AuthoritySignedForm)
        //                          //|| string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage))
        //                          (
        //                          (string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                          || (!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                          && !string.IsNullOrEmpty(customer.ProofOfAddressDoc)))
        //                          || (string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                          || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                          && !string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)))
        //                          )
        //                          && (customer.ValidatedOn != null && string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedForm)
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage)
        //                          && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
        //                          && true == (!string.IsNullOrEmpty(nic)
        //                          ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
        //                          : (!string.IsNullOrEmpty(passportNo)
        //                          ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true))
        //                          && user.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : user.CreatedOn)
        //                          && user.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : user.CreatedOn)
        //                          && rf.TypeId == (int)RequestFormType.Card
        //                          select new { Customer = customer, User = user })
        //                             .OrderByDescending(r => r.User.CreatedOn);

        //            totalCount = result.Count();
        //            var customers = result.Skip(skipRows).Take(pageSize.Value);
        //            foreach (var cust in customers)
        //            {
        //                cust.Customer.User = cust.User;
        //            }

        //            return customers.Select(c => c.Customer).ToList();
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        log.Error(exception);
        //        return null;
        //    }
        //}

        /// <summary>
        /// Return Custmers who are ready to send to Card Issuer
        /// </summary>
        /// <returns></returns>
        //public List<Customer> GetCustomersPendingToSendToCardIssuer(out int totalCount, string nic = null, string passportNo = null
        //    , DateTime? createdDateFrom = null, DateTime? createdDateTo = null, int pageIndex = 0, int? pageSize = null)
        //{
        //    if (!pageSize.HasValue)
        //        pageSize = new SettingService().ResultPageSize;

        //    int skipRows = pageSize.Value * pageIndex;

        //    if (createdDateTo.HasValue)
        //        createdDateTo = createdDateTo.Value.AddDays(1);
        //    totalCount = 0;
        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var result = (from customer in context.Customers
        //                          join user in context.Users on customer.UserId equals user.Id
        //                          join rf in context.RequestForms on user.Id equals rf.CustomerId
        //                          where (customer.SentToCardIssuerOn == null)
        //                          && !string.IsNullOrEmpty(customer.AuthoritySignedForm)
        //                          && !string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage)
        //                          && (
        //                          (string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                          || (!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                          && !string.IsNullOrEmpty(customer.ProofOfAddressDoc)))
        //                          || (string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                          || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                          && !string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)))
        //                          )
        //                          && (customer.ValidatedOn != null && string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedForm)
        //                          && !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage)
        //                          && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
        //                          && true == (!string.IsNullOrEmpty(nic)
        //                          ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase)
        //                          : (!string.IsNullOrEmpty(passportNo)
        //                          ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true))
        //                          && user.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : user.CreatedOn)
        //                          && user.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : user.CreatedOn)
        //                          && rf.TypeId == (int)RequestFormType.Card
        //                          select new { Customer = customer, User = user })
        //                             .OrderByDescending(r => r.User.CreatedOn);
        //            totalCount = result.Count();
        //            var customers = result.Skip(skipRows).Take(pageSize.Value).ToList();

        //            foreach (var cust in customers)
        //            {
        //                cust.Customer.User = cust.User;
        //            }

        //            return customers.Select(c => c.Customer).ToList();
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        log.Error(exception);
        //        return null;
        //    }
        //}

        #endregion Get

        #region Save

        /// <summary>
        /// Upload Customer Signed KYC Form
        /// </summary>
        public bool UploadCustomerSignedForm(int customerId, string signedForm, string signedFormInCustomerLanguage)
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
                        || !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage)
                        //|| !string.IsNullOrEmpty(customer.AuthoritySignedForm)
                        //|| !string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage)
                        //|| customer.SentToCardIssuerOn != null
                        || customer.CardIssuerRespondedOn != null)
                        throw new UserException("Customer either already have customer signed form uploaded or processed");

                    customer.CustomerSignedForm = signedForm;
                    customer.CustomerSignedFormInCustomerLanguage = signedFormInCustomerLanguage;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
                        , performedForUserId: customerId);

                    return true;
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
        /// Update Customer Validation Status
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="failureReason"></param>
        /// <returns></returns>
        public bool SetCustomerValidated(int customerId, string failureReason)
        {
            if (customerId <= 0)
                throw new ArgumentNullException("Customer Id is invalid");

            try
            {
                using (var context = new EntityContext())
                {
                    var customer = (from cust in context.Customers
                                    join rf in context.RequestForms on cust.UserId equals rf.CustomerId
                                    where cust.UserId == customerId
                                    && rf.TypeId == (int)RequestFormType.Card
                                    && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
                                    select cust).FirstOrDefault();

                    if (customer == null)
                        throw new UserException("Either Customer or their Card Requests not found");

                    if (string.IsNullOrEmpty(customer.CustomerSignedForm)
                        || string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
                        throw new UserException("Customer signed form is missing");

                    if (/*!string.IsNullOrEmpty(customer.AuthoritySignedForm) || customer.SentToCardIssuerOn != null*/
                        /*|| */
                            customer.CardIssuerRespondedOn != null)
                        throw new UserException("Customer either have already been processed or under process");

                    customer.ValidatedOn = DateTime.Now;
                    customer.ValidationFailureReason = string.IsNullOrEmpty(failureReason) ? null : failureReason;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    // If validation failed then send Customer email with reason
                    if (!string.IsNullOrEmpty(failureReason))
                        new MessageService().SendCustomerValidationFailedMessage(customer, failureReason
                            , customer.LanguageId);

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
                                                 , performedForUserId: customerId);

                    return true;
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
        /// Upload Customer Supporting Documents Translation
        /// </summary>
        //public bool UploadTranslatedDocuments(int customerId, List<string> proofOfAddressDocs
        //    , List<string> proofOfSourceOfFundsDocs)
        //{
        //    if (customerId <= 0)
        //        throw new ArgumentNullException("Customer Id is invalid");

        //    if (proofOfAddressDocs == null || proofOfAddressDocs.Count == 0)
        //        throw new ArgumentNullException("Proof Of Address Docs are required");

        //    if (proofOfAddressDocs.Any(d => !(d.ToLower().Contains(".jpeg") || d.ToLower().Contains(".jpg")
        //                    || d.ToLower().Contains(".png"))))
        //        throw new UserException("Only JPEG, JPG or PNG are allowed in Proof of Address Docs");

        //    if (proofOfAddressDocs.Any(d => (d.ToLower().Contains(".jpeg") && d.Length < 5) || d.Length < 4))
        //        throw new ArgumentException("Proof of Address Docs length is invalid");

        //    if (proofOfSourceOfFundsDocs == null || proofOfSourceOfFundsDocs.Count == 0)
        //        throw new ArgumentNullException("Proof Of Source Of Funds Docs In Customer Language is required");

        //    if (proofOfSourceOfFundsDocs.Any(d => !(d.ToLower().Contains(".jpeg") || d.ToLower().Contains(".jpg")
        //                    || d.ToLower().Contains(".png"))))
        //        throw new ArgumentException("Only JPEG, JPG or PNG are allowed in Proof of Source of Funds Docs");

        //    if (proofOfSourceOfFundsDocs.Any(d => (d.ToLower().Contains(".jpeg") && d.Length < 5) || d.Length < 4))
        //        throw new ArgumentException("Proof of Source Of Funds Docs length is invalid");

        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var customer = (from cust in context.Customers
        //                            join rf in context.RequestForms on cust.UserId equals rf.CustomerId
        //                            where cust.UserId == customerId
        //                            && rf.TypeId == (int)RequestFormType.Card
        //                            && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
        //                            select cust).SingleOrDefault();

        //            if (customer == null)
        //                throw new UserException("Either Customer or their Card Requests not found");

        //            if (string.IsNullOrEmpty(customer.CustomerSignedForm)
        //                || string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
        //                throw new UserException("Customer signed form is missing");

        //            if (customer.ValidatedOn == null || !string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                throw new UserException("Customer validation is either pending or failed");

        //            if (/*!string.IsNullOrEmpty(customer.AuthoritySignedForm)*/
        //            //    || !string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage)
        //            //    || customer.SentToCardIssuerOn != null ||
        //            customer.CardIssuerRespondedOn != null)
        //                throw new UserException("Customer either already have authority signed copy uploaded or processed");

        //            customer.ProofOfAddressDoc = string.Join(";", proofOfAddressDocs);
        //            customer.ProofOfSourceOfFundsDoc = string.Join(";", proofOfSourceOfFundsDocs);

        //            if (context.SaveChanges() <= 0)
        //                throw new UserException("Unable to save");

        //            new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name, performedForUserId: customerId);

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

        /// <summary>
        /// Upload Authority Signed KYC Form 
        /// In initial phases despite this function SetAuthoritySignedFormUploaded will be used
        /// </summary>
        //private bool UploadAuthoritySignedForm(int customerId, string signedForm, string signedFormInCustomerLanguage)
        //{
        //    if (customerId <= 0)
        //        throw new ArgumentNullException("Customer Id is invalid");

        //    if (string.IsNullOrEmpty(signedForm))
        //        throw new ArgumentNullException("Authority Signed Form is required");

        //    if (string.IsNullOrEmpty(signedFormInCustomerLanguage))
        //        throw new ArgumentNullException("Authority Signed Form In Customer Language is required");

        //    signedForm = signedForm.ToLower();
        //    signedFormInCustomerLanguage = signedFormInCustomerLanguage.ToLower();

        //    if (!(signedForm.Contains(".jpeg") || signedForm.Contains(".jpg") || signedForm.Contains(".png")))
        //        throw new ArgumentException("Only JPEG, JPG or PNG are only allowed in Authority Signed Form");

        //    if (!(signedFormInCustomerLanguage.Contains(".jpeg") || signedFormInCustomerLanguage.Contains(".jpg")
        //        || signedFormInCustomerLanguage.Contains(".png")))
        //        throw new ArgumentException("Only JPEG, JPG or PNG are only allowed in Authority Signed Form In Customer Language");

        //    if ((signedForm.Contains(".jpeg") && signedForm.Length < 5) || signedForm.Length < 4)
        //        throw new ArgumentException("Authority Signed Form length is invalid");

        //    if ((signedFormInCustomerLanguage.Contains(".jpeg") && signedFormInCustomerLanguage.Length < 5)
        //        || signedFormInCustomerLanguage.Length < 4)
        //        throw new ArgumentException("Authority Signed Form In Customer Language length is invalid");

        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var customer = (from cust in context.Customers
        //                            join rf in context.RequestForms on cust.UserId equals rf.CustomerId
        //                            where cust.UserId == customerId
        //                            && rf.TypeId == (int)RequestFormType.Card
        //                            && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
        //                            select cust).SingleOrDefault();

        //            if (customer == null)
        //                throw new UserException("Either Customer or their Card Requests not found");

        //            if (string.IsNullOrEmpty(customer.CustomerSignedForm)
        //                || string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
        //                throw new UserException("Customer signed form is missing");

        //            if (customer.ValidatedOn == null || !string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                throw new UserException("Customer validation is either pending or failed");

        //            if ((!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                && string.IsNullOrEmpty(customer.ProofOfAddressDoc))
        //                || !string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                && string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc))
        //                throw new UserException("Customer supporting documents translation is missing");

        //            if (
        //                //!string.IsNullOrEmpty(customer.AuthoritySignedForm)
        //                //|| !string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage)
        //                //|| customer.SentToCardIssuerOn != null ||
        //                customer.CardIssuerRespondedOn != null)
        //                throw new UserException("Customer either already have authority signed copy uploaded or processed");

        //            customer.AuthoritySignedForm = signedForm;
        //            customer.AuthoritySignedFormInCustomerLanguage = signedFormInCustomerLanguage;

        //            if (context.SaveChanges() <= 0)
        //                throw new UserException("Unable to save");

        //            new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name, performedForUserId: customerId);

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

        /// <summary>
        /// Marked Authority Signed KYC Form Uploaded
        /// In initial phases this will be used instead of UploadAuthoritySignedForm
        /// </summary>
        //public bool SetAuthoritySignedFormUploaded(int customerId)
        //{
        //    if (customerId <= 0)
        //        throw new ArgumentNullException("Customer Id is invalid");

        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var customer = (from cust in context.Customers
        //                            join rf in context.RequestForms on cust.UserId equals rf.CustomerId
        //                            where cust.UserId == customerId
        //                            && rf.TypeId == (int)RequestFormType.Card
        //                            && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
        //                            select cust).SingleOrDefault();

        //            if (customer == null)
        //                throw new UserException("Either Customer or their Card Requests not found");

        //            if (string.IsNullOrEmpty(customer.CustomerSignedForm)
        //                || string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
        //                throw new UserException("Customer signed form is missing");

        //            if (customer.ValidatedOn == null || !string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                throw new UserException("Customer validation is either pending or failed");

        //            if ((!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                && string.IsNullOrEmpty(customer.ProofOfAddressDoc))
        //                || !string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                && string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc))
        //                throw new UserException("Customer supporting documents translation is missing");

        //            if (!string.IsNullOrEmpty(customer.AuthoritySignedForm)
        //                || !string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage)
        //                || customer.SentToCardIssuerOn != null || customer.CardIssuerRespondedOn != null)
        //                throw new UserException("Customer either already have authority signed copy uploaded or processed");

        //            customer.AuthoritySignedForm = customer.CustomerSignedForm;
        //            customer.AuthoritySignedFormInCustomerLanguage = customer.CustomerSignedFormInCustomerLanguage;

        //            if (context.SaveChanges() <= 0)
        //                throw new UserException("Unable to save");

        //            new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
        //                , performedForUserId: customerId);

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

        /// <summary>
        /// Send Customer (KYC Form) To Card Issuer When it is ready
        /// </summary>
        //public bool SendCustomerToCardIssuer(int customerId)
        //{
        //    if (customerId <= 0)
        //        throw new ArgumentNullException("Customer Id is invalid");

        //    try
        //    {
        //        using (var context = new EntityContext())
        //        {
        //            var customer = (from cust in context.Customers
        //                            join rf in context.RequestForms on cust.UserId equals rf.CustomerId
        //                            where cust.UserId == customerId
        //                            && rf.TypeId == (int)RequestFormType.Card
        //                            && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
        //                            select cust).SingleOrDefault();

        //            if (customer == null)
        //                throw new UserException("Either Customer or their Card Requests not found");

        //            if (string.IsNullOrEmpty(customer.CustomerSignedForm)
        //                || string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
        //                throw new UserException("Customer signed form is missing");

        //            if (customer.ValidatedOn == null || !string.IsNullOrEmpty(customer.ValidationFailureReason))
        //                throw new UserException("Customer validation is either pending or failed");

        //            if ((!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
        //                && string.IsNullOrEmpty(customer.ProofOfAddressDoc))
        //                || !string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
        //                && string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc))
        //                throw new UserException("Customer supporting documents translation is missing");

        //            if (string.IsNullOrEmpty(customer.AuthoritySignedForm)
        //                || string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage))
        //                throw new UserException("Authority signature on Customer form is missing");

        //            if (customer.SentToCardIssuerOn != null || customer.CardIssuerRespondedOn != null)
        //                throw new UserException("Customer either already have been processed or under process");

        //            customer.SentToCardIssuerOn = DateTime.Now;

        //            if (context.SaveChanges() <= 0)
        //                throw new UserException("Unable to save");

        //            new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
        //                , performedForUserId: customerId);

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

        #endregion Save

        #endregion Pending & Processing (Customers With Incomplete Info or Under Processing)

        #region Processed (Customers who have been approved or declined)

        /// <summary>
        /// Return Customers who are either waiting for Card Issuer Response or were Declined
        /// </summary>
        /// <returns></returns>
        public List<Customer> GetCustomersWaitingForCardIssuerResponseOrDeclined(out int totalCount, string nic = null
            , string passportNo = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null
            , int pageIndex = 0, int? pageSize = null, bool showFailed = true)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;

            if (createdDateTo.HasValue)
                createdDateTo = createdDateTo.Value.AddDays(1);
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join u in context.Users on customer.UserId equals u.Id
                                  join rf in context.RequestForms on u.Id equals rf.CustomerId
                                  where (customer.CardIssuerRespondedOn == null ||
                                  (showFailed && !string.IsNullOrEmpty(customer.DeclinedReason)))
                                  //&& customer.SentToCardIssuerOn != null
                                  //&& !string.IsNullOrEmpty(customer.AuthoritySignedForm)
                                  //&& !string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage)
                                  && (
                                  (string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
                                  || (!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
                                  //&& !string.IsNullOrEmpty(customer.ProofOfAddressDoc)
                                  )
                                  )
                                  || (string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
                                  || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
                                  //&& !string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)
                                  )
                                  )
                                  )
                                  && (customer.ValidatedOn != null
                                  && string.IsNullOrEmpty(customer.ValidationFailureReason))
                                  && !string.IsNullOrEmpty(customer.CustomerSignedForm)
                                  && !string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage)
                                  && context.CardRequests.Any(cr => cr.CardRequestFormId == rf.Id)
                                  && true == (!string.IsNullOrEmpty(nic)
                                  ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) : true)
                                  && true == (!string.IsNullOrEmpty(passportNo)
                                  ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
                                  && u.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : u.CreatedOn)
                                  && u.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : u.CreatedOn)
                                  && rf.TypeId == (int)RequestFormType.Card
                                  select new { Customer = customer, User = u })
                                     .OrderByDescending(r => r.User.CreatedOn);

                    totalCount = result.Count();
                    var customers = result.Skip(skipRows).Take(pageSize.Value).ToList();
                    foreach (var cust in customers)
                    {
                        cust.Customer.User = cust.User;
                        cust.Customer.Status = GetCustomerStatus(cust.Customer);
                        GetCustomerStatusString(cust.Customer, false);
                    }

                    return customers.Select(c => c.Customer).ToList();

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
        /// Set Customer Approved or Declined on base of Card Issuer response
        /// In case of approved send Card Issuer Client Id in message
        /// In case of declined send reason
        /// </summary>
        public bool SetCustomerApprovedOrDeclined(int customerId, string billingAddress
                                                 , string emailForShopping, string declinedReason)
        {
            if (customerId <= 0)
                throw new ArgumentNullException("Customer Id is invalid");

            if (string.IsNullOrEmpty(declinedReason)
                && (string.IsNullOrEmpty(billingAddress) || string.IsNullOrEmpty(emailForShopping)))
                throw new ArgumentNullException("Billing Address and Email For Shopping are required");

            if (string.IsNullOrEmpty(declinedReason) && IsShoppingEmailExist(emailForShopping))
                throw new UserException("Email For Shopping already exist.");

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

                    if (string.IsNullOrEmpty(customer.CustomerSignedForm)
                        || string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
                        throw new UserException("Customer signed form is missing");

                    if (customer.ValidatedOn == null || !string.IsNullOrEmpty(customer.ValidationFailureReason))
                        throw new UserException("Customer validation is either pending or failed");

                    //if (!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
                    //    //&& string.IsNullOrEmpty(customer.ProofOfAddressDoc)

                    //    || !string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
                    //    //&& string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)
                    //    )
                    //    throw new UserException("Customer supporting documents translation is missing");

                    //if (string.IsNullOrEmpty(customer.AuthoritySignedForm)
                    //    || string.IsNullOrEmpty(customer.AuthoritySignedFormInCustomerLanguage))
                    //    throw new UserException("Authority signature on Customer form is missing");

                    //if (customer.SentToCardIssuerOn == null)
                    //    throw new UserException("Customer is pending to send to card issuer");

                    if (customer.CardIssuerRespondedOn != null && string.IsNullOrEmpty(customer.DeclinedReason))
                        throw new UserException("Customer has already been approved");

                    customer.CardIssuerRespondedOn = DateTime.Now;

                    if (string.IsNullOrEmpty(declinedReason))
                    {
                        customer.BillingAddress = billingAddress;
                        customer.EmailForShopping = emailForShopping;
                        customer.DeclinedReason = declinedReason;
                    }
                    else
                    {
                        customer.DeclinedReason = declinedReason;
                    }

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name,
                        performedForUserId: customerId);

                    if (!string.IsNullOrEmpty(declinedReason))
                        new MessageService().SendCustomerCardIssuerDeclinedMessage(customer, declinedReason,
                            customer.LanguageId);

                    return true;
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

        private bool SetCustomerApprovedOrDeclined(int customerId, string declinedReason
            , string cardServiceProviderClientId)
        {
            if (customerId <= 0)
                throw new ArgumentNullException("Customer Id is invalid");

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

                    if (string.IsNullOrEmpty(customer.CustomerSignedForm)
                        || string.IsNullOrEmpty(customer.CustomerSignedFormInCustomerLanguage))
                        throw new UserException("Customer signed form is missing");

                    if (customer.CardIssuerRespondedOn != null)
                        throw new UserException("Customer either have already been processed or under process");
                    customer.CardIssuerRespondedOn = DateTime.Now;
                    //customer.CardServiceProviderClientId = string.IsNullOrEmpty(cardServiceProviderClientId) ? null : cardServiceProviderClientId;
                    customer.DeclinedReason = string.IsNullOrEmpty(declinedReason) ? null : declinedReason;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    // If dclined then send Customer email with reason
                    if (!string.IsNullOrEmpty(declinedReason))
                        new MessageService().SendCustomerValidationFailedMessage(customer, declinedReason
                            , customer.LanguageId);

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
                        , performedForUserId: customerId);

                    return true;
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

        #endregion Processed (Customers who have been approved or declined)

        /// <summary>
        /// Return Customer (KYC Form) Status
        /// </summary>
        public CustomerStatus GetCustomerStatus(Customer customer)
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
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return 0;
            }
        }

        /// <summary>
        /// Return Customer (KYC Form) Status To Show To Customer
        /// </summary>
        public CustomerStatusForCustomer GetCustomerStatusForCustomer(Customer customer)
        {
            try
            {
                if (customer.CardIssuerRespondedOn != null && string.IsNullOrEmpty(customer.DeclinedReason))
                    return CustomerStatusForCustomer.Approved;
                else if (customer.CardIssuerRespondedOn != null && !string.IsNullOrEmpty(customer.DeclinedReason))
                    return CustomerStatusForCustomer.Failed;
                //if (customer.SentToCardIssuerOn != null)
                //    return CustomerStatusForCustomer.Processing;
                //else if (!string.IsNullOrEmpty(customer.AuthoritySignedForm))
                //return CustomerStatusForCustomer.Processing;
                else if (customer.ValidatedOn != null && string.IsNullOrEmpty(customer.ValidationFailureReason))
                    return CustomerStatusForCustomer.Validated;
                else if (customer.ValidatedOn != null && !string.IsNullOrEmpty(customer.ValidationFailureReason))
                    return CustomerStatusForCustomer.ValidationFailed;
                else
                    return CustomerStatusForCustomer.Pending;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return 0;
            }
        }

        /// <summary>
        /// Return Customer (KYC Form) Status String
        /// </summary>
        public void GetCustomerStatusString(Customer customer, bool isForCustomer)
        {
            int totalCount = 0;
            try
            {
                if (isForCustomer)
                {
                    if (customer.CardRequestForms == null)
                        customer.CardRequestForms = new CardService().GetCardRequestForms(out totalCount, customer.UserId);

                    if (customer.CardRequestForms == null || customer.CardRequestForms.Count == 0)
                        customer.StatusString = "Pending Card Requests";
                    else if (string.IsNullOrEmpty(customer.CustomerSignedForm))
                        customer.StatusString = "Pending For Customer Signed Form";
                    else if (customer.ValidatedOn != null && !string.IsNullOrEmpty(customer.ValidationFailureReason))
                        customer.StatusString = "Validation Failed";
                    else if (((!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
                              //&& string.IsNullOrEmpty(customer.ProofOfAddressDoc)
                              )
                             || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
                                 //&& string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)
                                 ))
                                 && (customer.CardIssuerRespondedOn != null
                                 && !string.IsNullOrEmpty(customer.DeclinedReason))

                                 )
                        customer.StatusString = "Processing";
                    //else if (string.IsNullOrEmpty(customer.AuthoritySignedForm))
                    //    customer.StatusString = "Processing";
                    //else if (customer.SentToCardIssuerOn == null)
                    //    customer.StatusString = "Processing";
                    else if (customer.CardIssuerRespondedOn != null && string.IsNullOrEmpty(customer.DeclinedReason))
                        customer.StatusString = "Approved";
                    else if (customer.CardIssuerRespondedOn != null && !string.IsNullOrEmpty(customer.DeclinedReason))
                        customer.StatusString = "Declined";
                    //else if (customer.SentToCardIssuerOn != null)
                    //    customer.StatusString = "Processing";
                    else if (customer.CardRequestForms != null && customer.CardRequestForms[0]
                        .Payments.Any(pay => pay.ConfirmedOn == null &&
                        string.IsNullOrEmpty(pay.ConfirmationFailureReason)
                        && pay.PaymentMethod != PaymentMethod.RAHYAB
                        && pay.PaymentMethod != PaymentMethod.Admin))
                        customer.StatusString = "Waiting For Payment Confirmation";
                    else if (customer.CardRequestForms != null && customer.CardRequestForms[0]
                        .Payments.Any(pay => pay.ConfirmedOn != null &&
                        !string.IsNullOrEmpty(pay.ConfirmationFailureReason)
                        && pay.PaymentMethod != PaymentMethod.RAHYAB
                        && pay.PaymentMethod != PaymentMethod.Admin))
                        customer.StatusString = "Payment Confirmation Failed";
                    else if (customer.CardRequestForms != null && customer.CardRequestForms[0]
                       .Payments.Any(pay => pay.ReConfirmedOn == null &&
                       string.IsNullOrEmpty(pay.ConfirmationFailureReason)
                       && pay.PaymentMethod != PaymentMethod.RAHYAB
                       && pay.PaymentMethod != PaymentMethod.Admin))
                        customer.StatusString = "Waiting For Payment Re-confirmation";
                    else if (customer.CardRequestForms != null && customer.CardRequestForms[0]
                       .Payments.Any(pay => pay.ReConfirmedOn != null &&
                       !string.IsNullOrEmpty(pay.ConfirmationFailureReason) &&
                       pay.ConfirmedOn != null
                       && pay.PaymentMethod != PaymentMethod.RAHYAB
                       && pay.PaymentMethod != PaymentMethod.Admin))
                        customer.StatusString = "Waiting For Payment Re-confirmation";
                    else if (customer.CardRequestForms != null && (customer.CardRequestForms[0]
                       .Payments.Any(pay => pay.ReConfirmedOn != null &&
                       string.IsNullOrEmpty(pay.ConfirmationFailureReason)
                       && pay.PaymentMethod != PaymentMethod.RAHYAB
                       && pay.PaymentMethod != PaymentMethod.Admin) && customer.CardRequestForms[0]
                       .Payments.Any(pay => pay.ConfirmedOn != null &&
                       string.IsNullOrEmpty(pay.ConfirmationFailureReason)
                       && pay.PaymentMethod != PaymentMethod.RAHYAB
                       && pay.PaymentMethod != PaymentMethod.Admin)) && (customer.ValidatedOn == null
                       && string.IsNullOrEmpty(customer.ValidationFailureReason)))
                        customer.StatusString = "Waiting for Validation";
                    else if ((customer.ValidatedOn != null && string.IsNullOrEmpty(customer.ValidationFailureReason))
                                && (customer.CardIssuerRespondedOn == null && customer.DeclinedReason == null))
                        customer.StatusString = "Waiting for Approval";
                    else
                        customer.StatusString = "Unknown";
                }
                else
                {
                    if (customer.CardRequestForms == null)
                        customer.CardRequestForms = new CardService().GetCardRequestForms(out totalCount, customer.UserId);

                    if (customer.CardRequestForms == null || customer.CardRequestForms.Count == 0)
                        customer.StatusString = "Waiting For Card Requests";
                    else if (string.IsNullOrEmpty(customer.CustomerSignedForm))
                        customer.StatusString = "Waiting For Customer Signed Form";
                    else if (customer.CardRequestForms != null && customer.CardRequestForms[0]
                        .Payments.Any(pay => pay.ConfirmedOn == null &&
                        string.IsNullOrEmpty(pay.ConfirmationFailureReason)
                        && pay.PaymentMethod != PaymentMethod.RAHYAB
                        && pay.PaymentMethod != PaymentMethod.Admin))
                        customer.StatusString = "Waiting For Payment Confirmation";
                    else if (customer.CardRequestForms != null && customer.CardRequestForms[0]
                       .Payments.Any(pay => pay.ReConfirmedOn == null &&
                       string.IsNullOrEmpty(pay.ConfirmationFailureReason)
                       && pay.PaymentMethod != PaymentMethod.RAHYAB
                       && pay.PaymentMethod != PaymentMethod.Admin))
                        customer.StatusString = "Waiting For Payment Re-confirmation";
                    else if (customer.CardRequestForms != null && customer.CardRequestForms[0]
                       .Payments.Any(pay => pay.ConfirmedOn != null && pay.ReConfirmedOn == null &&
                       !string.IsNullOrEmpty(pay.ConfirmationFailureReason)
                       && pay.PaymentMethod != PaymentMethod.RAHYAB
                       && pay.PaymentMethod != PaymentMethod.Admin))
                        customer.StatusString = "Waiting For Payment Confirmation";
                    else if (customer.ValidatedOn == null)
                        customer.StatusString = "Waiting For Validation";
                    else if (customer.ValidatedOn != null && !string.IsNullOrEmpty(customer.ValidationFailureReason))
                        customer.StatusString = "Validation Failed";
                    else if ((customer.ValidatedOn != null && string.IsNullOrEmpty(customer.ValidationFailureReason))
                                && (customer.CardIssuerRespondedOn == null && customer.DeclinedReason == null))
                        customer.StatusString = "Waiting for Approval";
                    //else if ((!string.IsNullOrEmpty(customer.ProofOfAddressDocInCustomerLanguage)
                    //          && string.IsNullOrEmpty(customer.ProofOfAddressDoc)
                    //          )
                    //         || (!string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDocInCustomerLanguage)
                    //             && string.IsNullOrEmpty(customer.ProofOfSourceOfFundsDoc)
                    //             ))
                    //    customer.StatusString = "Waiting For Translation";
                    //else if (string.IsNullOrEmpty(customer.AuthoritySignedForm))
                    //    customer.StatusString = "Waiting For Authority Signed Form";
                    //else if (customer.SentToCardIssuerOn == null)
                    //    customer.StatusString = "Waiting To Send To Card Issuer";
                    else if (customer.CardIssuerRespondedOn != null && string.IsNullOrEmpty(customer.DeclinedReason))
                        customer.StatusString = "Approved";
                    else if (customer.CardIssuerRespondedOn != null && !string.IsNullOrEmpty(customer.DeclinedReason))
                        customer.StatusString = "Declined";
                    //else if (customer.SentToCardIssuerOn != null)
                    //    customer.StatusString = "Waiting For Card Issuer Response";
                    else
                        customer.StatusString = "Unknown";
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
            }
        }

        /// <summary>
        /// Will use for Dashboard
        /// </summary>
        public int GetCustomersCountByStatus(CustomerStatus customerStatus)
        {
            int totalCount = 0;
            return GetCustomers(out totalCount, customerStatus: customerStatus).Count;
        }

        public List<SecurityQuestion> GetSecurityQuestions()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var securityQuestions = context.SecurityQuestions.ToList();

                    if (securityQuestions.Count > 0)
                    {
                        return securityQuestions;
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

        public bool SaveSecurityQuestion(int userId, string question, string answer)
        {
            if (userId <= 0)
                throw new ArgumentNullException("User Id is required");

            if (string.IsNullOrEmpty(question))
                throw new ArgumentNullException("Question is required");

            if (string.IsNullOrEmpty(answer))
                throw new ArgumentNullException("Answer is required");

            try
            {
                using (var context = new EntityContext())
                {
                    var customer = (from u in context.Users
                                    join c in context.Customers on u.Id equals c.UserId
                                    where u.Id == userId
                                    select new { User = u, Customer = c }).FirstOrDefault();

                    if (customer == null)
                        return false;

                    var commonService = new CommonService();
                    customer.Customer.SecurityQuestion = commonService.EncodeText(question, customer.User.HashKey);
                    customer.Customer.Answer = commonService.EncodeText(answer, customer.User.HashKey);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);
                    new MessageService().SendSecurityQuestionMessage(customer.User, customer.Customer.LanguageId);
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

        public bool ChangePassword(string oldPassword, string newPassword, int userId)
        {
            var commonService = new CommonService();
            using (var context = new EntityContext())
            {
                try
                {
                    User user = context.Users.SingleOrDefault(u => u.Id == userId);

                    if (user == null)
                        return false;

                    if (commonService.EncodeText(oldPassword, user.HashKey).ToString() == user.HashedPassword)
                    {
                        user.HashedPassword = commonService.EncodeText(newPassword, user.HashKey).ToString();
                        user.ChangePasswordRequired = false;

                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to save");

                        new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
                                                    , performedForUserId: user.Id);

                        new MessageService().SendChangePasswordMessage(user, new SettingService()
                                                                       .CustomerDefaultLanguage.Id);

                        return true;
                    }
                    return false;
                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return false;
                }
            }
        }
        public bool ChangePassword(string newPassword, string userEmail)
        {
            var commonService = new CommonService();
            using (var context = new EntityContext())
            {
                try
                {
                    User user = context.Users.SingleOrDefault(u => u.Email.Equals(userEmail));
                    if (user == null)
                        return false;

                    user.HashedPassword = commonService.EncodeText(newPassword, user.HashKey).ToString();
                    user.ChangePasswordRequired = false;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
                                                    , performedForUserId: user.Id);
                    new MessageService().SendChangePasswordMessage(user, new SettingService().CustomerDefaultLanguage.Id);
                    return true;
                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return false;
                }
            }
        }

        /// <summary>
        /// Validate Customer before adding in Database
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public void ValidateCustomer(Customer customer)
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

        public bool ActivateCustomer(int userId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var user = (from u in context.Users
                                where u.Id == userId
                                select u).SingleOrDefault();

                    if (user == null)
                        throw new UserException("Customer not found");

                    if (user.Status == (int)UserStatus.Active)
                        throw new UserException("Customer is already activated");

                    user.Status = (int)UserStatus.Active;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Activation failed, please try again later.");

                    return true;
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

        /// <summary>
        /// This method is used to send verification code to customer and 
        /// </summary>
        public bool SendCustomerForgotPasswordCode(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("Email is required");
            try
            {
                using (var context = new EntityContext())
                {
                    var customer = (from u in context.Users
                                    join c in context.Customers on u.Id equals c.UserId
                                    where u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                    && u.Status == (int)UserStatus.Active
                                    select new { Customer = c, User = u }).SingleOrDefault();
                    if (customer.Customer != null)
                    {
                        customer.Customer.User = customer.User;
                        customer.Customer.VerificationCode = new CommonService().GetVerificationCode();
                        customer.Customer.VerificationCodeExpireOn = DateTime.Now.AddHours(1);

                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to update");
                        new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                        // Send Customer Welcome Message

                        new MessageService().SendForgotPasswordCodeMessage(customer.Customer, new SettingService().CustomerDefaultLanguage.Id);
                    }
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
            return true;
        }

        public bool ResetCustomerForgotPassword(string email, string verificationCode, string password)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("Email is required");
            if (string.IsNullOrEmpty(verificationCode))
                throw new ArgumentNullException("verificationCode is required");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Invalid Password.");
            try
            {
                using (var context = new EntityContext())
                {
                    var customer = (from u in context.Users
                                    join c in context.Customers on u.Id equals c.UserId
                                    where u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                    && u.Status == (int)UserStatus.Active
                                    select new { Customer = c, User = u }).SingleOrDefault();
                    if (customer.Customer != null)
                    {
                        if (customer.Customer.VerificationCode != verificationCode)
                            throw new UserException("Invalid Verification Code!");

                        int result = DateTime.Compare(Convert.ToDateTime(customer.Customer.VerificationCodeExpireOn), DateTime.Now);
                        if (result < 0 || result == 0)
                            throw new UserException("Verification Code Expired!");

                        var commonService = new CommonService();
                        var newPass = password;

                        var hashKey = commonService.GeneratePassword(6);
                        customer.User.HashKey = hashKey;
                        customer.User.HashedPassword = commonService.EncodeText(password, hashKey);

                        customer.Customer.VerificationCode = null;
                        customer.Customer.VerificationCodeExpireOn = null;
                        customer.User.ChangePasswordRequired = false;

                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to update");
                        new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    }
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Discount check against the Affiliated customer
        /// </summary>

        public List<AffiliateDiscount> GetAffiliateDiscounts(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            try
            {
                using (var context = new EntityContext())
                {
                    if (customer.AffiliateId != null)
                    {
                        var result = (from query in context.AffiliateDiscounts
                                      where query.AffiliateId == customer.AffiliateId
                                      && query.Active
                                      && (DateTime.Now >= query.StartOn && DateTime.Now <= query.EndOn)
                                      select query).ToList();
                        return result;
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
    }
}

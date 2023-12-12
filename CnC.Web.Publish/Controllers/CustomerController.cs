using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Cards;
using CnC.Core.Customers;
using CnC.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Dev;
using CnC.Core.Payments;
using System.Net.Mail;
using System.Net;
using System.Drawing.Imaging;
using System.Drawing;

namespace CnC.Web.Controllers
{
    [Authorize]
    [RoleActionValidator]
    public class CustomerController : Controller
    {


        #region Security Questions

        public List<SecurityQuestion> GetSecurityQuestions()
        {
            return new CustomerService().GetSecurityQuestions();
        }

        public ActionResult SecurityQuestions(int? customerId = null)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SecurityQuestions(FormCollection formCollection)
        {
            var userSession = Session["CurrentSession"] as CnC.Core.Accounts.User;

            int questionId = 0;
            if (!string.IsNullOrEmpty(formCollection["SecurityQuestionDDL"].ToString()))
            {
                questionId = int.Parse(formCollection["SecurityQuestionDDL"]);
            }

            string question = string.Empty;
            if (!string.IsNullOrEmpty(formCollection["questionText"].ToString()))
            {
                question = formCollection["questionText"].ToString();
            }

            string answer = null;
            if (!string.IsNullOrEmpty(formCollection["answer"].ToString()))
            {
                answer = Convert.ToString(formCollection["answer"]);
            }

            try
            {
                bool isSaved = userSession != null && new CustomerService().SaveSecurityQuestion(userSession.Id, question, answer);

                if (isSaved)
                {
                    ViewBag.ResponseCode = "200";
                    ViewBag.ResponseMessage = "Updated successfully!";
                }
                else
                {
                    ViewBag.ResponseCode = "101";
                    ViewBag.ResponseMessage = "Failed to save security question. Please try again.";
                }
            }
            catch (Exception exp)
            {
                ViewBag.ResponseCode = "101";
                ViewBag.ResponseMessage = exp.Message;
            }

            return View();
        }

        #endregion Security Questions

        #region KYC Form
        public ActionResult KYCForm()
        {
            return View();
        }
        public ActionResult KYCForms()
        {
            var kycForms = new CustomerService().GetCustomers();
            return View(kycForms);
        }

        [HttpPost]
        public ActionResult KYCForms(FormCollection formCollection)
        {
            string email = Convert.ToString(formCollection["txtEmail"]);
            string nic = Convert.ToString(formCollection["txtNICNo"]);
            string passportNo = Convert.ToString(formCollection["txtPassportNo"]);
            var requestFormStatus = (formCollection["RequestFormStatus"]);

            //TODO: status is pending to search
            int status = 9;
            if (!string.IsNullOrEmpty(requestFormStatus))
                status = int.Parse(formCollection["RequestFormStatus"]);

            if (true)
            {

            }

            var searchKycForm = new CustomerService().GetCustomers(requestFormStatus);

            return View(searchKycForm);
        }

        public ActionResult UploadSignedKYCForm()
        {
            //later we will make a common method for all actions

            var kycForms = new CustomerService().GetCustomer(1);
            return View("KYCForms", kycForms);
        }



        //[HttpPost]
        //public JsonResult NewCustomer(FormCollection formCollection, HttpPostedFileBase customerSignedCopyEng,
        //    HttpPostedFileBase NICDocCustomer, HttpPostedFileBase ProofOfAddressDocCutomer,
        //    HttpPostedFileBase PassportDocCustomer, IEnumerable<HttpPostedFileBase> proofOfSourceOfFundsCl,
        //    HttpPostedFileBase customerSignedCopyCl, string hfCustomerId, string proofOfAddressType, string sourcceOfFundType, string hfsubmitActionName)
        //{

        //    // getting two docs in step1 
        //    // 1. NIC Doc In Customer Language
        //    // 2. Passport Doc in Customer Language

        //    // getting 5 docs in second step
        //    // CustomerId
        //    // 1.Proof Of Address with Type
        //    // 2.Source Of Funds Proof 4 docs includign type

        //    // Step3.
        //    // CustomerId
        //    // Get customer signed copy(Eng + Persian)

        //    var statusArray = new string[3];
        //    string email = Request.Form["txtEmail"];
        //    var user = new User();
        //    user.Email = email;
        //    user.Username = user.Email;
        //    user.Status = (int)UserStatus.Active;
        //    if (new UserService().GetUser(user.Email) != null)
        //    {
        //        // Show error message that user already exist
        //        statusArray[0] = "101";
        //        statusArray[1] = "Email already exists!";
        //        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    }
        //    var settingService = new SettingService();
        //    var documentService = new DocumentService();
        //    int defaultCurrencyId = settingService.CustomerDefaultCurrency.Id;
        //    //string fileUploadPath = ConfigurationManager.AppSettings["Upload"];

        //    string engFirstName = Request.Form["txtEngFirstName"];
        //    string engLastName = Request.Form["txtEngLastName"];
        //    string engDateOfBirth = Request.Form["txtEngDateOfBirth"];
        //    string engAddress1 = Request.Form["txtEngAddress"];
        //    string engAddress2 = Request.Form["txtEngAddress2"];

        //    string perFirstName = Request.Form["txtPerFirstName"];
        //    string perLastName = Request.Form["txtPerLastName"];
        //    string perDateOfBirth = Request.Form["txtPerDateOfBirth"];
        //    string perAddress1 = Request.Form["txtPerAddress"];
        //    string perAddress2 = Request.Form["txtPerAddress2"];

        //    string cityId = Request.Form["ddlCity"];
        //    string contactNo = Request.Form["txtContactNo"];
        //    string countryId = Request.Form["ddlCountry"];
        //    string gender = Request.Form["ddlGender"];
        //    string maritalStatus = Request.Form["ddlMaritalStatus"];
        //    string nationality = Request.Form["txtNationality"];
        //    string nic = Request.Form["txtNIC"];
        //    string passportNo = Request.Form["txtPassportNo"];
        //    string postalCode = Request.Form["txtPostalCode"];

        //    string cardId = Request.Form["cardId"];
        //    string cardQuantity = Request.Form["CardQty"];

        //    var proofOfSourceOfFundsImagesCl = proofOfSourceOfFundsCl as IList<HttpPostedFileBase> ?? proofOfSourceOfFundsCl.ToList();
        //    if (NICDocCustomer != null && PassportDocCustomer != null && !string.IsNullOrEmpty(perFirstName) && !string.IsNullOrEmpty(engFirstName) && !string.IsNullOrEmpty(email))//formCollection.AllKeys.Contains("btnSaveInfoStep1"))
        //    {
        //        #region Step1

        //        if (true)
        //        {

        //            user.FirstName = engFirstName;
        //            user.LastName = engLastName;
        //            user.FirstNameInCustomerLanguage = perFirstName;
        //            user.LastNameInCustomerLanguage = perLastName;

        //            var customer = new Customer();
        //            customer.DateOfBirth = Convert.ToDateTime(engDateOfBirth);//formCollection["txtEngDateOfBirth"]
        //            customer.Address = engAddress1;//Convert.ToString(formCollection["txtEngAddress"]);
        //            customer.Address2 = engAddress2;//Convert.ToString(formCollection["txtEngAddress2"]);

        //            customer.DateOfBirthInCustomerLanguage = perDateOfBirth;//formCollection["txtPerDateOfBirth"];
        //            customer.AddressInCustomerLanguage = perAddress1;//Convert.ToString(formCollection["txtPerAddress"]);
        //            customer.Address2InCustomerLanguage = perAddress2;//Convert.ToString(formCollection["txtPerAddress2"]);

        //            customer.CityId = int.Parse(cityId); //formCollection["ddlCity"]
        //            customer.ContactNo = contactNo;//Convert.ToString(formCollection["txtContactNo"]);
        //            customer.CountryId = int.Parse(countryId);//formCollection["ddlCountry"]
        //            customer.GenderId = int.Parse(gender);//formCollection["ddlGender"]
        //            customer.LanguageId = settingService.CustomerDefaultLanguage.Id;
        //            customer.MaritalStatusId = int.Parse(maritalStatus);//formCollection["ddlMaritalStatus"]
        //            customer.Nationality = nationality;//Convert.ToString(formCollection["txtNationality"]);
        //            customer.NIC = nic;//Convert.ToString(formCollection["txtNIC"]);
        //            customer.PassportNo = passportNo;//Convert.ToString(formCollection["txtPassportNo"]);
        //            customer.PostalCode = postalCode;//Convert.ToString(formCollection["txtPostalCode"]);
        //            customer.CurrencyId = defaultCurrencyId;
        //            if (PassportDocCustomer.ContentLength > 0)
        //            {
        //                string extension = Path.GetExtension(PassportDocCustomer.FileName);
        //                string newFileName = customer.NIC + "-PassportDocCustomer" + extension;
        //                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                PassportDocCustomer.SaveAs(path);
        //                customer.PassportDocInCustomerLanguage = newFileName;
        //            }
        //            if (NICDocCustomer.ContentLength > 0)
        //            {
        //                string extension = Path.GetExtension(NICDocCustomer.FileName);
        //                string newFileName = customer.NIC + "-NICDocCustomer" + extension;
        //                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                NICDocCustomer.SaveAs(path);
        //                customer.NICDocInCustomerLanguage = newFileName;
        //            }
        //            var customerService = new CustomerService();
        //            try
        //            {
        //                int res = customerService.AddCustomer(customer, user);
        //                if (res > 0)
        //                {
        //                    statusArray[0] = "200";
        //                    statusArray[1] = "" + res;
        //                    statusArray[2] = "Step2";
        //                    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            catch (Exception exp)
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "Sorry we are unable to process your request due to following exception " + exp.Message;

        //            }

        //        }
        //        else
        //        {
        //            statusArray[0] = "101";
        //            statusArray[1] = "Missing required information.";
        //        }

        //        #endregion
        //    }
        //    else if (!string.IsNullOrEmpty(cardId) && !string.IsNullOrEmpty(cardQuantity) && customerSignedCopyEng == null && customerSignedCopyCl == null)//formCollection.AllKeys.Contains("btnSaveInfoStep2"))
        //    {
        //        //ignore in case of GOlD, Platinum card
        //        // 1.Proof Of Address with Type
        //        // 2.Source Of Funds Proof 4 docs includign type

        //        #region Step2
        //        var listOfSourceOfFundsDocs = new List<string>();
        //        string proofOfAddresDoc = string.Empty;
        //        var typesOfCards = new CardService().GetCardTypes();
        //        var arrCardTypeIds = formCollection["cardId"].Split(',');
        //        var card = typesOfCards
        //            .SingleOrDefault(ct => ct.IsProofOfSourceOfFundsRequired && ct.IsProofOfAddressRequired);
        //        if (card != null)
        //        {
        //            var isDocsRequired = arrCardTypeIds.Count(x => x.Equals("" + card.Id));
        //            if (isDocsRequired > 0)
        //            {
        //                if (string.IsNullOrEmpty(sourcceOfFundType) && string.IsNullOrEmpty(proofOfAddressType) && ProofOfAddressDocCutomer == null && !proofOfSourceOfFundsImagesCl.Any())
        //                {
        //                    //return error docs are required
        //                    statusArray[0] = "101";
        //                    statusArray[1] = "Missing required documents.";
        //                    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(hfCustomerId) && !string.IsNullOrEmpty(proofOfAddressType) && !string.IsNullOrEmpty(sourcceOfFundType))
        //        {
        //            var cardRequests = new List<CardRequest>();
        //            var arrCardQty = formCollection["CardQty"].Split(',');
        //            bool cardPurchased = false;
        //            decimal totalAmount = 0;

        //            for (var i = 0; i < typesOfCards.Count; i++)
        //            {
        //                var cardQty = int.Parse(arrCardQty[i]);
        //                var cardTypeId = int.Parse(arrCardTypeIds[i]);
        //                if (cardQty <= 0) continue;
        //                cardPurchased = true;
        //                for (var j = 0; j < cardQty; j++)
        //                {
        //                    var cr = new CardRequest();
        //                    cr.CardQty = cardQty;
        //                    cr.CardTypeId = cardTypeId;
        //                    cr.Fee = (new CardService().GetCardType(cardTypeId).Fee) * cardQty;
        //                    cardRequests.Add(cr);
        //                    totalAmount = totalAmount + cr.Fee;
        //                }
        //            }

        //            // Request Form info
        //            var requestForm = new RequestForm();
        //            requestForm.CustomerId = Convert.ToInt32(hfCustomerId);
        //            requestForm.Type = RequestFormType.Card;
        //            requestForm.TypeId = (int)RequestFormType.Card;
        //            requestForm.ExchangeRate = new ExchangeRateService().GetExchangeRate(defaultCurrencyId).Rate;
        //            requestForm.ServiceFee = new ServiceFeeService().GetServiceFee(ServiceType.Card).Percentage;
        //            // Proof of source of funds doc upload

        //            var customer = new CustomerService().GetCustomer(Convert.ToInt32(hfCustomerId));

        //            if (ProofOfAddressDocCutomer != null)
        //            {
        //                string extension = Path.GetExtension(ProofOfAddressDocCutomer.FileName);
        //                string newFileName = customer.NIC + "-ProofOfAddressDocCutomer" + extension;
        //                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                ProofOfAddressDocCutomer.SaveAs(path);
        //                proofOfAddresDoc = newFileName;
        //                //customer.ProofOfSourceOfFundsDocInCustomerLanguage = customer.ProofOfSourceOfFundsDocInCustomerLanguage + newFileName + ";";

        //            }
        //            if (proofOfSourceOfFundsImagesCl.Any())
        //            {
        //                int flagCounter = 0;
        //                foreach (var proofOfSourceOfFundsImageCustomer in proofOfSourceOfFundsImagesCl)
        //                {
        //                    string extension = Path.GetExtension(proofOfSourceOfFundsImageCustomer.FileName);
        //                    string newFileName = customer.NIC + "-ProofOfSourceOfFundsCL" + flagCounter + extension;
        //                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                    proofOfSourceOfFundsImageCustomer.SaveAs(path);
        //                    listOfSourceOfFundsDocs.Add(newFileName);
        //                    // customer.ProofOfSourceOfFundsDocInCustomerLanguage = customer.ProofOfSourceOfFundsDocInCustomerLanguage + newFileName + ";";
        //                    // flagCounter++;
        //                }
        //            }



        //            // Payment info
        //            var payments = new List<Payment>
        //        {
        //            new Payment()
        //            {
        //                Amount = totalAmount,
        //                CreatedOn = DateTime.UtcNow,
        //                PaymentMethod = PaymentMethod.RAHYAB,
        //                PaymentMethodId = (int) PaymentMethod.RAHYAB
        //            }
        //        };

        //            if (cardPurchased)
        //            {
        //                try
        //                {
        //                    var res = new CardService().AddCardRequestForm(requestForm, cardRequests, payments, proofOfAddressType, proofOfAddresDoc, sourcceOfFundType, listOfSourceOfFundsDocs);
        //                    if (res > 0)
        //                    {
        //                        statusArray[0] = "200";
        //                        statusArray[1] = "" + hfCustomerId;//res; Send customer id back to client for next step
        //                        statusArray[2] = "Step3";
        //                        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //                catch (Exception exp)
        //                {
        //                    statusArray[0] = "101";
        //                    statusArray[1] = "Sorry we are unable to process your request due to following exception " +
        //                                     exp.Message;

        //                }
        //            }
        //            else
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "No card request found!";
        //            }

        //        }
        //        else
        //        {
        //            statusArray[0] = "101";
        //            statusArray[1] = "Customer related info is not found!";
        //        }

        //        #endregion
        //    }
        //    else if (!string.IsNullOrEmpty(hfCustomerId) && customerSignedCopyEng != null && customerSignedCopyCl != null )//formCollection.AllKeys.Contains("btnSaveInfoStep3"))
        //    {
        //        #region step3
        //        if (true)
        //        {
        //            var customerService = new CustomerService();
        //            var customer = customerService.GetCustomer(Convert.ToInt32(hfCustomerId));
        //            string customerSignedCopyPath = string.Empty;
        //            string customerSignedCopyClPath = string.Empty;
        //            if (customerSignedCopyEng.ContentLength > 0)
        //            {
        //                string extension = Path.GetExtension(customerSignedCopyEng.FileName);
        //                string newFileName = customer.NIC + "-CustomerSignedCopy" + extension;
        //                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                customerSignedCopyEng.SaveAs(path);
        //                customerSignedCopyPath = newFileName;
        //                //customer.NICDocInCustomerLanguage = newFileName;
        //            }
        //            else
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "Invalid File. Please try again.";
        //            }
        //            if (customerSignedCopyCl.ContentLength > 0)
        //            {
        //                string extension = Path.GetExtension(customerSignedCopyCl.FileName);
        //                string newFileName = customer.NIC + "-NICDocCustomer" + extension;
        //                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                customerSignedCopyCl.SaveAs(path);
        //                customerSignedCopyClPath = newFileName;
        //                //customer.NICDocInCustomerLanguage = newFileName;
        //            }
        //            else
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "Invalid File. Please try again.";
        //            }


        //            try
        //            {
        //                var res = customerService.UploadCustomerSignedForm(Convert.ToInt32(hfCustomerId), customerSignedCopyPath, customerSignedCopyClPath);
        //                if (res)
        //                {
        //                    statusArray[0] = "200";
        //                    statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />KYC form has been saved successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='javascript:window.location.href=window.location.href'>Go back</a></span>";
        //                    statusArray[2] = "";
        //                    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            catch (Exception exp)
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "Sorry we are unable to process your request due to following exception " +
        //                                 exp.Message;
        //            }
        //        }
        //        else
        //        {
        //            statusArray[0] = "101";
        //            statusArray[1] = "Missing required information.";
        //        }
        //        #endregion
        //    }
        //    else
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Unable to process request. Please try again. ";

        //    }

        //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //}


        #region Rahyab (Include New Customer)


        #region New Customer

        [HttpGet]
        public ActionResult NewCustomer()
        {
            return View();
        }
        [HttpPost]
        public JsonResult NewCustomer(FormCollection formCollection, HttpPostedFileBase NICDocCustomer, HttpPostedFileBase PassportDocCustomer)
        {

            // getting two docs in step1 
            // 1. NIC Doc In Customer Language
            // 2. Passport Doc in Customer Language

            // getting 5 docs in second step
            // CustomerId
            // 1.Proof Of Address with Type
            // 2.Source Of Funds Proof 4 docs includign type

            // Step3.
            // CustomerId
            // Get customer signed copy(Eng + Persian)

            var statusArray = new string[3];
            string email = Request.Form["txtEmail"];
            var user = new User();
            user.Email = email;
            user.Username = user.Email;
            user.Status = (int)UserStatus.Active;
            if (new UserService().GetUser(user.Email) != null)
            {
                // Show error message that user already exist
                statusArray[0] = "101";
                statusArray[1] = "Email already exists!";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            var settingService = new SettingService();
            var documentService = new DocumentService();
            int defaultCurrencyId = settingService.CustomerDefaultCurrency.Id;
            //string fileUploadPath = ConfigurationManager.AppSettings["Upload"];

            string engFirstName = Request.Form["txtEngFirstName"];
            string engLastName = Request.Form["txtEngLastName"];
            string engDateOfBirth = Request.Form["txtEngDateOfBirth"];
            string engAddress1 = Request.Form["txtEngAddress"];
            string engAddress2 = Request.Form["txtEngAddress2"];

            string perFirstName = Request.Form["txtPerFirstName"];
            string perLastName = Request.Form["txtPerLastName"];
            string perDateOfBirth = Request.Form["txtPerDateOfBirth"];
            string perAddress1 = Request.Form["txtPerAddress"];
            string perAddress2 = Request.Form["txtPerAddress2"];

            string cityId = Request.Form["ddlCity"];
            string contactNo = Request.Form["txtContactNo"];
            string countryId = Request.Form["ddlCountry"];
            string gender = Request.Form["ddlGender"];
            string maritalStatus = Request.Form["ddlMaritalStatus"];
            string nationality = Request.Form["txtNationality"];
            string nic = Request.Form["txtNIC"];
            string passportNo = Request.Form["txtPassportNo"];
            string postalCode = Request.Form["txtPostalCode"];
            var utility = new Utility();
            if (string.IsNullOrEmpty(email) && string.IsNullOrWhiteSpace(email))
            {
                statusArray[0] = "101";
                statusArray[1] = "Email is a required field.";
            }
            else if (string.IsNullOrEmpty(engFirstName) && string.IsNullOrWhiteSpace(engFirstName))
            {
                statusArray[0] = "101";
                statusArray[1] = "FirstName in Englih is a required field.";
            }
            else if (string.IsNullOrEmpty(engLastName) && string.IsNullOrWhiteSpace(engLastName))
            {
                statusArray[0] = "101";
                statusArray[1] = "LastName in English is a required field.";
            }
            else if (string.IsNullOrEmpty(engDateOfBirth) && string.IsNullOrWhiteSpace(engDateOfBirth))
            {
                statusArray[0] = "101";
                statusArray[1] = "DateOfBirth is a required field.";
            }
            else if (string.IsNullOrEmpty(engAddress1) && string.IsNullOrWhiteSpace(engAddress1))
            {
                statusArray[0] = "101";
                statusArray[1] = "Address1 is a required field.";
            }
            else if (string.IsNullOrEmpty(perFirstName) && string.IsNullOrWhiteSpace(perFirstName))
            {
                statusArray[0] = "101";
                statusArray[1] = "FirstName in Persian is a required field.";
            }
            else if (string.IsNullOrEmpty(perLastName) && string.IsNullOrWhiteSpace(perLastName))
            {
                statusArray[0] = "101";
                statusArray[1] = "LastName in Persian is a required field.";
            }
            else if (string.IsNullOrEmpty(perDateOfBirth) && string.IsNullOrWhiteSpace(perDateOfBirth))
            {
                statusArray[0] = "101";
                statusArray[1] = "DateOfBirth in Persian is a required field.";
            }
            else if (string.IsNullOrEmpty(perAddress1) && string.IsNullOrWhiteSpace(perAddress1))
            {
                statusArray[0] = "101";
                statusArray[1] = "Address1 in Persian is a required field.";
            }
            else if (string.IsNullOrEmpty(cityId) && string.IsNullOrWhiteSpace(cityId))
            {
                statusArray[0] = "101";
                statusArray[1] = "City is a required field.";
            }
            else if (string.IsNullOrEmpty(contactNo) && string.IsNullOrWhiteSpace(contactNo))
            {
                statusArray[0] = "101";
                statusArray[1] = "ContactNo is a required field.";
            }
            else if (string.IsNullOrEmpty(countryId) && string.IsNullOrWhiteSpace(countryId))
            {
                statusArray[0] = "101";
                statusArray[1] = "Country is a required field.";
            }
            else if (string.IsNullOrEmpty(gender) && string.IsNullOrWhiteSpace(gender))
            {
                statusArray[0] = "101";
                statusArray[1] = "Gender is a required field.";
            }
            else if (string.IsNullOrEmpty(maritalStatus) && string.IsNullOrWhiteSpace(maritalStatus))
            {
                statusArray[0] = "101";
                statusArray[1] = "MaritalStatus is a required field.";
            }
            else if (string.IsNullOrEmpty(nationality) && string.IsNullOrWhiteSpace(nationality))
            {
                statusArray[0] = "101";
                statusArray[1] = "Nationality is a required field.";
            }
            else if (string.IsNullOrEmpty(nic) && string.IsNullOrWhiteSpace(nic))
            {
                statusArray[0] = "101";
                statusArray[1] = "NIC is a required field.";
            }
            else if (string.IsNullOrEmpty(passportNo) && string.IsNullOrWhiteSpace(passportNo))
            {
                statusArray[0] = "101";
                statusArray[1] = "PassportNo is a required field.";
            }
            else if (string.IsNullOrEmpty(postalCode) && string.IsNullOrWhiteSpace(postalCode))
            {
                statusArray[0] = "101";
                statusArray[1] = "NIC is a required field.";
            }
            else if (!utility.IsValidImage(NICDocCustomer))
            {
                statusArray[0] = "101";
                statusArray[1] = "Invalid NIC Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
            }
            else if (!utility.IsValidImage(PassportDocCustomer))
            {
                statusArray[0] = "101";
                statusArray[1] =
                    "Invalid Passport Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
            }
            else
            {

                if (NICDocCustomer != null && PassportDocCustomer != null)
                {
                    #region Step1

                    user.FirstName = engFirstName;
                    user.LastName = engLastName;
                    user.FirstNameInCustomerLanguage = perFirstName;
                    user.LastNameInCustomerLanguage = perLastName;

                    var customer = new Customer();
                    customer.DateOfBirth = Convert.ToDateTime(engDateOfBirth);
                    customer.Address = engAddress1;
                    customer.Address2 = engAddress2;

                    customer.DateOfBirthInCustomerLanguage = perDateOfBirth;
                    customer.AddressInCustomerLanguage = perAddress1;
                    customer.Address2InCustomerLanguage = perAddress2;

                    customer.CityId = int.Parse(cityId);
                    customer.ContactNo = contactNo;
                    customer.CountryId = int.Parse(countryId);
                    customer.GenderId = int.Parse(gender);
                    customer.LanguageId = settingService.CustomerDefaultLanguage.Id;
                    customer.MaritalStatusId = int.Parse(maritalStatus);
                    customer.Nationality = nationality;
                    customer.NIC = nic;
                    customer.PassportNo = passportNo;
                    customer.PostalCode = postalCode;
                    customer.CurrencyId = defaultCurrencyId;
                    if (PassportDocCustomer.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(PassportDocCustomer.FileName);
                        string newFileName = customer.NIC + "-PassportCL" + extension;
                        var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                        PassportDocCustomer.SaveAs(path);
                        customer.PassportDocInCustomerLanguage = newFileName;
                    }
                    if (NICDocCustomer.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(NICDocCustomer.FileName);
                        string newFileName = customer.NIC + "-DocCL" + extension;
                        var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                        NICDocCustomer.SaveAs(path);
                        customer.NICDocInCustomerLanguage = newFileName;
                    }
                    var customerService = new CustomerService();
                    try
                    {
                        int res = customerService.AddCustomer(customer, user);
                        if (res > 0)
                        {
                            statusArray[0] = "200";
                            statusArray[1] = "" + res;
                            statusArray[2] = "Step2";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Unable to process request.";
                        }
                    }
                    catch (Exception exp)
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Sorry we are unable to process your request due to following exception " + exp.Message;

                    }

                    #endregion
                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Missing required information.";
                }

            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NewCustomerCardRequests(string id)
        {
            //Put some validation here on base of Id is it valid or not etc.
            try
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(id, "^[0-9]*$")) return RedirectToAction("NewCustomer", "Customer");
                if (!new CardService().IsCustomerCardRequestFormExist(Convert.ToInt32(id)) && id.Length >= 1 && id.Length <= 15)
                {
                    ViewBag.CustomerId = id;
                    return View();
                }
                return RedirectToAction("NewCustomer", "Customer");
            }
            catch (Exception e)
            {
                return RedirectToAction("NewCustomer", "Customer");
            }
        }

        [HttpPost]
        public JsonResult NewCustomerCardRequests(FormCollection formCollection,
        IEnumerable<HttpPostedFileBase> proofOfAddressDocCl, IEnumerable<HttpPostedFileBase> proofOfSourceOfFundsCl,
        string hfCustomerId, string proofOfAddressType, string sourcceOfFundType, string hfsubmitActionName)
        {
            var statusArray = new string[3];

            string cardTypeIds = Request.Form["cardId"];
            string cardQuantity = Request.Form["CardQty"];
            if (!string.IsNullOrEmpty(cardTypeIds) && !string.IsNullOrEmpty(cardQuantity))
            {
                var settingService = new SettingService();
                var documentService = new DocumentService();
                int defaultCurrencyId = settingService.CustomerDefaultCurrency.Id;
                var proofOfSourceOfFundsImagesCl = proofOfSourceOfFundsCl as IList<HttpPostedFileBase> ?? proofOfSourceOfFundsCl.ToList();
                var proofOfAddressImagesCl = proofOfAddressDocCl as IList<HttpPostedFileBase> ?? proofOfAddressDocCl.ToList();

                #region Step2
                var listOfSourceOfFundsDocs = new List<string>();
                var listOfProofOfAddrss = new List<string>();
                string proofOfAddresDoc = string.Empty;
                var typesOfCards = new CardService().GetCardTypes();
                var arrCardTypeIds = cardTypeIds.Split(',');
                //var card = typesOfCards
                //    .FirstOrDefault(ct => ct.IsProofOfSourceOfFundsRequired && ct.IsProofOfAddressRequired);
                //if (card != null)
                //{
                //    var isDocsRequired = arrCardTypeIds.Count(x => x.Equals("" + card.Id));
                //    if (isDocsRequired > 0)
                //    {
                //        if (string.IsNullOrEmpty(sourcceOfFundType) && string.IsNullOrEmpty(proofOfAddressType) && proofOfAddressDocCutomer == null && !proofOfSourceOfFundsImagesCl.Any())
                //        {
                //            //return error docs are required
                //            statusArray[0] = "101";
                //            statusArray[1] = "Missing required documents.";
                //            return Json(statusArray, JsonRequestBehavior.AllowGet);
                //        }
                //    }
                //}

                if (!string.IsNullOrEmpty(hfCustomerId) && !string.IsNullOrEmpty(proofOfAddressType) && !string.IsNullOrEmpty(sourcceOfFundType))
                {
                    var cardRequests = new List<CardRequest>();
                    var arrCardQty = formCollection["CardQty"].Split(',');
                    bool cardPurchased = false;
                    decimal totalAmount = 0;
                    bool isProofOfAddressRequired = false;
                    bool isProofOfSourceOfFundsRequired = false;

                    for (var i = 0; i < typesOfCards.Count; i++)
                    {
                        var cardQty = int.Parse(arrCardQty[i]);
                        if (cardQty <= 0) continue;
                        cardPurchased = true;
                        var cardTypeId = int.Parse(arrCardTypeIds[i]);
                        if (cardQty > typesOfCards[i].MaxQuantity)
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Applied Card Quantity is greater then allowed.";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }

                        var cr = new CardRequest();
                        cr.CardQty = cardQty;
                        cr.CardTypeId = cardTypeId;
                        cr.Fee = typesOfCards[i].Fee;
                        cardRequests.Add(cr);
                        totalAmount = totalAmount + (cr.Fee * cr.CardQty);

                        if (typesOfCards[i].IsProofOfAddressRequired)
                            isProofOfAddressRequired = typesOfCards[i].IsProofOfAddressRequired;

                        if (typesOfCards[i].IsProofOfSourceOfFundsRequired)
                            isProofOfSourceOfFundsRequired = typesOfCards[i].IsProofOfSourceOfFundsRequired;
                    }

                    // Request Form info
                    var requestForm = new RequestForm();
                    requestForm.CustomerId = Convert.ToInt32(hfCustomerId);
                    requestForm.Type = RequestFormType.Card;
                    requestForm.TypeId = (int)RequestFormType.Card;
                    requestForm.ExchangeRate = new ExchangeRateService().GetExchangeRate(defaultCurrencyId).Rate;
                    // Proof of source of funds doc upload

                    var customer = new CustomerService().GetCustomer(Convert.ToInt32(hfCustomerId));

                    if (isProofOfAddressRequired && !proofOfAddressImagesCl.Any())
                    {
                        // Error
                        statusArray[0] = "101";
                        statusArray[1] = "Missing Required ProofOfAddress Documents.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }

                    if (isProofOfSourceOfFundsRequired && !proofOfSourceOfFundsImagesCl.Any())
                    {
                        // Error
                        statusArray[0] = "101";
                        statusArray[1] = "Missing required documents ProofOfSourceOfFunds";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }

                    if (isProofOfAddressRequired)
                    {
                        int flagAddressCounter = 0;
                        foreach (var proofOfAddressImgCustomer in proofOfAddressImagesCl)
                        {
                            if (proofOfAddressImgCustomer != null)
                            {
                                string extension = Path.GetExtension(proofOfAddressImgCustomer.FileName);
                                string newFileName = customer.NIC + "-AddressCL" + flagAddressCounter + extension;
                                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                                proofOfAddressImgCustomer.SaveAs(path);
                                listOfProofOfAddrss.Add(newFileName);
                                flagAddressCounter++;
                                //proofOfAddresDoc = newFileName;
                                //customer.ProofOfSourceOfFundsDocInCustomerLanguage = customer.ProofOfSourceOfFundsDocInCustomerLanguage + newFileName + ";";

                            }
                        }

                    }

                    if (isProofOfSourceOfFundsRequired)
                    {
                        int flagCounter = 0;
                        foreach (var proofOfSourceOfFundsImageCustomer in proofOfSourceOfFundsImagesCl)
                        {
                            if (proofOfSourceOfFundsImageCustomer != null)
                            {
                                string extension = Path.GetExtension(proofOfSourceOfFundsImageCustomer.FileName);
                                string newFileName = customer.NIC + "-FundsCL" + flagCounter + extension;
                                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                                proofOfSourceOfFundsImageCustomer.SaveAs(path);
                                listOfSourceOfFundsDocs.Add(newFileName);
                                flagCounter++;
                            }
                            // customer.ProofOfSourceOfFundsDocInCustomerLanguage = customer.ProofOfSourceOfFundsDocInCustomerLanguage + newFileName + ";";
                            // flagCounter++;
                        }
                    }

                    // Payment info
                    var payments = new List<Payment>
                    {
                        new Payment()
                        {
                            Amount = totalAmount,
                            CreatedOn = DateTime.UtcNow,
                            PaymentMethod = PaymentMethod.RAHYAB,
                            PaymentMethodId = (int) PaymentMethod.RAHYAB
                        }
                    };

                    if (cardPurchased)
                    {
                        try
                        {
                            var res = new CardService().AddCardRequestForm(requestForm, cardRequests, payments, true, proofOfAddressType, listOfProofOfAddrss, sourcceOfFundType, listOfSourceOfFundsDocs);
                            if (res > 0)
                            {
                                statusArray[0] = "200";
                                statusArray[1] = "" + hfCustomerId;//res; Send customer id back to client for next step
                                statusArray[2] = "Step3";
                                return Json(statusArray, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                statusArray[0] = "101";
                                statusArray[1] = "Unable to process request.";
                            }
                        }
                        catch (Exception exp)
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Sorry we are unable to process your request due to following exception " +
                                             exp.Message;

                        }
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "No card request found!";
                    }

                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Customer related info is not found!";
                }

                #endregion
            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NewCustomerSignedForm(string id)
        {
            try
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(id, "^[0-9]*$")) return RedirectToAction("NewCustomer", "Customer");
                if (new CustomerService().IsCustomerExist(Convert.ToInt32(id)) && id.Length >= 1 && id.Length <= 15)
                {
                    ViewBag.CustomerId = id;
                    return View();
                }
                return RedirectToAction("NewCustomer", "Customer");
            }
            catch (Exception exp)
            {
                return RedirectToAction("NewCustomer", "Customer");
            }
        }

        public JsonResult NewCustomerSignedForm(string hfCustomerId, HttpPostedFileBase customerSignedCopyEng, HttpPostedFileBase customerSignedCopyCl)
        {
            var statusArray = new string[3];
            if (!string.IsNullOrEmpty(hfCustomerId) && customerSignedCopyEng != null && customerSignedCopyCl != null)
            {

                var documentService = new DocumentService();
                var customerService = new CustomerService();
                var customer = customerService.GetCustomer(Convert.ToInt32(hfCustomerId));
                string customerSignedCopyPath = string.Empty;
                string customerSignedCopyClPath = string.Empty;
                if (customerSignedCopyEng.ContentLength > 0)
                {
                    string extension = Path.GetExtension(customerSignedCopyEng.FileName);
                    string newFileName = customer.NIC + "-KYCCustomerSigned" + extension;
                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                    customerSignedCopyEng.SaveAs(path);
                    customerSignedCopyPath = newFileName;
                    //customer.NICDocInCustomerLanguage = newFileName;
                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid File. Please try again.";
                }
                if (customerSignedCopyCl.ContentLength > 0)
                {
                    string extension = Path.GetExtension(customerSignedCopyCl.FileName);
                    string newFileName = customer.NIC + "-KYCCustomerSignedCL" + extension;
                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                    customerSignedCopyCl.SaveAs(path);
                    customerSignedCopyClPath = newFileName;
                    //customer.NICDocInCustomerLanguage = newFileName;
                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid File. Please try again.";
                }


                try
                {
                    var res = customerService.UploadCustomerSignedForm(Convert.ToInt32(hfCustomerId), customerSignedCopyPath, customerSignedCopyClPath);
                    if (res)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />KYC form has been saved successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/NewCustomer'>Go back</a></span>";
                        statusArray[2] = "";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to process request.";
                    }
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry we are unable to process your request due to following exception " +
                                     exp.Message;
                }


            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Missing required info.";
            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region Pending Card Requests


        public ActionResult CardRequests()
        {
            return View(new CustomerService().GetCustomersPendingForCardRequests());
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

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new CustomerService().GetCustomersPendingForCardRequests(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }


        #endregion


        #region CustomerSignedDocument

        public ActionResult CustomerSignedDocument()
        {
            return View(new CustomerService().GetCustomersPendingForSignedForm());
        }

        [HttpPost]
        public ActionResult CustomerSignedDocument(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

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

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new CustomerService().GetCustomersPendingForSignedForm(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        #endregion

        #region Customer Validation


        public ActionResult CustomerValidation()
        {
            return View(new CustomerService().GetCustomersPendingForValidatationOrFailed());
        }

        [HttpPost]
        public ActionResult CustomerValidation(FormCollection formCollection)
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

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new CustomerService().GetCustomersPendingForValidatationOrFailed(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        public JsonResult UpdateCustomerValidateStatus(string key, string failureReason)
        {
            var statusArray = new string[3];
            var customerService = new CustomerService();
            if (!string.IsNullOrEmpty(key) && string.IsNullOrEmpty(failureReason))
            {
                //Completed
                try
                {
                    int customerId = Convert.ToInt32(key);
                    var res = customerService.SetCustomerValidated(Convert.ToInt32(customerId), null);
                    if (res)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CustomerValidation'>Go back</a></span>";
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
            else if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(failureReason))
            {
                // Failure Reason
                try
                {
                    int customerId = Convert.ToInt32(key);
                    var res = customerService.SetCustomerValidated(Convert.ToInt32(customerId), failureReason);
                    if (res)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Failure reason submitted successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CustomerValidation'>Go back</a></span>";
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




        #region Translation

        public ActionResult CustomerTranslation()
        {
            return View(new CustomerService().GetCustomersPendingForTranslation());
        }

        [HttpPost]
        public ActionResult CustomerTranslation(FormCollection formCollection, HttpPostedFileBase proofOfSourceOfAddress, HttpPostedFileBase proofOfSourceOfFunds)
        {
            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new CustomerService().GetCustomersPendingForTranslation(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        [HttpPost]
        public JsonResult CustomerTranslationUploadFiles()
        {
            var statusArray = new string[3];
            var listOfSourceOfFundsDocs = new List<string>();
            var listOfProofOfAddrss = new List<string>();
            try
            {
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var customerService = new CustomerService();
                    var customerId = System.Web.HttpContext.Current.Request["CustId"]; //Put validations
                    var customer = customerService.GetCustomer(Convert.ToInt32(customerId));
                    var documentService = new DocumentService();
                    var allFiles = System.Web.HttpContext.Current.Request.Files;
                    int flagCounterForFunds = 1;
                    int flagCounterForAddress = 1;
                    foreach (var file in allFiles)
                    {
                        //TODO: Name of file is currentyl hardcoded later we will fix 
                        //currently if we don't do this we have to write seperatly for each file
                        if (file.ToString().Contains("SourceOfAddress"))
                        {
                            var proofOfSourceOfAddressScanCopy = System.Web.HttpContext.Current.Request.Files[file.ToString()];
                            if (proofOfSourceOfAddressScanCopy != null && proofOfSourceOfAddressScanCopy.ContentLength > 0)
                            {
                                var utility = new Utility();
                                if (!utility.IsValidImageClientSideUploading(proofOfSourceOfAddressScanCopy))
                                {
                                    statusArray[0] = "101";
                                    statusArray[1] = "Invalid image type or size. Maximum size 1MB format shoulde be .jpg/.Jpeg/.png ";
                                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                                }
                                string extension = Path.GetExtension(proofOfSourceOfAddressScanCopy.FileName);
                                string newFileName = customer.NIC + "-Address" + flagCounterForAddress + extension;
                                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                                proofOfSourceOfAddressScanCopy.SaveAs(path);
                                listOfProofOfAddrss.Add(newFileName);
                                flagCounterForAddress++;
                            }

                        }
                        else if (file.ToString().Contains("SourceOfFunds"))
                        {
                            var proofOfSourceOfFundsScanCopy =
                               System.Web.HttpContext.Current.Request.Files[file.ToString()];
                            var utility = new Utility();
                            if (!utility.IsValidImageClientSideUploading(proofOfSourceOfFundsScanCopy))
                            {
                                statusArray[0] = "101";
                                statusArray[1] = "Invalid image type or size. Maximum size 1MB format shoulde be .jpg/.Jpeg/.png ";
                                return Json(statusArray, JsonRequestBehavior.AllowGet);
                            }

                            if (proofOfSourceOfFundsScanCopy != null &&
                                proofOfSourceOfFundsScanCopy.ContentLength > 0)
                            {
                                string extension = Path.GetExtension(proofOfSourceOfFundsScanCopy.FileName);
                                string newFileName = customer.NIC + "-Funds" + flagCounterForFunds + extension;
                                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                                proofOfSourceOfFundsScanCopy.SaveAs(path);
                                listOfSourceOfFundsDocs.Add(newFileName);
                                flagCounterForFunds++;
                            }
                        }
                    }

                    if (listOfSourceOfFundsDocs.Count > 0 && listOfProofOfAddrss.Count > 0 && !string.IsNullOrEmpty(customerId))
                    {
                        try
                        {
                            var result = customerService.UploadTranslatedDocuments(Convert.ToInt32(customerId),
                            listOfProofOfAddrss, listOfSourceOfFundsDocs);
                            if (result)
                            {
                                statusArray[0] = "200";
                                statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Files submitted successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CustomerTranslation'>Go back</a></span>";
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
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Required fields missing!";
                    }

                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Required fields missing!";
                }
            }
            catch (Exception exp)
            {

                statusArray[0] = "101";
                statusArray[1] = exp.Message;
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #endregion


        #region AuthoritySignedForm

        public ActionResult AuthoritySignedForm()
        {
            return View(new CustomerService().GetCustomersPendingForAuthoritySignature());
        }

        [HttpPost]
        public ActionResult AuthoritySignedForm(FormCollection formCollection)
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

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new CustomerService().GetCustomersPendingForAuthoritySignature(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        [HttpPost]
        public JsonResult AuthoritySignedFormUploadFile()
        {
            var statusArray = new string[3];
            try
            {
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var authoritySignedFormEng = System.Web.HttpContext.Current.Request.Files["AuthoritySignedFormEng"];
                    var authoritySignedFormPer = System.Web.HttpContext.Current.Request.Files["AuthoritySignedFormPer"];
                    var customerId = System.Web.HttpContext.Current.Request["CustId"];

                    if (authoritySignedFormEng != null && authoritySignedFormPer != null && !string.IsNullOrEmpty(customerId))
                    {
                        var utility = new Utility();
                        if (!utility.IsValidImageClientSideUploading(authoritySignedFormEng) || !utility.IsValidImageClientSideUploading(authoritySignedFormPer))
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Invalid image type or size. Maximum size 1MB format shoulde be .jpg/.Jpeg/.png ";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        string authoritySignedFormPerName;
                        string authoritySignedFormEngName;
                        //get customer on base of customerId
                        var customerService = new CustomerService();
                        var customer = customerService.GetCustomer(Convert.ToInt32(customerId));
                        var documentService = new DocumentService();
                        if (authoritySignedFormEng.ContentLength > 0)
                        {
                            string extension = Path.GetExtension(authoritySignedFormEng.FileName);
                            string newFileName = customer.NIC + "-KYCAuthoritySigned" + extension;
                            var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                            authoritySignedFormEng.SaveAs(path);
                            authoritySignedFormEngName = newFileName;
                        }
                        else
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "English AuthoritySignedForm Document is missing";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        if (authoritySignedFormPer.ContentLength > 0)
                        {
                            string extension = Path.GetExtension(authoritySignedFormPer.FileName);
                            string newFileName = customer.NIC + "-KYCAuthoritySignedCL" + extension;
                            var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                            authoritySignedFormPer.SaveAs(path);
                            authoritySignedFormPerName = newFileName;
                        }
                        else
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Persian AuthoritySignedForm Document is missing";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        if (!string.IsNullOrEmpty(authoritySignedFormEngName) && !string.IsNullOrEmpty(authoritySignedFormPerName))
                        {
                            try
                            {//temp
                                var result = true;//customerService.UploadAuthoritySignedForm(Convert.ToInt32(customerId),authoritySignedFormEngName, authoritySignedFormPerName);
                                if (result)
                                {
                                    statusArray[0] = "200";
                                    statusArray[1] = "Documents uploaded successfully";
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

                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Required fields missing!";
                    }
                }
            }
            catch (Exception e)
            {

                statusArray[0] = "101";
                statusArray[1] = "Failed to update Documents";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SetAuthoritySignedFormUploaded(string customerId)
        {
            var statusArray = new string[3];
            if (!string.IsNullOrEmpty(customerId))
            {
                try
                {
                    var result = new CustomerService().SetAuthoritySignedFormUploaded(Convert.ToInt32(customerId));
                    if (result)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Form has been marked successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/AuthoritySignedForm'>Go back</a></span>";
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to update data please try again";
                    }
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = exp.Message;
                }
            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Unable to update data please try again";
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult SearchKYCForms(FormCollection formCollection)
        {
            string kycFormNo = Convert.ToString(formCollection["KYCFormNo"]);
            string email = Convert.ToString(formCollection["Email"]);
            string nic = Convert.ToString(formCollection["NIC"]);
            string passportNo = Convert.ToString(formCollection["PassportNo"]);
            int statusId = int.Parse(formCollection["FormStatusDDL"]);

            var customerSevice = new CustomerService();
            // customerSevice.GetKYCForms(kycFormNo, email, nic, statusId, passportNo);

            return View();
        }

        #endregion KYC Form

        #region Customers
        /// <summary>
        /// Get All Customers for TBO Agent/ Used for TBO Agent
        /// </summary>
        [HttpGet]
        public ActionResult Customers()
        {
            var customerList = new CustomerService().GetCustomers();
            return View(customerList);
        }

        [HttpPost]
        public ActionResult Customers(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            CustomerStatus? customerStatus = null;

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["CustomerStatus"]))
                customerStatus = (CustomerStatus)Enum.Parse(typeof(CustomerStatus), Request.Form["CustomerStatus"]);

            return View(new CustomerService().GetCustomers(nic: nic, passportNo: passport, customerStatus: customerStatus,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        public JsonResult SetCustomerApprovedOrDeclined(string key, string failureReason, string clientId, string emailAddress, string billingAddress)
        {
            var statusArray = new string[3];
            var customerService = new CustomerService();
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(clientId) && string.IsNullOrEmpty(failureReason) && !string.IsNullOrEmpty(emailAddress) && !string.IsNullOrEmpty(billingAddress))
            {
                //Completed
                try
                {

                    if (!customerService.IsCustomerExist(clientId, emailAddress))
                    {
                        int customerId = Convert.ToInt32(key);
                        var res = customerService.SetCustomerApprovedOrDeclined(Convert.ToInt32(customerId), clientId, billingAddress, emailAddress, null);
                        if (res)
                        {
                            statusArray[0] = "200";
                            statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CardIssuerResponse'>Go back</a></span>";
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
                        statusArray[1] = "Client Id Or Email For Shopping is already exist";
                    }
                }
                catch (Exception ex)
                {
                    statusArray[0] = "101";
                    statusArray[1] = ex.Message;
                }

            }
            else if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(failureReason) && string.IsNullOrEmpty(clientId))
            {
                // Failure Reason
                try
                {
                    int customerId = Convert.ToInt32(key);
                    //(int customerId, string clientId, string billingAddress, string emailForShopping, string declinedReason)
                    var res = customerService.SetCustomerApprovedOrDeclined(Convert.ToInt32(customerId), clientId, null, null, failureReason);
                    if (res)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CardIssuerResponse'>Go back</a></span>";
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


        //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public ActionResult CustomerDetails(int Id)
        {
            //var customerService = new CustomerService();
            //var customer = customerService.GetCustomer(customerId);

            //if (customer != null)
            //{
            //    var userService = new UserService();
            //    var user = userService.GetUser(customer.UserId);
            //}
            //var cardService = new CardService();
            //ViewData["UserDetail"] = user;
            //ViewData["Cards"] = cardService.GetCards(customerId: customerId);
            //ViewData["CardRequest"] = cardService.GetCardRequests(customerId: customerId);
            //ViewData["TopupRequest"] = new TopUpService().GetTopUpRequests(customerId: customerId);

            // var cardTypes = cardService.GetCardTypes();
            return View();
        }

        #region Get Customers

        public ActionResult MyProfile()
        {
            var userSession = Session["CurrentSession"] as User;
            var customer = new CustomerService().GetCustomer(userSession.Id, true);
            return View(customer);
        }


        /// <summary>
        /// Get single customer based on filter criteria
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="nic"></param>
        /// <param name="securityQuestion"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public JsonResult GetCustomer(string nic, string securityQuestion, string answer)
        {
            try
            {
                var commonService = new CommonService();
                string encryptedQuestion = commonService.base64Encode(securityQuestion);
                string encryptedAnswer = commonService.base64Encode(answer);

                string actualNic = nic.Trim(',');
                var customerService = new CustomerService();
                var customer = customerService.GetCustomer(actualNic, encryptedQuestion, encryptedAnswer);

                if (customer == null)
                {
                    return Json(new { message = "No customer found." }, JsonRequestBehavior.AllowGet);
                }

                string[] customerInfo = new string[] { customer.User.FirstName, customer.User.LastName, customer.NIC, "" + customer.Id };
                return Json(customerInfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "An error occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Customer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCustomers(FormCollection formCollection)
        {
            string nic = Convert.ToString(formCollection["NIC"]);
            DateTime regDateFrom = DateTime.ParseExact(formCollection["RegDateFrom"], "d/m/yyyy", null);
            DateTime regDateTo = DateTime.ParseExact(formCollection["RegDateTo"], "d/m/yyyy", null);
            int statusId = int.Parse(formCollection["StatusDDL"]);

            var customerSevice = new CustomerService();
            customerSevice.GetCustomers(nic: nic, createdDateFrom: regDateFrom, createdDateTo: regDateTo);//, statusId);

            return View();
        }

        [HttpGet]
        public JsonResult GetCustmers(string cardNo = null, string nic = null, DateTime? createdDateFrom = null
            , DateTime? createdDateTo = null, UserStatus? status = null)
        {
            var customerService = new CustomerService();
            var customers = customerService.GetCustomers(nic: nic, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo
                , userStatus: status);
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Will use for Dashboard
        /// </summary>
        public int GetCustomersCountByStatus(CustomerStatus customerStatus)
        {
            return new CustomerService().GetCustomersCountByStatus(customerStatus);
        }

        #endregion

        #endregion Customers

        #region Change Password

        public ActionResult ChangePassword()
        {
            var topUpService = new TopUpService();
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection formCollection)
        {
            var userSession = Session["CurrentSession"] as User;
            if (userSession != null)
            {
                var customerService = new CustomerService();
                string oldPassword = null;
                if (!string.IsNullOrEmpty(formCollection["oldPassword"]))
                {
                    oldPassword = Convert.ToString(formCollection["oldPassword"]);
                }
                string password = null;
                if (!string.IsNullOrEmpty(formCollection["password"]))
                {
                    password = formCollection["password"]; // Not used
                }
                string cfmPassword = null;
                if (!string.IsNullOrEmpty(formCollection["cfmPassword"]))
                {
                    cfmPassword = formCollection["cfmPassword"];
                }
                bool isSaved = customerService.ChangePassword(oldPassword, cfmPassword, userSession.Id);
                if (isSaved)
                {
                    ViewBag.ResponseCode = "200";
                    ViewBag.ResponseMessage = "Password successfully changed!";
                }
                else
                {
                    ViewBag.ResponseCode = "101";
                    ViewBag.ResponseMessage = "Failed to change password. Please try again.";
                }
            }
            return View();
        }

        #endregion

        public ActionResult ValidateKYCForms(int? customerId)
        {
            int totalCount = 0;
            var customers = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount);
            return View(customers);
        }

        [HttpPost]
        public ActionResult ValidateKYCForms(FormCollection formCollection)
        {
            string nic = Convert.ToString(formCollection["nic"]);
            string passportNo = Convert.ToString(formCollection["passportNo"]);
            DateTime? createdDateFrom = null;
            if (!String.IsNullOrEmpty(formCollection["createdDateFrom"].ToString()))
            {
                createdDateFrom = DateTime.ParseExact(formCollection["createdDateFrom"].ToString(), "d/m/yyyy", null);
            }
            DateTime? createdDateTo = null;
            if (!String.IsNullOrEmpty(formCollection["createdDateTo"].ToString()))
            {
                createdDateTo = DateTime.ParseExact(formCollection["createdDateTo"].ToString(), "d/m/yyyy", null);
            }
            var customers = new CustomerService().GetCustomersPendingForValidatationOrFailed(nic: nic, passportNo: passportNo, createdDateTo: createdDateTo, createdDateFrom: createdDateFrom);
            return View(customers);
        }

        [HttpGet]
        public JsonResult UpdateCustomerValidationStatus(int customerId, string failureReason)
        {
            bool IsUpdated = new CustomerService().SetCustomerValidated(customerId, failureReason);
            return Json(new { IsUpdate = IsUpdated, CustomerId = customerId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendToCardProvider()
        {
            return View(new CustomerService().GetCustomersPendingToSendToCardIssuer());
        }

        [HttpPost]
        public ActionResult SendToCardProvider(FormCollection formCollection)
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

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new CustomerService().GetCustomersPendingToSendToCardIssuer(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        public JsonResult UpdateCustomerCardIssuerStatus(string customerId)
        {
            var statusArray = new string[3];
            if (!string.IsNullOrEmpty(customerId))
            {
                try
                {
                    bool isUpdated = new CustomerService().SendCustomerToCardIssuer(Convert.ToInt32(customerId));
                    if (isUpdated)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Data sent successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/SendToCardProvider'>Go back</a></span>";
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to update data please try again";
                    }
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = exp.Message;
                }
            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Unable to update data please try again";
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }




        #region LAMDA Response

        public ActionResult CardIssuerResponse()
        {
            return View(new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined());
        }
        [HttpPost]
        public ActionResult CardIssuerResponse(FormCollection formCollection)
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

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new CustomerService().GetCustomersWaitingForCardIssuerResponseOrDeclined(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }
    }
    #endregion
}
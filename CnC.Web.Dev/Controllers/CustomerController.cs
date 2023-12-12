using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Cards;
using CnC.Core.Customers;
using CnC.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Helper;
using CnC.Core.Payments;
using System.Net.Mail;
using CnC.Core.Exceptions;
using CnC.Web.Models;
using System.Web.Security;
using BotDetect.Web.Mvc;
using log4net;

namespace CnC.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CustomerController));

        #region Security Questions
        [RoleActionValidator]
        public List<SecurityQuestion> GetSecurityQuestions()
        {
            return new CustomerService().GetSecurityQuestions();
        }
        [RoleActionValidator]
        public ActionResult SecurityQuestions(int? customerId = null)
        {
            return View();
        }

        [RoleActionValidator]
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
            catch (UserException exp)
            {
                ViewBag.ResponseCode = "101";
                ViewBag.ResponseMessage = "Failed: " + exp.Message;
            }
            catch (Exception exp)
            {
                ViewBag.ResponseCode = "101";
                ViewBag.ResponseMessage = "Sorry, we are unable to process your request. Please try again later.";
            }

            return View();
        }

        #endregion Security Questions

        #region KYC Form
        [RoleActionValidator]
        public ActionResult KYCForm()
        {
            return View();
        }

        [RoleActionValidator]
        public ActionResult KYCForms()
        {
            int totalCount = 0;
            return View(new CustomerService().GetCustomers(out totalCount));
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult KYCForms(FormCollection formCollection)
        {
            string email = Convert.ToString(formCollection["txtEmail"]);
            string nic = Convert.ToString(formCollection["txtNICNo"]);
            string passportNo = Convert.ToString(formCollection["txtPassportNo"]);
            var requestFormStatus = (formCollection["RequestFormStatus"]);
            int totalCounts = 0;
            //TODO: status is pending to search
            int status = 9;
            if (!string.IsNullOrEmpty(requestFormStatus))
                status = int.Parse(formCollection["RequestFormStatus"]);

            if (true)
            {

            }

            var searchKycForm = new CustomerService().GetCustomers(out totalCounts, requestFormStatus);

            return View(searchKycForm);
        }

        [RoleActionValidator]
        public ActionResult UploadSignedKYCForm()
        {
            //later we will make a common method for all actions

            var kycForms = new CustomerService().GetCustomer(1);
            return View("KYCForms", kycForms);
        }


        #region Rahyab (Include New Customer)


        #region New Customer

        [RoleActionValidator]
        [HttpGet]
        public ActionResult EditCustomer(string editId)
        {
            if (editId != null)
            {
                var customerId = new Utility().UrlSafeDecrypt(editId);
                if (!string.IsNullOrEmpty(customerId))
                {
                    var customer = new CustomerService().GetCustomer(Convert.ToInt32(customerId));
                    ViewBag.imgNicDocPath = customer.NICDocInCustomerLanguage;
                    ViewBag.imgPassportDocPath = customer.PassportDocInCustomerLanguage;
                    ViewBag.custSignedDocCl = customer.CustomerSignedFormInCustomerLanguage;
                    ViewBag.custSignedDocEng = customer.CustomerSignedForm;
                    customer.NICDocInCustomerLanguage = Path.Combine(new DocumentService()
                        .GetDocumentDirectoryPath(customer.NIC), customer.NICDocInCustomerLanguage);
                    customer.PassportDocInCustomerLanguage = Path.Combine(new DocumentService()
                        .GetDocumentDirectoryPath(customer.NIC), customer.PassportDocInCustomerLanguage);
                    customer.CustomerSignedFormInCustomerLanguage = Path.Combine(new DocumentService()
                        .GetDocumentDirectoryPath(customer.NIC), customer.CustomerSignedFormInCustomerLanguage);
                    customer.CustomerSignedForm = Path.Combine(new DocumentService()
                        .GetDocumentDirectoryPath(customer.NIC), customer.CustomerSignedForm);
                    return View(customer);
                }
                else
                {
                    return View();
                }
            }

            return View();
        }

        [RoleActionValidator]
        [HttpPost]
        public JsonResult EditCustomer(FormCollection formCollection, HttpPostedFileBase NICDocCustomer
            , HttpPostedFileBase PassportDocCustomer, HttpPostedFileBase customerSignedCopyCl
            , HttpPostedFileBase customerSignedCopyEng)
        {
            var statusArray = new string[3];
            string email = Request.Form["txtEmail"];
            var editId = Request.Form["hfCustomerId"];
            string originalEmail = Request.Form["txtOriginalEmail"];
            string originalNic = Request.Form["txtOriginalNic"];
            string originalPassport = Request.Form["txtOriginalPassport"];

            var user = new User();
            user.Email = email;
            user.Username = user.Email;
            user.Status = (int)UserStatus.Active;
            var userSession = Session["CurrentSession"] as User;

            if (!string.IsNullOrEmpty(originalEmail) && !originalEmail.Equals(email)
                && new UserService().GetUser(email) != null)
            {
                statusArray[0] = "101";
                statusArray[1] = "Email already exists!";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(editId) && new UserService().GetUser(user.Email) != null)
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

            string nicDocPath = Request.Form["NicDocPath"];
            string passportDocPath = Request.Form["PassportDocPath"];

            string custSignedDocCl = Request.Form["CustSignedDocCl"];
            string custSignedDocEng = Request.Form["CustSignedDocEng"];

            string resendPassword = Request.Form["chkResendPass"];

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
            else if (!string.IsNullOrEmpty(originalPassport) && !originalPassport.Equals(passportNo)
                && new CustomerService().GetCustomerByPassportNo(passportNo) != null)
            {
                statusArray[0] = "101";
                statusArray[1] = "Passport No. already exists!";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            else if (string.IsNullOrEmpty(postalCode) && string.IsNullOrWhiteSpace(postalCode))
            {
                statusArray[0] = "101";
                statusArray[1] = "postalCode is a required field.";
            }
            else if (!string.IsNullOrEmpty(originalNic) && !originalNic.Equals(nic)
                 && new CustomerService().GetCustomerByNic(nic) != null)
            {
                statusArray[0] = "101";
                statusArray[1] = "NIC already exists!";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            else if (NICDocCustomer != null && !utility.IsValidImage(NICDocCustomer))
            {
                statusArray[0] = "101";
                statusArray[1] = "Invalid NIC Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
            }
            else if (PassportDocCustomer != null && !utility.IsValidImage(PassportDocCustomer))
            {
                statusArray[0] = "101";
                statusArray[1] =
                    "Invalid Passport Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
            }
            else if (customerSignedCopyEng != null && !utility.IsValidImage(customerSignedCopyEng))
            {
                statusArray[0] = "101";
                statusArray[1] = "Invalid Customer Signed English Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
            }
            else if (customerSignedCopyCl != null && !utility.IsValidImage(customerSignedCopyCl))
            {
                statusArray[0] = "101";
                statusArray[1] =
                    "Invalid Customer Signed Persian Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
            }
            else
            {
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

                if (PassportDocCustomer != null && PassportDocCustomer.ContentLength > 0)
                {
                    string extension = Path.GetExtension(PassportDocCustomer.FileName);
                    string newFileName = customer.NIC + "-PassportCL" + extension;
                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                    PassportDocCustomer.SaveAs(path);
                    customer.PassportDocInCustomerLanguage = newFileName;
                }
                else
                {
                    customer.PassportDocInCustomerLanguage = passportDocPath;
                }
                if (NICDocCustomer != null && NICDocCustomer.ContentLength > 0)
                {
                    string extension = Path.GetExtension(NICDocCustomer.FileName);
                    string newFileName = customer.NIC + "-DocCL" + extension;
                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                    NICDocCustomer.SaveAs(path);
                    customer.NICDocInCustomerLanguage = newFileName;
                }

                else
                {
                    customer.NICDocInCustomerLanguage = nicDocPath;
                }

                if (customerSignedCopyEng != null && customerSignedCopyEng.ContentLength > 0)
                {
                    string extension = Path.GetExtension(customerSignedCopyEng.FileName);
                    string newFileName = customer.NIC + "-KYCCustomerSigned" + extension;
                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                    customerSignedCopyEng.SaveAs(path);
                    customer.CustomerSignedForm = newFileName;
                }

                else
                {
                    customer.CustomerSignedForm = custSignedDocEng;
                }

                if (customerSignedCopyCl != null && customerSignedCopyCl.ContentLength > 0)
                {
                    string extension = Path.GetExtension(customerSignedCopyCl.FileName);
                    string newFileName = customer.NIC + "-KYCCustomerSignedCL" + extension;
                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                    customerSignedCopyCl.SaveAs(path);
                    customer.CustomerSignedFormInCustomerLanguage = newFileName;

                }

                else
                {
                    customer.CustomerSignedFormInCustomerLanguage = custSignedDocCl;
                }

                var customerService = new CustomerService();
                try
                {
                    //check here whether it is from update or add

                    editId = utility.UrlSafeDecrypt(editId);
                    if (!string.IsNullOrEmpty(editId) && !string.IsNullOrEmpty(editId))
                    {
                        //update 
                        user.Id = Convert.ToInt32(editId);
                        int updateRes = customerService.UpdateCustomer(customer, user, resendPassword: resendPassword);
                        if (updateRes > 0)
                        {
                            //statusArray[0] = "200";
                            //statusArray[1] = utility.UrlSafeEncrypt(updateRes.ToString());
                            //statusArray[2] = "Step2";
                            statusArray[0] = "200";
                            if (userSession.IsTBOAdmin)
                                statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Information updated successfully!<br/> <a id='lkGoBack' style='font-size: 14px; ' href='/Customer/Customers'>Go Back</a></span>";//"" + res;
                            else
                            {
                                statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Information updated successfully!<br/> <a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CustomerInfoCorrection'>Go Back</a></span>";//"" + res;
                            }
                            statusArray[2] = "102";//Update
                        }
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Invalid request. Try again.";
                    }
                }
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                }

            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [RoleActionValidator]
        [HttpGet]
        public ActionResult CustomerInfoCorrection(int page = 0)
        {
            var userSession = Session["CurrentSession"] as User;
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;

            List<Customer> customers = null;
            if (userSession != null)
            {
                customers = new CustomerService()
                    .GetCustomersPendingForValidatationOrFailed(out totalCount, pageIndex: page
                    , pageSize: pageSize);

                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(customers);
            }
            return View();
        }


        [RoleActionValidator]
        [HttpPost]
        public ActionResult CustomerInfoCorrection(FormCollection formCollection)
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
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            int pageSize = new SettingService().ResultPageSize;
            var customersPendingForInfoCorrection = new CustomerService().GetCustomersPendingForValidatationOrFailed(
                out totalCount, nic: nic, passportNo: passport, createdDateFrom: createdDateFrom
                , createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(customersPendingForInfoCorrection);
        }

        [RoleActionValidator]
        [HttpGet]
        public ActionResult EditCustomerCardRequests(string id)
        {
            //Put some validation here on base of Id is it valid or not etc.
            var userSession = Session["CurrentSession"] as User;
            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    //Decode Id here
                    id = new Utility().UrlSafeDecrypt(id);
                }
                else
                {
                    //When customer wants to add new card request online
                    if (userSession != null && userSession.IsCustomer && string.IsNullOrEmpty(id))
                        id = userSession.Id.ToString();
                }


                if (!System.Text.RegularExpressions.Regex.IsMatch(id, "^[0-9]*$"))
                {
                    if (userSession != null && userSession.IsCustomer)
                    {
                        return RedirectToAction("MyProfile", "Customer");
                    }
                    return RedirectToAction("NewCustomer", "Customer");
                }

                if (userSession != null && userSession.IsCustomer && id.Length >= 1 && id.Length <= 15)
                {
                    ViewBag.CustomerId = new Utility().UrlSafeEncrypt(id);
                    return View();
                }
                if (!new CardService().IsCustomerCardRequestFormExist(Convert.ToInt32(id)) && id.Length >= 1 && id.Length <= 15)
                {
                    ViewBag.CustomerId = new Utility().UrlSafeEncrypt(id);
                    return View();
                }

                if (userSession != null && userSession.IsCustomer)
                {
                    return RedirectToAction("MyProfile", "Customer");
                }
                return RedirectToAction("NewCustomer", "Customer");
            }
            catch (Exception e)
            {
                if (userSession != null && userSession.IsCustomer)
                {
                    return RedirectToAction("MyProfile", "Customer");
                }
                return RedirectToAction("NewCustomer", "Customer");
            }
        }

        [RoleActionValidator]
        [HttpGet]
        public ActionResult NewCustomer(string editId)
        {
            return View();
        }

        [RoleActionValidator]
        [HttpPost]
        public JsonResult NewCustomer(FormCollection formCollection, HttpPostedFileBase NICDocCustomer
            , HttpPostedFileBase PassportDocCustomer)
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
            var editId = Request.Form["hfCustomerId"];
            var userSession = Session["CurrentSession"] as User;
            var user = new User();
            user.Email = email;
            user.Username = user.Email;
            user.Status = (int)UserStatus.Active;
            if (string.IsNullOrEmpty(editId) && new UserService().GetUser(user.Email) != null)
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
                    string DomainName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

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
                        //check here whether it is from update or add

                        editId = utility.UrlSafeDecrypt(editId);
                        if (!string.IsNullOrEmpty(editId) && !string.IsNullOrEmpty(editId))
                        {
                            //update 
                            user.Id = Convert.ToInt32(editId);
                            int updateRes = customerService.UpdateCustomer(customer, user);
                            if (updateRes > 0)
                            {
                                statusArray[0] = "200";
                                statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Information updated successfully!<br/> <a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CustomerValidation'>Go Back</a></span>";//"" + res;
                                statusArray[2] = "102";//Update
                            }
                        }
                        else
                        {
                            //add
                            if (userSession.IsRahyab)
                                customer.CustomerRegistrationMode = (int)RegistrationMode.RAHYAB;
                            if (userSession.IsTBOAgent)
                                customer.CustomerRegistrationMode = (int)RegistrationMode.TBOAgent;
                            if (userSession.IsTBOAdmin)
                                customer.CustomerRegistrationMode = (int)RegistrationMode.Admin;
                            if (userSession.IsTBOSuperAgent)
                                customer.CustomerRegistrationMode = (int)RegistrationMode.TBOSuperAgent;

                            int res = customerService.AddCustomer(customer, user);

                            if (res > 0)
                            {
                                statusArray[0] = "200";
                                statusArray[1] = utility.UrlSafeEncrypt(res.ToString());
                                statusArray[2] = "Step2";
                                return Json(statusArray, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                statusArray[0] = "101";
                                statusArray[1] = "Unable to process request.";
                            }
                        }

                    }
                    catch (UserException exp)
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Failed: " + exp.Message;
                    }
                    catch (Exception exp)
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                    }
                }

                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Missing required information.";
                }

            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [RoleActionValidator]
        [HttpGet]
        public ActionResult NewCustomerCardRequests(string id)
        {
            //Put some validation here on base of Id is it valid or not etc.
            var userSession = Session["CurrentSession"] as User;
            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    //Decode Id here
                    id = new Utility().UrlSafeDecrypt(id);
                }
                else
                {
                    //When customer wants to add new card request online
                    if (userSession != null && userSession.IsCustomer && string.IsNullOrEmpty(id))
                        id = userSession.Id.ToString();
                }


                if (!System.Text.RegularExpressions.Regex.IsMatch(id, "^[0-9]*$"))
                {
                    if (userSession != null && userSession.IsCustomer)
                    {
                        return RedirectToAction("MyProfile", "Customer");
                    }
                    return RedirectToAction("NewCustomer", "Customer");
                }

                if (userSession != null && userSession.IsCustomer && id.Length >= 1 && id.Length <= 15)
                {
                    var newCardRequest = Session["TempNewCardRequest"] as CustomerNewRequestContainer;
                    if (Session["TempNewCardRequest"] != null)
                    {

                        // get user seleted card type id and card qty

                        //To show user selection in case of failure
                        ViewBag.CardTypeId = newCardRequest.CardTypeIds;
                        ViewBag.CardQty = newCardRequest.CardQty;
                        ViewBag.Amount = newCardRequest.Payments[0].Amount;

                        ViewBag.ResultCode = TempData["ResultCode"];
                        ViewBag.Description = TempData["Description"];
                        ViewBag.CustomerId = new Utility().UrlSafeEncrypt(userSession.Id.ToString());
                        Session["TempNewCardRequest"] = null;

                        // to show the failure/success message to customer after online payment
                        if (!string.IsNullOrEmpty(newCardRequest.Message))
                            ViewBag.MessageFailure = newCardRequest.Message;
                    }

                    ViewBag.CustomerId = new Utility().UrlSafeEncrypt(id);
                    return View();
                }

                if (userSession != null && userSession.IsCustomer)
                {
                    return RedirectToAction("MyProfile", "Customer");
                }

                ViewBag.CustomerId = new Utility().UrlSafeEncrypt(id);
                return View();
            }
            catch (Exception e)
            {
                if (userSession != null && userSession.IsCustomer)
                {
                    return RedirectToAction("MyProfile", "Customer");
                }
                return RedirectToAction("NewCustomer", "Customer");
            }
        }

        [RoleActionValidator]
        public ActionResult NewCustomerSignedForms()
        {
            var userSession = Session["CurrentSession"] as User;
            var customer = new CustomerService().GetCustomer(userSession.Id);
            var content = new Utility().DownloadCustomerFilledForm(customer, userSession, customer.CardRequestForms[0]);
            ViewBag.Contents = content;
            return View();
        }

        [RoleActionValidator]
        [ActionName("PaymentGatewayResponse")]
        [HttpPost]
        public ActionResult NewCustomerCardRequests()
        {
            var userSession = Session["CurrentSession"] as User;

            if (Session["TempNewCardRequest"] == null)
                return Redirect("~/Customer/NewCustomerCardRequests" + "?id="
                    + new Utility().UrlSafeEncrypt(userSession.Id.ToString()));
            int gateWayId = (int)RequestFormType.Card;
            IPaymentGateway paymentGateway = new PaymentGatewayInfoService().GetActivePaymentGateway(gateWayId);
            var response = paymentGateway.GetResponse(Request);

            if (!response.IsSuccess)
            {
                TempData["ResultCode"] = "";
                TempData["Description"] = response.Message;
                return Redirect("~/Customer/NewCustomerCardRequests" + "?id="
                    + new Utility().UrlSafeEncrypt(userSession.Id.ToString()));
            }

            var statusArray = new string[5];

            var newCardRequest = Session["TempNewCardRequest"] as CustomerNewRequestContainer;
            try
            {


                var settingService = new SettingService();
                var tempActualAmt = newCardRequest.Payments[0].Amount; //* exchangeRate;
                tempActualAmt = tempActualAmt + ((tempActualAmt / 100) * settingService.ExchangeRateServiceCharges);

                var totalAmtIncludingAllCharges = Convert.ToInt32(tempActualAmt);

                newCardRequest.Payments[0].Amount = totalAmtIncludingAllCharges;
                newCardRequest.Payments[0].TransactionNo = response.TransactionNumber;

                int requestForm = 0;
                int totalCount = 0;
                var customerDeliveredCards = new CardService().GetCards(out totalCount, customerId: userSession.Id);

                if (customerDeliveredCards != null && customerDeliveredCards.Count() > 0)
                {
                    requestForm = new CardService().AddCardRequestForm(newCardRequest.RequestFormObj
                       , newCardRequest.CardRequestCollection, newCardRequest.Payments, false
                       , newCardRequest.ProofOfAddressType, newCardRequest.ListOfProofOfAddrss
                       , newCardRequest.SourcceOfFundType, newCardRequest.ListOfSourceOfFundsDocs);
                }
                else
                {
                    requestForm = new CardService().AddCardRequestForm(newCardRequest.RequestFormObj
                        , newCardRequest.CardRequestCollection, newCardRequest.Payments, true
                        , newCardRequest.ProofOfAddressType, newCardRequest.ListOfProofOfAddrss
                        , newCardRequest.SourcceOfFundType, newCardRequest.ListOfSourceOfFundsDocs);
                }

                if (requestForm > 0)
                {
                    if (customerDeliveredCards != null
                    && customerDeliveredCards.Count() > 0)
                    {
                        Session["TempNewCardRequest"] = null;
                        return RedirectToAction("NewCustomerCardRequests");
                    }
                    else
                    {
                        Session["TempNewCardRequest"] = null;
                        return RedirectToAction("NewCustomerSignedForms");
                        //return Redirect("~/Customer/NewCustomerSignedForm?id="
                        //    + new Utility().UrlSafeEncrypt(userSession.Id.ToString()));
                    }
                }
                else
                {
                    Session["TempNewCardRequest"] = null;
                    // redirect to card request page
                    return Redirect("~/Customer/NewCustomerCardRequests?id="
                                + new Utility().UrlSafeEncrypt(userSession.Id.ToString()));
                }
            }
            catch (Exception ex)
            {
                newCardRequest.Message = ex.Message;
                log.Error(ex);
                return Redirect("~/Customer/NewCustomerCardRequests?id="
                                + new Utility().UrlSafeEncrypt(userSession.Id.ToString()));
            }
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult NewCustomerCardRequests(FormCollection formCollection,
        IEnumerable<HttpPostedFileBase> proofOfAddressDocCl, IEnumerable<HttpPostedFileBase> proofOfSourceOfFundsCl,
        string hfCustomerId, string proofOfAddressType, string sourcceOfFundType, string hfsubmitActionName)
        {
            var statusArray = new string[5];
            var cardName = Request.Form["CardName"];
            string cardTypeIds = Request.Form["cardId"];
            string cardQuantity = Request.Form["CardQty"];
            var userSession = Session["CurrentSession"] as User;
            string sourceOfFundTypeName = Request.Form["hfSourceOfFundTypeN"];
            string proofOfAddressTypeName = Request.Form["hfProofOfAddressTypeN"];
            string imgBase64Eng = Request.Form["hiddenFieldEnglish"];
            string imgBase64Per = Request.Form["hiddenFieldPersian"];

            // this if condition will execute when the images from the View page are
            // created and stored in 'imgBase64Eng' && 'imgBase64Per' (the page will load 2nd time , not first time)

            if (!string.IsNullOrEmpty(imgBase64Eng) && !string.IsNullOrEmpty(imgBase64Per))
            {
                SaveKycForm(imgBase64Eng, imgBase64Per);
                statusArray[0] = "207";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }

            Customer customer = null;
            if (hfCustomerId != null)
            {
                hfCustomerId = new Utility().UrlSafeDecrypt(hfCustomerId);
                customer = new CustomerService().GetCustomer(Convert.ToInt32(hfCustomerId));
            }

            if (!string.IsNullOrEmpty(cardTypeIds) && !string.IsNullOrEmpty(cardQuantity))
            {
                var settingService = new SettingService();
                var documentService = new DocumentService();
                int defaultCurrencyId = settingService.CustomerDefaultCurrency.Id;
                var proofOfSourceOfFundsImagesCl = proofOfSourceOfFundsCl as IList<HttpPostedFileBase>
                    ?? proofOfSourceOfFundsCl.ToList();
                var proofOfAddressImagesCl = proofOfAddressDocCl as IList<HttpPostedFileBase>
                    ?? proofOfAddressDocCl.ToList();

                #region Step2
                var listOfSourceOfFundsDocs = new List<string>();
                var listOfProofOfAddrss = new List<string>();
                string proofOfAddresDoc = string.Empty;
                var typesOfCards = new CardService().GetCardTypes(customer: customer);
                var arrCardTypeIds = cardTypeIds.Split(',');

                if (!string.IsNullOrEmpty(hfCustomerId) && !string.IsNullOrEmpty(proofOfAddressType)
                    && !string.IsNullOrEmpty(sourcceOfFundType))
                {
                    if (string.IsNullOrEmpty(hfCustomerId))
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Invalid value please try again.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }

                    var cardRequests = new List<CardRequest>();
                    var arrCardQty = formCollection["CardQty"].Split(',');
                    bool cardPurchased = false;
                    decimal totalAmount = 0;
                    bool isProofOfAddressRequired = false;
                    bool isProofOfSourceOfFundsRequired = false;

                    // if customer is already having a card request and it contains black card

                    bool blackCardExist = false;
                    int totalCount = 0;

                    var customerDeliveredCards = new CardService().GetCards(out totalCount
                                               , customerId: Convert.ToInt32(hfCustomerId));
                    var customerCardRequestForm = new CardService().GetCardRequestForms(out totalCount
                                                , customerId: Convert.ToInt32(hfCustomerId));

                    if (customerCardRequestForm != null && customerCardRequestForm.Count() > 0)
                    {
                        foreach (var cardRequest in customerCardRequestForm[0].CardRequests)
                        {
                            if (cardRequest.CardType.IsProofOfSourceOfFundsRequired)
                            {
                                blackCardExist = true;
                                break;
                            }
                        }
                    }

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
                        if (userSession != null && userSession.IsCustomer)
                            cr.ServiceExchangeFee = new SettingService().ExchangeRateServiceCharges;

                        cardRequests.Add(cr);
                        totalAmount = totalAmount + (cr.Fee * cr.CardQty);

                        if (!blackCardExist)
                        {
                            if (typesOfCards[i].IsProofOfAddressRequired)
                                isProofOfAddressRequired = typesOfCards[i].IsProofOfAddressRequired;

                            if (typesOfCards[i].IsProofOfSourceOfFundsRequired)
                                isProofOfSourceOfFundsRequired = typesOfCards[i].IsProofOfSourceOfFundsRequired;
                        }
                    }

                    // Request Form info
                    var requestForm = new RequestForm();
                    requestForm.CustomerId = Convert.ToInt32(hfCustomerId);
                    requestForm.Type = RequestFormType.Card;
                    requestForm.TypeId = (int)RequestFormType.Card;
                    requestForm.ExchangeRate = new ExchangeRateService().GetExchangeRate(defaultCurrencyId).Rate;
                    // Proof of source of funds doc upload

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
                    var utility = new Utility();
                    if (isProofOfAddressRequired)
                    {
                        int flagAddressCounter = 0;
                        foreach (var proofOfAddressImgCustomer in proofOfAddressImagesCl)
                        {
                            if (proofOfAddressImgCustomer != null && utility.IsValidImage(proofOfAddressImgCustomer))
                            {
                                string extension = Path.GetExtension(proofOfAddressImgCustomer.FileName);
                                string newFileName = customer.NIC + "-AddressCL" + flagAddressCounter + extension;
                                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                                proofOfAddressImgCustomer.SaveAs(path);
                                listOfProofOfAddrss.Add(newFileName);
                                flagAddressCounter++;
                            }
                        }
                    }

                    if (isProofOfSourceOfFundsRequired)
                    {
                        int flagCounter = 0;
                        foreach (var proofOfSourceOfFundsImageCustomer in proofOfSourceOfFundsImagesCl)
                        {
                            if (proofOfSourceOfFundsImageCustomer != null &&
                                utility.IsValidImage(proofOfSourceOfFundsImageCustomer))
                            {
                                string extension = Path.GetExtension(proofOfSourceOfFundsImageCustomer.FileName);
                                string newFileName = customer.NIC + "-FundsCL" + flagCounter + extension;
                                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC)
                                    , newFileName);
                                proofOfSourceOfFundsImageCustomer.SaveAs(path);
                                listOfSourceOfFundsDocs.Add(newFileName);
                                flagCounter++;
                            }
                        }
                    }

                    // Payment info

                    var payments = new List<Payment>();
                    var payment = new Payment();
                    if (userSession != null && userSession.IsCustomer)
                    {
                        string cardTransactionNo = Request.Form["txtCardTransactionNo"];
                        string cardAccountNo = Request.Form["txtCardAccountNo"];
                        string cardReqCustName = Request.Form["txtCardReqCustName"];
                        string cardPaidAmount = Request.Form["txtCardPaidAmount"];

                        if (hfsubmitActionName != "Online" && !string.IsNullOrEmpty(cardTransactionNo)
                            && !string.IsNullOrEmpty(cardAccountNo) && !string.IsNullOrEmpty(cardReqCustName)
                            && !string.IsNullOrEmpty(cardPaidAmount))
                        {
                            var exchangeRate = new ExchangeRateService()
                                .GetExchangeRate(settingService.CustomerDefaultCurrency.Id).Rate;

                            var tempActualAmt = totalAmount * exchangeRate;
                            tempActualAmt = tempActualAmt + ((tempActualAmt / 100) *
                                settingService.ExchangeRateServiceCharges); // Applying exchange rate services charges
                            var tempToVerifyAmt = Convert.ToInt32(cardPaidAmount);

                            if (tempToVerifyAmt < tempActualAmt)
                            {
                                statusArray[0] = "101";
                                statusArray[1] = "Invalid amount entered in Amount Textbox.";
                                return Json(statusArray, JsonRequestBehavior.AllowGet);
                            }
                            payment.Amount = Convert.ToInt32(cardPaidAmount);
                            payment.CreatedOn = DateTime.UtcNow;
                            payment.TransactionAccountNo = cardAccountNo;
                            payment.TransactionName = cardReqCustName;
                            payment.TransactionNo = cardTransactionNo;
                            payment.PaymentMethod = PaymentMethod.Bank;
                            payment.PaymentMethodId = (int)PaymentMethod.Bank;
                        }
                        else if (hfsubmitActionName == "Online")
                        {
                            var exchangeRate = new ExchangeRateService()
                                .GetExchangeRate(settingService.CustomerDefaultCurrency.Id).Rate;

                            payment.Amount = totalAmount * exchangeRate;//tempActualAmt;
                            payment.CreatedOn = DateTime.UtcNow;
                            payment.PaymentMethod = PaymentMethod.Online;
                            payment.PaymentMethodId = (int)PaymentMethod.Online;
                        }
                        else
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Invalid Transaction information.";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var exchangeRate = new ExchangeRateService()
                                .GetExchangeRate(settingService.CustomerDefaultCurrency.Id).Rate;

                        payment.Amount = totalAmount * exchangeRate;
                        payment.CreatedOn = DateTime.UtcNow;
                        payment.ConfirmedOn = DateTime.Now;
                        payment.ReConfirmedOn = DateTime.Now;

                        if (userSession != null && userSession.IsTBOSuperAgent)
                        {
                            payment.PaymentMethod = PaymentMethod.TBOSuperAgent;
                            payment.PaymentMethodId = (int)PaymentMethod.TBOSuperAgent;
                        }
                        else
                        {
                            payment.PaymentMethod = PaymentMethod.RAHYAB;
                            payment.PaymentMethodId = (int)PaymentMethod.RAHYAB;
                        }
                    }

                    payments.Add(payment);

                    if (cardPurchased)
                    {
                        try
                        {
                            int requestFormId = 0;
                            if (userSession != null && userSession.IsCustomer)
                            {
                                if (hfsubmitActionName == "Online" && payments.Count > 0)
                                {
                                    var tempActualAmt = payments[0].Amount; //* exchangeRate;
                                    tempActualAmt = tempActualAmt + ((tempActualAmt / 100) *
                                        settingService.ExchangeRateServiceCharges);

                                    var totalAmtIncludingAllCharges = Convert.ToInt32(tempActualAmt); //(payments[0].Amount);
                                    IPaymentGateway paymentGateway = new PaymentGatewayInfoService().
                                        GetActivePaymentGateway(requestForm.TypeId);
                                    PaymentGatewayResponse responsePaymentService = paymentGateway
                                        .GenerateToken(totalAmtIncludingAllCharges, DateTime.Now.ToString("yyMMddHHmmss")
                                        , DateTime.Now.ToString("yyMMddHHmmss")
                                        , Request.Url.Scheme + "://" + Request.Url.Authority
                                        + "/Customer/PaymentGatewayResponse");

                                    if (responsePaymentService.IsSuccess)
                                    {
                                        var tempContainer = new CustomerNewRequestContainer();
                                        tempContainer.RequestFormObj = requestForm;
                                        tempContainer.CardRequestCollection = cardRequests;
                                        tempContainer.Payments = payments;
                                        tempContainer.ProofOfAddressType = proofOfAddressType;
                                        tempContainer.ListOfProofOfAddrss = listOfProofOfAddrss;
                                        tempContainer.SourcceOfFundType = sourcceOfFundType;
                                        tempContainer.ListOfSourceOfFundsDocs = listOfSourceOfFundsDocs;
                                        tempContainer.CardQty = cardQuantity;
                                        tempContainer.CardTypeIds = cardTypeIds;
                                        Session["TempNewCardRequest"] = tempContainer;
                                        statusArray[0] = "205"; // in case of url redidrect set status 205
                                        statusArray[1] = responsePaymentService.Message;
                                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        statusArray[0] = "206";
                                        statusArray[1] = responsePaymentService.Message;
                                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    if (customerDeliveredCards != null && customerDeliveredCards.Count() > 0)
                                    {
                                        requestFormId = new CardService().AddCardRequestForm(requestForm, cardRequests
                                        , payments, false, proofOfAddressType, listOfProofOfAddrss, sourcceOfFundType
                                        , listOfSourceOfFundsDocs);
                                    }
                                    else
                                        requestFormId = new CardService().AddCardRequestForm(requestForm, cardRequests
                                        , payments, true, proofOfAddressType, listOfProofOfAddrss, sourcceOfFundType
                                        , listOfSourceOfFundsDocs);
                                }
                            }
                            else
                            {
                                if (customerDeliveredCards != null && customerDeliveredCards.Count() > 0)
                                {
                                    requestFormId = new CardService().AddCardRequestForm(requestForm, cardRequests
                                    , payments, false, proofOfAddressType, listOfProofOfAddrss, sourcceOfFundType
                                    , listOfSourceOfFundsDocs);
                                }
                                else
                                    requestFormId = new CardService().AddCardRequestForm(requestForm, cardRequests
                                        , payments, true, proofOfAddressType, listOfProofOfAddrss, sourcceOfFundType
                                        , listOfSourceOfFundsDocs);
                            }

                            if (requestFormId > 0)
                            {
                                if (userSession != null && userSession.IsCustomer &&
                                    (customerDeliveredCards != null && customerDeliveredCards.Count() > 0))
                                {
                                    statusArray[0] = "208";
                                    statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Card request submitted successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/MyProfile'>Go back</a></span>";
                                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                                }
                                if (userSession != null && userSession.IsCustomer)
                                {
                                    if (!string.IsNullOrEmpty(hfCustomerId))
                                    {
                                        var updatedCustomer = new CustomerService()
                                             .GetCustomer(Convert.ToInt32(hfCustomerId));

                                        var content = utility.DownloadCustomerFilledForm(updatedCustomer, userSession,
                                            cardTypeIds, cardQuantity, requestForm.ApplicationNumber);
                                        statusArray[3] = content[0];
                                        statusArray[4] = content[1];

                                        statusArray[0] = "200";
                                        statusArray[1] = "" + requestFormId;//"<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Card request submitted successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/MyProfile'>Go back</a></span>";
                                        statusArray[2] = "Step5"; //Changing this to step3 from Step4
                                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    if (customerDeliveredCards != null && customerDeliveredCards.Count() > 0)
                                    {
                                        statusArray[0] = "900";
                                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Card request submitted successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/ExistingCustomersForCardRequests'>Go back</a></span>";//res; show user success message                                        
                                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                                    }

                                    else
                                    {
                                        statusArray[0] = "200";
                                        statusArray[1] = new Utility().UrlSafeEncrypt(hfCustomerId);//res; Send customer id back to client for next step
                                        statusArray[2] = "Step3";
                                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            else
                            {
                                statusArray[0] = "101";
                                statusArray[1] = "Failed to process request.";
                            }
                        }
                        catch (UserException exp)
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Failed: " + exp.Message;
                        }
                        catch (Exception exp)
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
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
        [RoleActionValidator]
        public ActionResult ExistingCustomersForCardRequests(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().ResultPageSize;
                var result = new CardService().GetCustomerWithDeliveredCards(out totalCount, pageIndex: page
                                                    , pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(result);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }
        }

        [HttpPost]
        [RoleActionValidator]
        public ActionResult ExistingCustomersForCardRequests(FormCollection formCollection)
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
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            int pageSize = new SettingService().ResultPageSize;

            try
            {
                int totalCount = 0;
                var result = new CardService().GetCustomerWithDeliveredCards(out totalCount, nic: nic
                    , passportNo: passport, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo
                                        , pageSize: pageSize, pageIndex: page);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(result);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }
        }

        [RoleActionValidator]
        [HttpGet]
        public ActionResult NewCustomerSignedForm(string id)
        {
            var userSession = Session["CurrentSession"] as User;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    //Decode Id here
                    id = new Utility().UrlSafeDecrypt(id);
                }
                else
                {
                    //When customer register online after adding new card requests next page will be 
                    // Customer Signed Form
                    if (userSession != null && userSession.IsCustomer && string.IsNullOrEmpty(id))
                        id = userSession.Id.ToString();
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(id, "^[0-9]*$"))
                {
                    if (userSession != null && userSession.IsCustomer)
                    {
                        return RedirectToAction("MyProfile", "Customer");
                    }
                    return RedirectToAction("NewCustomer", "Customer");
                }
                if (new CustomerService().IsCustomerExist(Convert.ToInt32(id)) && id.Length >= 1 && id.Length <= 15)
                {
                    ViewBag.CustomerId = new Utility().UrlSafeEncrypt(id);
                    return View();
                }

                if (userSession != null && userSession.IsCustomer)
                {
                    return RedirectToAction("MyProfile", "Customer");
                }
                return RedirectToAction("NewCustomer", "Customer");
            }
            catch (Exception exp)
            {
                if (userSession != null && userSession.IsCustomer)
                {
                    return RedirectToAction("MyProfile", "Customer");
                }
                return RedirectToAction("NewCustomer", "Customer");
            }
        }

        [RoleActionValidator]
        public JsonResult NewCustomerSignedForm(FormCollection formCollection, string hfCustomerId, HttpPostedFileBase customerSignedCopyEng, HttpPostedFileBase customerSignedCopyCl)
        {
            var statusArray = new string[5];
            var customerSession = Session["CurrentCustomer"] as Customer;
            LocalizationService localizationService = null;
            if (!string.IsNullOrEmpty(formCollection["txtHidden"]))
            {
                if (ResendKYCEmails() > 0)
                {
                    statusArray[0] = "203";
                    statusArray[1] = "Email Sent Successfully";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }

                statusArray[0] = "204";
                statusArray[1] = "Email cannot be sent, please click on download button to manually download the forms";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }

            if (customerSession == null)
            {
                localizationService = new LocalizationService();
            }
            else
            {
                localizationService = new LocalizationService(customerSession.LanguageId);
            }
            if ((string.IsNullOrEmpty(Request.Form["hfIsCallForDownload"]) || Request.Form["hfIsCallForDownload"] == "false") && !string.IsNullOrEmpty(hfCustomerId) && customerSignedCopyEng != null && customerSignedCopyCl != null)
            {

                hfCustomerId = new Utility().UrlSafeDecrypt(hfCustomerId);
                if (string.IsNullOrEmpty(hfCustomerId))
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid value please try again.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                var documentService = new DocumentService();
                var customerService = new CustomerService();
                var utility = new Utility();
                var customer = customerService.GetCustomer(Convert.ToInt32(hfCustomerId));
                string customerSignedCopyPath = string.Empty;
                string customerSignedCopyClPath = string.Empty;
                if (customerSignedCopyEng != null && !utility.IsValidImage(customerSignedCopyEng))
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Invalid Customer Signed English Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                else if (customerSignedCopyCl != null && !utility.IsValidImage(customerSignedCopyCl))
                {
                    statusArray[0] = "101";
                    statusArray[1] =
                        "Invalid Customer Signed Persian Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                if (customerSignedCopyEng != null && customerSignedCopyEng.ContentLength > 0)
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
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                if (customerSignedCopyCl != null && customerSignedCopyCl.ContentLength > 0)
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
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    var res = customerService.UploadCustomerSignedForm(Convert.ToInt32(hfCustomerId),
                        customerSignedCopyPath, customerSignedCopyClPath);
                    if (res)
                    {
                        statusArray[0] = "200";
                        statusArray[2] = "";
                        var userSession = Session["CurrentSession"] as User;
                        statusArray[1] = userSession != null && userSession.IsCustomer // In case of customer new card requests
                            ? "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i>" + localizationService.GetResource("Cnc.Complete", null, "Complete") + "</strong></span><br />" + localizationService.GetResource("Cnc.CustomerSignedDocsSavedSuccessfully", null, "Customer signed documents has been saved successfully!") + "<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/MyProfile'>" + localizationService.GetResource("Cnc.GoBack", null, "Go Back") + "</a></span>"
                            : "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> " + localizationService.GetResource("Cnc.Complete", null, "Complete") + "</strong></span><br />" + localizationService.GetResource("Cnc.KycFormSavedSuccessfully", null, "KYC form has been saved successfully!") + "<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/NewCustomer'>" + localizationService.GetResource("Cnc.GoBack", null, "Go Back") + "</a></span>";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to process request.";
                    }
                }
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                }
            }
            else if (!string.IsNullOrEmpty(hfCustomerId) && !string.IsNullOrEmpty(Request.Form["hfIsCallForDownload"])
                && Request.Form["hfIsCallForDownload"] == "true")
            {
                //Download
                var userSession = Session["CurrentSession"] as User;
                if (userSession != null && userSession.IsCustomer)
                {
                    var customer = new CustomerService().GetCustomer(userSession.Id, false);
                    var content = new Utility()
                        .DownloadCustomerFilledForm(customer, userSession, customer.CardRequestForms[0]);
                    statusArray[3] = content[0];
                    statusArray[4] = content[1];

                    statusArray[0] = "200";
                    statusArray[1] = "" + customer.UserId;//"<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Card request submitted successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/MyProfile'>Go back</a></span>";
                    statusArray[2] = "Step5"; //Changing this to step3 from Step4

                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Missing required info.";
            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }
        public void SaveKycForm(string base64English, string base64Persian)
        {
            try
            {
                var userSession = Session["CurrentSession"] as User;
                if (userSession != null && userSession.IsCustomer)
                {
                    var customer = new CustomerService().GetCustomer(userSession.Id, false);
                    if (!string.IsNullOrEmpty(base64English))
                    {
                        string imgBase64 = base64English.Split(',')[1];
                        byte[] bytes = Convert.FromBase64String(imgBase64);
                        new Utility().Save_EngKycImage(bytes, customer);
                    }
                    if (!string.IsNullOrEmpty(base64Persian))
                    {
                        string imgBase64 = base64Persian.Split(',')[1];
                        byte[] bytes = Convert.FromBase64String(imgBase64);
                        new Utility().Save_PersKycImage(bytes, customer);
                    }
                    new MessageService().SendCustomerKycForms(customer);
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [RoleActionValidator]
        public ActionResult SaveKycFormOnline(FormCollection formCollection)
        {
            var userSession = Session["CurrentSession"] as User;
            if (userSession != null && userSession.IsCustomer)
            {
                var customer = new CustomerService().GetCustomer(userSession.Id, false);
                if (!string.IsNullOrEmpty(formCollection["hiddenFieldEnglish"]))
                {
                    string imgBase64 = Request.Form["hiddenFieldEnglish"].Split(',')[1];
                    byte[] bytes = Convert.FromBase64String(imgBase64);
                    new Utility().Save_EngKycImage(bytes, customer);
                }
                if (!string.IsNullOrEmpty(formCollection["hiddenFieldPersian"]))
                {
                    string imgBase64 = Request.Form["hiddenFieldPersian"].Split(',')[1];
                    byte[] bytes = Convert.FromBase64String(imgBase64);
                    new Utility().Save_PersKycImage(bytes, customer);
                }
                new MessageService().SendCustomerKycForms(customer);
            }
            return Redirect("~/Customer/NewCustomerSignedForm?id="
                + new Utility().UrlSafeEncrypt(userSession.Id.ToString()));
        }

        public int ResendKYCEmails()
        {
            var userSession = Session["CurrentSession"] as User;
            if (userSession != null && userSession.IsCustomer)
            {
                try
                {
                    var customer = new CustomerService().GetCustomer(userSession.Id, false);
                    int result = new MessageService().SendCustomerKycForms(customer);
                    if (result > 0)
                    {
                        return result;
                    }
                    return -1;
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return -1;
                }
            }
            return -1;
        }
        #endregion


        #region Pending Card Requests

        [RoleActionValidator]
        public ActionResult CardRequests(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var cardRequest = new CustomerService().GetCustomersPendingForCardRequests(out totalCount, pageIndex: page,
                pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(cardRequest);
        }

        [RoleActionValidator]
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
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            int pageSize = new SettingService().ResultPageSize;
            var customersPendingForCardRequests = new CustomerService()
                .GetCustomersPendingForCardRequests(out totalCount, nic: nic, passportNo: passport,
               createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(customersPendingForCardRequests);
        }


        #endregion


        #region CustomerSignedDocument

        [RoleActionValidator]
        public ActionResult CustomerSignedDocument(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var cardRequest = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, pageIndex: page, pageSize: pageSize);
            ViewBag.TotalCount = totalCount;
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(cardRequest);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult CustomerSignedDocument(FormCollection formCollection)
        {
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
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
            int pageSize = new SettingService().ResultPageSize;
            ViewBag.TotalCount = totalCount;
            var customersPendingForSignedForm = new CustomerService().GetCustomersPendingForSignedForm(out totalCount, nic: nic, passportNo: passport,
                 createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(customersPendingForSignedForm);
        }

        #endregion


        #region Customer Validation

        [RoleActionValidator]
        public ActionResult CustomerValidation(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var cardRequest = new CustomerService()
                .GetCustomersPendingForValidatationOrFailed(out totalCount, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(cardRequest);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult CustomerValidation(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
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
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            int pageSize = new SettingService().ResultPageSize;
            var customersPendingForValidatationOrFailed = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize
                , showFailed: isShowFailedPayments);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(customersPendingForValidatationOrFailed);
        }

        [RoleActionValidator]
        public JsonResult UpdateCustomerValidateStatus(string key, string failureReason)
        {
            var statusArray = new string[3];
            var customerService = new CustomerService();
            if (!string.IsNullOrEmpty(key) && string.IsNullOrEmpty(failureReason))
            {
                //Completed
                try
                {
                    key = new Utility().UrlSafeDecrypt(key);
                    if (key != null && key != "")
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
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to process the request";
                    }
                }
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                }
                catch (Exception ex)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry we are unable to process your request";
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
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                }
                catch (Exception ex)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry we are unable to process your request";
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


        //#region Translation

        //[RoleActionValidator]
        //public ActionResult CustomerTranslation(int page = 0)
        //{
        //    int totalCount = 0;
        //    int pageSize = new SettingService().ResultPageSize;
        //    var cardRequest = new CustomerService().GetCustomersPendingForTranslation(out totalCount, pageIndex: page
        //                                            , pageSize: pageSize);
        //    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
        //    this.ViewBag.Page = page;
        //    return View(cardRequest);
        //}

        //[RoleActionValidator]
        //[HttpPost]
        //public ActionResult CustomerTranslation(FormCollection formCollection, HttpPostedFileBase proofOfSourceOfAddress
        //                , HttpPostedFileBase proofOfSourceOfFunds)
        //{
        //    string nic = null;
        //    string passport = null;
        //    DateTime? createdDateFrom = null;
        //    DateTime? createdDateTo = null;

        //    if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
        //        nic = Request.Form["txtNICNo"];

        //    if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
        //        passport = Request.Form["txtPassportNo"];

        //    if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
        //    {
        //        DateTime txtCreatedDateFrom;
        //        if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
        //        {
        //            return View();
        //        }

        //        createdDateFrom = txtCreatedDateFrom;
        //    }

        //    if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
        //    {
        //        DateTime txtCreatedDateTo;
        //        if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
        //        {
        //            return View();
        //        }

        //        createdDateTo = txtCreatedDateTo;
        //    }
        //    int totalCount = 0;
        //    int page = 0;
        //    if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
        //    {
        //        page = Convert.ToInt32(Request.Form["btnPagination"]);
        //    }
        //    int pageSize = new SettingService().ResultPageSize;
        //    var customersPendingForTranslation = new CustomerService().GetCustomersPendingForTranslation(out totalCount
        //                                            , nic: nic, passportNo: passport,
        //         createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
        //    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
        //    this.ViewBag.Page = page;
        //    return View(customersPendingForTranslation);
        //}

        //[RoleActionValidator]
        //[HttpPost]
        //public JsonResult CustomerTranslationUploadFiles()
        //{
        //    var statusArray = new string[3];
        //    var listOfSourceOfFundsDocs = new List<string>();
        //    var listOfProofOfAddrss = new List<string>();
        //    try
        //    {
        //        if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
        //        {
        //            var customerService = new CustomerService();
        //            var customerId = System.Web.HttpContext.Current.Request["CustId"]; //Put validations
        //            var customer = customerService.GetCustomer(Convert.ToInt32(customerId));
        //            var documentService = new DocumentService();
        //            var allFiles = System.Web.HttpContext.Current.Request.Files;
        //            int flagCounterForFunds = 1;
        //            int flagCounterForAddress = 1;
        //            foreach (var file in allFiles)
        //            {
        //                //TODO: Name of file is currentyl hardcoded later we will fix 
        //                //currently if we don't do this we have to write seperatly for each file
        //                if (file.ToString().Contains("SourceOfAddress"))
        //                {
        //                    var proofOfSourceOfAddressScanCopy = System.Web.HttpContext.Current.Request.Files[file.ToString()];
        //                    if (proofOfSourceOfAddressScanCopy != null && proofOfSourceOfAddressScanCopy.ContentLength > 0)
        //                    {
        //                        var utility = new Utility();
        //                        if (!utility.IsValidImageClientSideUploading(proofOfSourceOfAddressScanCopy))
        //                        {
        //                            statusArray[0] = "101";
        //                            statusArray[1] = "Invalid image type or size. Maximum size 1MB format shoulde be .jpg/.Jpeg/.png ";
        //                            return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                        }
        //                        string extension = Path.GetExtension(proofOfSourceOfAddressScanCopy.FileName);
        //                        string newFileName = customer.NIC + "-Address" + flagCounterForAddress + extension;
        //                        var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                        proofOfSourceOfAddressScanCopy.SaveAs(path);
        //                        listOfProofOfAddrss.Add(newFileName);
        //                        flagCounterForAddress++;
        //                    }

        //                }
        //                else if (file.ToString().Contains("SourceOfFunds"))
        //                {
        //                    var proofOfSourceOfFundsScanCopy =
        //                       System.Web.HttpContext.Current.Request.Files[file.ToString()];
        //                    var utility = new Utility();
        //                    if (!utility.IsValidImageClientSideUploading(proofOfSourceOfFundsScanCopy))
        //                    {
        //                        statusArray[0] = "101";
        //                        statusArray[1] = "Invalid image type or size. Maximum size 1MB format shoulde be .jpg/.Jpeg/.png ";
        //                        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                    }

        //                    if (proofOfSourceOfFundsScanCopy != null &&
        //                        proofOfSourceOfFundsScanCopy.ContentLength > 0)
        //                    {
        //                        string extension = Path.GetExtension(proofOfSourceOfFundsScanCopy.FileName);
        //                        string newFileName = customer.NIC + "-Funds" + flagCounterForFunds + extension;
        //                        var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                        proofOfSourceOfFundsScanCopy.SaveAs(path);
        //                        listOfSourceOfFundsDocs.Add(newFileName);
        //                        flagCounterForFunds++;
        //                    }
        //                }
        //            }

        //            if (listOfSourceOfFundsDocs.Count > 0 && listOfProofOfAddrss.Count > 0 && !string.IsNullOrEmpty(customerId))
        //            {
        //                try
        //                {
        //                    var result = customerService.UploadTranslatedDocuments(Convert.ToInt32(customerId),
        //                    listOfProofOfAddrss, listOfSourceOfFundsDocs);
        //                    if (result)
        //                    {
        //                        statusArray[0] = "200";
        //                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Files submitted successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CustomerTranslation'>Go back</a></span>";
        //                        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //                catch (UserException exp)
        //                {
        //                    statusArray[0] = "101";
        //                    statusArray[1] = "Failed: " + exp.Message;
        //                    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                }
        //                catch (Exception exp)
        //                {
        //                    statusArray[0] = "101";
        //                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
        //                    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "Required fields missing!";
        //            }

        //        }
        //        else
        //        {
        //            statusArray[0] = "101";
        //            statusArray[1] = "Required fields missing!";
        //        }
        //    }
        //    catch (UserException exp)
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Failed: " + exp.Message;
        //        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exp)
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
        //        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //}


        //#endregion


        #endregion


        //#region AuthoritySignedForm

        //[RoleActionValidator]
        //public ActionResult AuthoritySignedForm(int page = 0)
        //{
        //    int totalCount = 0;
        //    int pageSize = new SettingService().ResultPageSize;
        //    var customerPendingForAuthoritySignature = new CustomerService()
        //        .GetCustomersPendingForAuthoritySignature(out totalCount, pageIndex: page, pageSize: pageSize);
        //    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
        //    this.ViewBag.Page = page;
        //    return View(customerPendingForAuthoritySignature);
        //}

        //[RoleActionValidator]
        //[HttpPost]
        //public ActionResult AuthoritySignedForm(FormCollection formCollection)
        //{
        //    if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
        //        return Redirect(Request.UrlReferrer.ToString());

        //    string nic = null;
        //    string passport = null;
        //    DateTime? createdDateFrom = null;
        //    DateTime? createdDateTo = null;
        //    int totalCount = 0;
        //    int page = 0;
        //    if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
        //    {
        //        page = Convert.ToInt32(Request.Form["btnPagination"]);
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
        //        nic = Request.Form["txtNICNo"];

        //    if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
        //        passport = Request.Form["txtPassportNo"];

        //    if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
        //    {
        //        DateTime txtCreatedDateFrom;
        //        if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
        //        {
        //            return View();
        //        }

        //        createdDateFrom = txtCreatedDateFrom;
        //    }

        //    if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
        //    {
        //        DateTime txtCreatedDateTo;
        //        if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
        //        {
        //            return View();
        //        }

        //        createdDateTo = txtCreatedDateTo;
        //    }
        //    int pageSize = new SettingService().ResultPageSize;
        //    var customersPendingForAuthoritySignature = new CustomerService().GetCustomersPendingForAuthoritySignature(out totalCount, nic: nic, passportNo: passport,
        //          createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
        //    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
        //    this.ViewBag.Page = page;
        //    return View(customersPendingForAuthoritySignature);
        //}

        //[RoleActionValidator]
        //[HttpPost]
        //public JsonResult AuthoritySignedFormUploadFile()
        //{
        //    var statusArray = new string[3];
        //    try
        //    {
        //        if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
        //        {
        //            var authoritySignedFormEng = System.Web.HttpContext.Current.Request.Files["AuthoritySignedFormEng"];
        //            var authoritySignedFormPer = System.Web.HttpContext.Current.Request.Files["AuthoritySignedFormPer"];
        //            var customerId = System.Web.HttpContext.Current.Request["CustId"];

        //            if (authoritySignedFormEng != null && authoritySignedFormPer != null && !string.IsNullOrEmpty(customerId))
        //            {
        //                var utility = new Utility();
        //                if (!utility.IsValidImageClientSideUploading(authoritySignedFormEng) || !utility.IsValidImageClientSideUploading(authoritySignedFormPer))
        //                {
        //                    statusArray[0] = "101";
        //                    statusArray[1] = "Invalid image type or size. Maximum size 1MB format shoulde be .jpg/.Jpeg/.png ";
        //                    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                }
        //                string authoritySignedFormPerName;
        //                string authoritySignedFormEngName;
        //                //get customer on base of customerId
        //                var customerService = new CustomerService();
        //                var customer = customerService.GetCustomer(Convert.ToInt32(customerId));
        //                var documentService = new DocumentService();
        //                if (authoritySignedFormEng.ContentLength > 0)
        //                {
        //                    string extension = Path.GetExtension(authoritySignedFormEng.FileName);
        //                    string newFileName = customer.NIC + "-KYCAuthoritySigned" + extension;
        //                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                    authoritySignedFormEng.SaveAs(path);
        //                    authoritySignedFormEngName = newFileName;
        //                }
        //                else
        //                {
        //                    statusArray[0] = "101";
        //                    statusArray[1] = "English AuthoritySignedForm Document is missing";
        //                    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                }
        //                if (authoritySignedFormPer.ContentLength > 0)
        //                {
        //                    string extension = Path.GetExtension(authoritySignedFormPer.FileName);
        //                    string newFileName = customer.NIC + "-KYCAuthoritySignedCL" + extension;
        //                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //                    authoritySignedFormPer.SaveAs(path);
        //                    authoritySignedFormPerName = newFileName;
        //                }
        //                else
        //                {
        //                    statusArray[0] = "101";
        //                    statusArray[1] = "Persian AuthoritySignedForm Document is missing";
        //                    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                }
        //                if (!string.IsNullOrEmpty(authoritySignedFormEngName) && !string.IsNullOrEmpty(authoritySignedFormPerName))
        //                {
        //                    try
        //                    {//temp
        //                        var result = true;//customerService.UploadAuthoritySignedForm(Convert.ToInt32(customerId),authoritySignedFormEngName, authoritySignedFormPerName);
        //                        if (result)
        //                        {
        //                            statusArray[0] = "200";
        //                            statusArray[1] = "Documents uploaded successfully";
        //                            return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    catch (UserException exp)
        //                    {
        //                        statusArray[0] = "101";
        //                        statusArray[1] = "Failed: " + exp.Message;
        //                        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                    }
        //                    catch (Exception exp)
        //                    {
        //                        statusArray[0] = "101";
        //                        statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
        //                        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //                    }

        //                }

        //            }
        //            else
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "Required fields missing!";
        //            }
        //        }
        //    }
        //    catch (UserException exp)
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Failed: " + exp.Message;
        //        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {

        //        statusArray[0] = "101";
        //        statusArray[1] = "Failed to update Documents";
        //        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //}

        //[RoleActionValidator]
        //public JsonResult SetAuthoritySignedFormUploaded(string customerId)
        //{
        //    var statusArray = new string[3];
        //    if (!string.IsNullOrEmpty(customerId))
        //    {
        //        try
        //        {
        //            var result = new CustomerService().SetAuthoritySignedFormUploaded(Convert.ToInt32(customerId));
        //            if (result)
        //            {
        //                statusArray[0] = "200";
        //                statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Form has been marked successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/AuthoritySignedForm'>Go back</a></span>";
        //            }
        //            else
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "Unable to update data please try again";
        //            }
        //        }
        //        catch (UserException exp)
        //        {
        //            statusArray[0] = "101";
        //            statusArray[1] = "Failed: " + exp.Message;
        //        }
        //        catch (Exception exp)
        //        {
        //            statusArray[0] = "101";
        //            statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
        //        }
        //    }
        //    else
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Unable to update data please try again";
        //    }
        //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        [RoleActionValidator]
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
        [RoleActionValidator]
        [HttpGet]
        public ActionResult Customers(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().ResultPageSize;
                var customers = new CustomerService().GetCustomers(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(customers);
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        [RoleActionValidator]
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
            string firstName = null;
            string lastName = null;
            string contactNo = null;
            DateTime? creationDate = null;
            string emailAddress = null;
            string clientId = null;
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

            if (!string.IsNullOrEmpty(formCollection["txtFirstName"]))
            {
                firstName = Request.Form["txtFirstName"];
            }
            if (!string.IsNullOrEmpty(formCollection["txtLastName"]))
            {
                lastName = Request.Form["txtLastName"];
            }
            if (!string.IsNullOrEmpty(formCollection["txtEmail"]))
            {
                emailAddress = Request.Form["txtEmail"];
            }
            if (!string.IsNullOrEmpty(formCollection["txtClientId"]))
            {
                clientId = Request.Form["txtClientId"];
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["ddlCustomerStatus"]))
                customerStatus = (CustomerStatus)Enum.Parse(typeof(CustomerStatus), Request.Form["ddlCustomerStatus"]);
            int pageSize = new SettingService().ResultPageSize;
            var customers = new CustomerService().GetCustomers(out totalCount, nic: nic, passportNo: passport,
                customerStatus: customerStatus, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo,
                emailAddress: emailAddress, clientId: clientId, contactNo: contactNo, pageIndex: page,
                pageSize: pageSize, firstName: firstName, lastName: lastName);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(customers);
        }

        [RoleActionValidator]
        public JsonResult SetCustomerApprovedOrDeclined(string key, string failureReason, string emailAddress
            , string billingAddress)
        {
            var statusArray = new string[3];
            var customerService = new CustomerService();
            if (!string.IsNullOrEmpty(key) && string.IsNullOrEmpty(failureReason) && !string.IsNullOrEmpty(emailAddress)
                && !string.IsNullOrEmpty(billingAddress))
            {
                //Completed
                try
                {
                    if (customerService.IsCustomerExist(Convert.ToInt32(key)))
                    {
                        int customerId = Convert.ToInt32(key);
                        var res = customerService.SetCustomerApprovedOrDeclined(Convert.ToInt32(customerId)
                            , billingAddress, emailAddress, null);
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
                        statusArray[1] = "Customer Not Found";
                    }
                }
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;

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
                    var res = customerService.SetCustomerApprovedOrDeclined(Convert.ToInt32(customerId), null, null
                        , failureReason);
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
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
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
        [RoleActionValidator]
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

        [RoleActionValidator]
        public ActionResult MyProfile()
        {
            //return RedirectToAction("NewCustomerSignedForms");
            var userSession = Session["CurrentSession"] as User;
            if (userSession != null)
            {
                var customer = new CustomerService().GetCustomer(userSession.Id, true);
                return View(customer);
            }
            return RedirectToAction("Login", "Account");
        }

        [RoleActionValidator]
        [HttpGet]
        public JsonResult CustomerProfile(string customerId)
        {
            var statusArray = new object[3];
            try
            {
                if (!string.IsNullOrEmpty(customerId))
                {
                    var customer = new CustomerService().GetCustomer(Convert.ToInt32(customerId), true);
                    var customerProfile = new CustomerProfileModel();
                    customerProfile.FirstName = customer.User.FirstName;
                    customerProfile.LastName = customer.User.LastName;
                    customerProfile.DateOfBirth = customer.DateOfBirth;
                    customerProfile.DateOfBirthInCustomerLanguage = customer.DateOfBirthInCustomerLanguage;
                    customerProfile.NIC = customer.NIC;
                    //MaritalStatusId
                    customerProfile.PassportNo = customer.PassportNo;
                    customerProfile.Nationality = customer.Nationality;
                    customerProfile.Address = customer.Address;
                    customerProfile.AddressInCustomerLanguage = customer.AddressInCustomerLanguage;
                    customerProfile.Address2 = customer.Address2;
                    customerProfile.Address2InCustomerLanguage = customer.Address2InCustomerLanguage;

                    customerProfile.EmailAddress = customer.User.Email;
                    customerProfile.EmailForShopping = string.IsNullOrEmpty(customer.EmailForShopping) ? "N/A" : customer.EmailForShopping;
                    customerProfile.BillingAddress = string.IsNullOrEmpty(customer.BillingAddress) ? "N/A" : customer.BillingAddress;
                    //CityId
                    customerProfile.PostalCode = customer.PostalCode;
                    //CountryId
                    customerProfile.ContactNo = customer.ContactNo;
                    //genderId
                    customerProfile.Gender = "" + (Gender)customer.GenderId;
                    //MaritalStatus
                    customerProfile.MaritalStatus = "" + (MaritalStatus)customer.MaritalStatusId;
                    customerProfile.CityName = customer.City.Name;
                    customerProfile.CountryName = customer.Country.Name;
                    //var custoerProfile = JsonConvert.SerializeObject(customerProfile, Formatting.Indented, new JsonSerializerSettings()
                    //{
                    //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    //});

                    statusArray[0] = "200";
                    statusArray[1] = customerProfile;
                }
                else
                {
                    statusArray[0] = "101";
                    statusArray[1] = "No record found.";
                }
            }
            catch (Exception exp)
            {
                statusArray[0] = "101";
                statusArray[1] = "Unable to process.";
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get single customer based on filter criteria
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="nic"></param>
        /// <param name="securityQuestion"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        [RoleActionValidator]
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
            catch (UserException exp)
            {
                return Json(new { success = false, responseText = "Failed: " + exp.Message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "An error occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [RoleActionValidator]
        public ActionResult Customer()
        {
            return View();
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult GetCustomers(FormCollection formCollection)
        {
            string nic = Convert.ToString(formCollection["NIC"]);
            DateTime regDateFrom = DateTime.ParseExact(formCollection["RegDateFrom"], "d/m/yyyy", null);
            DateTime regDateTo = DateTime.ParseExact(formCollection["RegDateTo"], "d/m/yyyy", null);
            int statusId = int.Parse(formCollection["StatusDDL"]);
            int totalCounts = 0;
            var customerSevice = new CustomerService();
            customerSevice.GetCustomers(out totalCounts, nic: nic, createdDateFrom: regDateFrom, createdDateTo: regDateTo);//, statusId);

            return View();
        }

        [RoleActionValidator]
        [HttpGet]
        public JsonResult GetCustmers(string cardNo = null, string nic = null, DateTime? createdDateFrom = null
            , DateTime? createdDateTo = null, UserStatus? status = null)
        {
            int totalCounts = 0;
            var customerService = new CustomerService();
            var customers = customerService.GetCustomers(out totalCounts, nic: nic, createdDateFrom: createdDateFrom, createdDateTo: createdDateTo
                , userStatus: status);
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Will use for Dashboard
        /// </summary>
        [RoleActionValidator]
        public int GetCustomersCountByStatus(CustomerStatus customerStatus)
        {
            return new CustomerService().GetCustomersCountByStatus(customerStatus);
        }

        #endregion

        #endregion Customers

        //#region Existing Customer Card Request

        //[RoleActionValidator]
        //public ActionResult CustomerCardRequestValidation(int page = 0)
        //{
        //    int totalCount = 0;
        //    int pageSize = new SettingService().ResultPageSize;
        //    var customerPendingCardRequests = new CustomerService()
        //        .GetCustomerCardRequestPendingForValidatationOrFailed(out totalCount, pageIndex: page
        //                    , pageSize: pageSize);
        //    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
        //    this.ViewBag.Page = page;
        //    return View(customerPendingCardRequests);            
        //}

        //#endregion

        #region Change Password

        [RoleActionValidator]
        public ActionResult ChangePassword()
        {
            //var userSession = Session["CurrentSession"] as User;
            //if (userSession != null)
            //{
            //    if (userSession.IsCustomer && userSession.ChangePasswordRequired)
            //    {
            //        return View();
            //    }else if (userSession.IsCustomer){
            //        return View();
            //    }else
            //        return RedirectToAction("Error", "Home");
            //}

            //return RedirectToAction("Error", "Home");
            return View();

        }

        [RoleActionValidator]
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
                    ViewBag.ResponseMessage = "Password successfully changed, please login with your new password";
                    FormsAuthentication.SignOut();
                    // clear authentication cookie
                    HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                    cookie1.Expires = DateTime.Now.AddYears(-1);
                    Response.Cookies.Add(cookie1);
                    // AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("LogOff", "Account");
                }
                else
                {
                    ViewBag.ResponseCode = "101";
                    //ViewBag.ResponseMessage = "Failed to change password. Please try again.";
                    ViewBag.ResponseMessage = "Old Passowrd is not Correct, Please enter correct Old password";
                }
            }
            return View();
        }

        #endregion

        [RoleActionValidator]
        public ActionResult ValidateKYCForms(int? customerId)
        {
            int totalCount = 0;
            var customers = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount);
            return View(customers);
        }

        [RoleActionValidator]
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
            int totalCount = 0;
            var customers = new CustomerService().GetCustomersPendingForValidatationOrFailed(out totalCount, nic: nic, passportNo: passportNo, createdDateTo: createdDateTo, createdDateFrom: createdDateFrom);
            return View(customers);
        }

        [RoleActionValidator]
        [HttpGet]
        public JsonResult UpdateCustomerValidationStatus(int customerId, string failureReason)
        {
            bool IsUpdated = new CustomerService().SetCustomerValidated(customerId, failureReason);
            return Json(new { IsUpdate = IsUpdated, CustomerId = customerId }, JsonRequestBehavior.AllowGet);
        }

        //[RoleActionValidator]
        //public ActionResult SendToCardProvider(int page = 0)
        //{
        //    int totalCount = 0;
        //    int pageSize = new SettingService().ResultPageSize;
        //    var customerPendingToSendToCardIssuer = new CustomerService().GetCustomersPendingToSendToCardIssuer(out totalCount, pageIndex: page, pageSize: pageSize);
        //    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
        //    this.ViewBag.Page = page;
        //    return View(customerPendingToSendToCardIssuer);
        //}

        //[RoleActionValidator]
        //[HttpPost]
        //public ActionResult SendToCardProvider(FormCollection formCollection)
        //{
        //    if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
        //        return Redirect(Request.UrlReferrer.ToString());

        //    string nic = null;
        //    string passport = null;
        //    DateTime? createdDateFrom = null;
        //    DateTime? createdDateTo = null;
        //    int totalCount = 0;
        //    int page = 0;

        //    if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
        //    {
        //        page = Convert.ToInt32(Request.Form["btnPagination"]);
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
        //        nic = Request.Form["txtNICNo"];

        //    if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
        //        passport = Request.Form["txtPassportNo"];

        //    if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateFrom"]))
        //    {
        //        DateTime txtCreatedDateFrom;
        //        if (!DateTime.TryParse(Request.Form["txtRegistrationDateFrom"], out txtCreatedDateFrom))
        //        {
        //            return View();
        //        }

        //        createdDateFrom = txtCreatedDateFrom;
        //    }

        //    if (!string.IsNullOrEmpty(formCollection["txtRegistrationDateTo"]))
        //    {
        //        DateTime txtCreatedDateTo;
        //        if (!DateTime.TryParse(Request.Form["txtRegistrationDateTo"], out txtCreatedDateTo))
        //        {
        //            return View();
        //        }

        //        createdDateTo = txtCreatedDateTo;
        //    }

        //    int pageSize = new SettingService().ResultPageSize;
        //    var customersPendingToSendToCardIssuer = new CustomerService()
        //        .GetCustomersPendingToSendToCardIssuer(out totalCount, nic: nic, passportNo: passport,
        //          createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);
        //    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
        //    this.ViewBag.Page = page;
        //    return View(customersPendingToSendToCardIssuer);
        //}

        //[RoleActionValidator]
        //public JsonResult UpdateCustomerCardIssuerStatus(string customerId)
        //{
        //    var statusArray = new string[3];
        //    if (!string.IsNullOrEmpty(customerId))
        //    {
        //        try
        //        {
        //            bool isUpdated = new CustomerService().SendCustomerToCardIssuer(Convert.ToInt32(customerId));
        //            if (isUpdated)
        //            {
        //                statusArray[0] = "200";
        //                statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Data sent successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/SendToCardProvider'>Go back</a></span>";
        //            }
        //            else
        //            {
        //                statusArray[0] = "101";
        //                statusArray[1] = "Unable to update data please try again";
        //            }
        //        }
        //        catch (UserException exp)
        //        {
        //            statusArray[0] = "101";
        //            statusArray[1] = "Failed: " + exp.Message;
        //        }
        //        catch (Exception exp)
        //        {
        //            statusArray[0] = "101";
        //            statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
        //        }
        //    }
        //    else
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Unable to update data please try again";
        //    }
        //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //}

        #region LAMDA Response

        [RoleActionValidator]
        public ActionResult CardIssuerResponse(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var customerWaitingForCardIssuerResponseOrDeclined = new CustomerService()
                .GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(customerWaitingForCardIssuerResponseOrDeclined);
        }

        [RoleActionValidator]
        [HttpPost]
        public ActionResult CardIssuerResponse(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int totalCount = 0;
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
            int pageSize = new SettingService().ResultPageSize;
            var customersWaitingForCardIssuerResponseOrDeclined = new CustomerService()
                .GetCustomersWaitingForCardIssuerResponseOrDeclined(out totalCount, nic: nic, passportNo: passport,
                   createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize
                   , showFailed: isShowFailedPayments);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(customersWaitingForCardIssuerResponseOrDeclined);
        }

        #endregion

        #region Customer Payments

        [RoleActionValidator]
        public ActionResult Payments(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var userSession = Session["CurrentSession"] as User;

            if (userSession != null)
            {
                var customerPayments = new PaymentService().GetPayments(out totalCount, userSession.Id, pageIndex:
                                                                             page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0); this.ViewBag.Page = page;
                try
                {

                    if (customerPayments != null && customerPayments.Count > 0)
                    {
                        return View(customerPayments);
                    }
                }
                catch (UserException exp)
                {
                    return View();
                }
                catch (Exception exp)
                {
                    return View();
                }
            }

            return View();
        }
        #endregion

        #region Online Customer Registration
        /// <summary>
        /// This method is used to verify customer
        /// </summary>
        [AllowAnonymous]
        public ActionResult Activate(string cust)
        {

            string message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(cust))
                {
                    var key = new Utility().UrlSafeDecrypt(cust);
                    if (!string.IsNullOrEmpty(key))
                    {
                        if (new CustomerService().ActivateCustomer(Convert.ToInt32(key)))
                            message = "Your account has been activated.";
                    }
                    else { message = "Invalid Customer"; }
                }
                else
                {
                    message = "Invalid Customer";
                }
            }
            catch (UserException exception)
            {
                log.Error(exception.Message);
                message = exception.Message;
            }
            catch (Exception)
            {
                message = "Activation failed, please try again later";
            }

            ViewBag.MessagePass = message;
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Signup(string affiliateId)
        {
            //string cookieName = "CnCAffiliateId";

            //if (!string.IsNullOrEmpty(affiliateId))
            //{
            //    var affiliate = new AffiliateService().GetAffiliate(Convert.ToInt32(affiliateId));

            //    if (affiliate != null)
            //    {
            //        HttpCookie myCookie = new HttpCookie(cookieName);
            //        myCookie.Value = affiliate.Id.ToString();
            //        myCookie.Expires = DateTime.Now.AddDays(30);
            //        Response.Cookies.Add(myCookie);
            //        ViewBag.AffiliateId = affiliate.Id;
            //    }
            //}
            //new CardService().CardRequestFinancialEvaluation();
            if (ViewBag.AffiliateId == null)
            {
                ViewBag.AffiliateId = affiliateId;
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [CaptchaValidation("CaptchaCode", "ExampleCaptcha", "Incorrect CAPTCHA code!")]
        public JsonResult Signup(FormCollection formCollection, HttpPostedFileBase NICDocCustomer
                                   , HttpPostedFileBase PassportDocCustomer)
        {
            var statusArray = new string[5];
            if (!ModelState.IsValid)
            {
                statusArray[0] = "102";
                statusArray[1] = "Captcha Failed! prove you are not robot";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            else
            {
                statusArray[0] = "102";
                statusArray[1] = "Captcha Passed!";
                //return Json(statusArray, JsonRequestBehavior.AllowGet);
                // TODO: Captcha validation passed, proceed with protected action  
            }
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

            var userSession = Session["CurrentSession"] as User;
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

            if (userSession == null)
            {
                string customerPassword = Request.Form["txtPassword"];
                string customerRePassword = Request.Form["txtRePassword"];
                if (string.IsNullOrEmpty(customerPassword) || string.IsNullOrEmpty(customerRePassword))
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Password is a required field";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                if (customerPassword != customerRePassword)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Password mismatched";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                if (customerPassword.Length < 8 || customerPassword.Length > 50)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Password minimum length must be of 8 characters";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                user.HashedPassword = customerPassword;

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
                string affiliateId = Request.Form["txtAffiliate"];
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
                        customer.CustomerRegistrationMode = (int)RegistrationMode.Online;

                        if (!String.IsNullOrEmpty(affiliateId))
                        {
                            customer.AffiliateId = Convert.ToInt32(affiliateId);
                        }

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
                            string domain = Request.Url.Scheme + "://" + Request.Url.Authority;
                            int res = customerService.AddCustomer(customer, user, domain: domain);
                            if (res > 0)
                            {                               
                                statusArray[0] = "200";
                                statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Thank you for registration.<br /> An email has been sent to you on your email account,<br /> please follow the instruction to activate your account. <br/> <a id='lkGoBack' style='font-size: 14px; ' href='/Account/Login'>Login</a></span>";//"" + res;
                                statusArray[2] = "" + res;

                                return Json(statusArray, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                statusArray[0] = "101";
                                statusArray[1] = "Unable to process request.";
                            }
                        }
                        catch (UserException exp)
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Failed: " + exp.Message;
                        }
                        catch (Exception exp)
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                        }

                        #endregion
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Missing required information.";
                    }

                }
            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Invalid Request.";
            }

            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Forgot Password
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult ForgotPassword(FormCollection formCollection)
        {
            var statusArray = new string[3];
            string hfFormType = Request.Form["hfCallFromId"];
            if (hfFormType == "Email")
            {
                try
                {
                    string email = Request.Form["txtEmailAddress"];
                    if (!string.IsNullOrEmpty(email) && IsEmailValid(email))
                    {
                        var customerService = new CustomerService();
                        if (customerService.IsCustomerExist(email))
                        {
                            //if (customerService.IsCustomerChangePasswordRequired(email))
                            //{
                            // Now verification code to customer via email
                            var res = customerService.SendCustomerForgotPasswordCode(email);
                            if (res)
                            {
                                statusArray[0] = "200";
                                statusArray[2] = "Next Page";
                            }
                            //}
                            //else
                            //{
                            //    statusArray[0] = "101";
                            //    statusArray[1] = "Email has already been sent to your email address";
                            //    return Json(statusArray, JsonRequestBehavior.AllowGet);
                            //}
                        }
                        else
                        {
                            statusArray[0] = "101";
                            statusArray[1] = "Email address not found!";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Invalid email address";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "" + exp.Message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            else if (hfFormType == "Reset")
            {
                try
                {

                    if (Session["attempt"] != null)
                    {
                        int attempt = Convert.ToInt32(Session["attempt"]);

                        if (attempt >= 2)
                        {
                            Session["attempt"] = 0;
                            statusArray[0] = "102";
                            statusArray[1] = "Invalid attempt to verification code.";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                    }
                    string verifiationCode = Request.Form["txtVerificationCode"];
                    var emailAddress = Request.Form["hfEmailId"];

                    string newPassword = Request.Form["password"];
                    string cfmPassword = Request.Form["cfmPassword"];
                    if (string.IsNullOrEmpty(verifiationCode))
                    {

                        statusArray[0] = "101";
                        statusArray[1] = "Invalid Verification Code";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    if (string.IsNullOrEmpty(newPassword))
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Invalid New Customer";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    if (string.IsNullOrEmpty(cfmPassword))
                    {
                        //retrun error
                        statusArray[0] = "101";
                        statusArray[1] = "Invalid Confirm Password";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    if (newPassword != cfmPassword)
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "New Password mismatched with confirm password.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    if (newPassword.Length < 8)
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Password length should be minimum of 8 characters";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    if (cfmPassword.Length < 8)
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Password length should be minimum of 8 characters";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }

                    // verification with emailId
                    // get expireon of verification code
                    if (!string.IsNullOrEmpty(emailAddress) && IsEmailValid(emailAddress))
                    {
                        var cust = new CustomerService().GetCustomerByEmail(email: emailAddress);
                        if (cust != null && cust.VerificationCode == verifiationCode &&
                            cust.VerificationCodeExpireOn != null)
                        {
                            int result = DateTime.Compare(Convert.ToDateTime(cust.VerificationCodeExpireOn), DateTime.Now);
                            if (result > 0)
                            {
                                var isUpdated = new CustomerService()
                                    .ResetCustomerForgotPassword(emailAddress, verifiationCode, newPassword);
                                if (isUpdated)
                                {
                                    statusArray[0] = "200";
                                    statusArray[2] = "Next Page";
                                }
                            }
                            else
                            {
                                statusArray[0] = "101";
                                statusArray[1] = "Invalid email address";
                                return Json(statusArray, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            //no record found
                            int time = 1;
                            Session["attempt"] = Convert.ToInt32(Session["attempt"]) + time;
                            statusArray[0] = "101";
                            statusArray[1] = "Invalid verification code.";
                        }
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Invalid email address";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "" + exp.Message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Invalid request";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);

        }

        [RoleActionValidator]
        public JsonResult ActivateCustomer(string customerId)
        {
            var statusArray = new string[3];
            bool isActive = false;
            if (!string.IsNullOrEmpty(customerId))
            {
                try
                {
                    isActive = new CustomerService()
                          .ActivateCustomer(Convert.ToInt16(new Utility().UrlSafeDecrypt(customerId)));
                    if (isActive)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />The Customer has been activated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CardRequests'>Go back</a></span>";
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to update data please try again";
                    }
                }

                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                }
            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Unable to update data please try again";
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }
        public bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        #endregion
    }


}

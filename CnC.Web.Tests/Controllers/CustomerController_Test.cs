using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Customers;
using CnC.Service;
using System;
using System.Web.Mvc;
using CnC.Core.Exceptions;
using CnC.Service.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CnC.Web.Helper;
using System.Collections.Generic;
using CnC.Core.Cards;
using CnC.Core.Payments;
using System.IO;

namespace CnC.Web.Tests.Controllers
{
    [TestClass]
    public class CustomerController_Test : Controller
    {
        [TestMethod]
        public void NewCustomer()
        {
            string email = "haider.mustafa581@gmail.com";
            var user = new User();
            user.Email = email;
            var editId = "23";
            user.Username = user.Email;
            user.Status = (int)UserStatus.Active;
            var settingService = new SettingService();
            //var documentService = new DocumentService();
            int defaultCurrencyId = settingService.CustomerDefaultCurrency.Id;

            string engFirstName = "Haider";
            string engLastName = "Mustafa";
            string engDateOfBirth = "22/06/1993";
            string engAddress1 = "Test Address1";
            string engAddress2 = "Test Address2";

            string perFirstName = "persian first name";
            string perLastName = "persian Last name";
            string perDateOfBirth = "22/06/1993";
            string perAddress1 = "persian address 1";
            string perAddress2 = "persian address 2";

            string cityId = "1";
            string contactNo = "03207818199";
            string countryId = "1";
            string gender = "Male";
            string maritalStatus = "Single";
            string nationality = "Iran";
            string nic = "1234567890";
            string passportNo = "A34567892";
            string postalCode = "32999";
            var utility = new Utility();

            user.FirstName = engFirstName;
            user.LastName = engLastName;
            user.FirstNameInCustomerLanguage = perFirstName;
            user.LastNameInCustomerLanguage = perLastName;

            var customer = new Customer();
            customer.DateOfBirth = DateTime.Now.AddYears(-19);
            customer.Address = engAddress1;
            customer.Address2 = engAddress2;

            customer.DateOfBirthInCustomerLanguage = perDateOfBirth;
            customer.AddressInCustomerLanguage = perAddress1;
            customer.Address2InCustomerLanguage = perAddress2;

            customer.CityId = int.Parse(cityId);
            customer.ContactNo = contactNo;
            customer.CountryId = int.Parse(countryId);
            customer.GenderId = int.Parse("0");
            customer.LanguageId = settingService.CustomerDefaultLanguage.Id;
            customer.MaritalStatusId = int.Parse("0");
            customer.Nationality = nationality;
            customer.NIC = nic;
            customer.PassportNo = passportNo;
            customer.PostalCode = postalCode;
            customer.CurrencyId = defaultCurrencyId;

            string newFileName = "abc.jpg";
            customer.PassportDocInCustomerLanguage = newFileName;

            string newFileNameNic = "xyz.jpg";
            customer.NICDocInCustomerLanguage = newFileNameNic;

            var customerService = new CustomerServiceTest();

            int res = customerService.AddCustomer_Test(customer, user);
            if (res <= 0)
            {
                Assert.Fail();
            }

            //return View();
        }

        [TestMethod]
        public JsonResult NewCustomerCardRequests()
        {
            string proofOfAddressType = "1";
            string sourcceOfFundType = "1";
            string hfsubmitActionName = "Bank";
            string hfCustomerId = "2000";
            var statusArray = new string[5];
            var cardName = "Black,Platinum,Gold";
            string cardTypeIds = "1,2,3";
            string cardQuantity = "1,1,1";
            var userSession = Session["CurrentSession"] as User;
            string sourceOfFundTypeName = "Employement";
            string proofOfAddressTypeName = "Property Ownership or Lease Legal Document";
            if (!string.IsNullOrEmpty(cardTypeIds) && !string.IsNullOrEmpty(cardQuantity))
            {
                var settingService = new SettingService();
                var documentService = new DocumentService();
                int defaultCurrencyId = settingService.CustomerDefaultCurrency.Id;
                //var proofOfSourceOfFundsImagesCl = proofOfSourceOfFundsCl as IList<HttpPostedFileBase> ?? proofOfSourceOfFundsCl.ToList();
                //var proofOfAddressImagesCl = proofOfAddressDocCl as IList<HttpPostedFileBase> ?? proofOfAddressDocCl.ToList();
                //proofOfAddressImagesCl[0].ContentLength = 381649;
                //proofOfAddressImagesCl[0].ContentType = "image/jpeg";
                //proofOfAddressImagesCl[0].FileName = "Coustomer Top UP Service charges.jpg";
                //proofOfAddressImagesCl[0].ContentLength = 381649;
                #region Step2
                var listOfSourceOfFundsDocs = new List<string>();
                var listOfProofOfAddrss = new List<string>();
                string proofOfAddresDoc = string.Empty;
                var typesOfCards = new CardService().GetCardTypes();
                var arrCardTypeIds = cardTypeIds.Split(',');

                if (!string.IsNullOrEmpty(hfCustomerId) && !string.IsNullOrEmpty(proofOfAddressType) && !string.IsNullOrEmpty(sourcceOfFundType))
                {
                    hfCustomerId = new Utility().UrlSafeDecrypt(hfCustomerId);
                    if (string.IsNullOrEmpty(hfCustomerId))
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Invalid value please try again.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }

                    var cardRequests = new List<CardRequest>();
                    var arrCardQty = new string[] { "1", "1", "1" };
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
                        if (userSession != null && userSession.IsCustomer)
                            cr.ServiceExchangeFee = new SettingService().ExchangeRateServiceCharges;

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

                    //if (isProofOfAddressRequired && !proofOfAddressImagesCl.Any())
                    //{
                    //    // Error
                    //    statusArray[0] = "101";
                    //    statusArray[1] = "Missing Required ProofOfAddress Documents.";
                    //    return Json(statusArray, JsonRequestBehavior.AllowGet);
                    //}

                    //if (isProofOfSourceOfFundsRequired && !proofOfSourceOfFundsImagesCl.Any())
                    //{
                    //    // Error
                    //    statusArray[0] = "101";
                    //    statusArray[1] = "Missing required documents ProofOfSourceOfFunds";
                    //    return Json(statusArray, JsonRequestBehavior.AllowGet);
                    //}
                    var utility = new Utility();
                    if (isProofOfAddressRequired)
                    {
                        int flagAddressCounter = 0;
                        //foreach (var proofOfAddressImgCustomer in proofOfAddressImagesCl)
                        //{
                        //    if (proofOfAddressImgCustomer != null && utility.IsValidImage(proofOfAddressImgCustomer))
                        //    {
                        string extensions = "jpg";
                        string newFileNames = customer.NIC + "-AddressCL" + flagAddressCounter + extensions;
                        var paths = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileNames);
                        //proofOfAddressImgCustomer.SaveAs(path);
                        listOfProofOfAddrss.Add(newFileNames);
                        flagAddressCounter++;
                        //        //proofOfAddresDoc = newFileName;
                        //        //customer.ProofOfSourceOfFundsDocInCustomerLanguage = customer.ProofOfSourceOfFundsDocInCustomerLanguage + newFileName + ";";

                        //    }
                        //    //else
                        //    //{
                        //    //    statusArray[0] = "101";
                        //    //    statusArray[1] = "Invalid Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB."; ;
                        //    //    return Json(statusArray, JsonRequestBehavior.AllowGet);
                        //    //}
                        //}

                    }

                    //if (isProofOfSourceOfFundsRequired)
                    //{
                    int flagCounter = 0;
                    //    foreach (var proofOfSourceOfFundsImageCustomer in proofOfSourceOfFundsImagesCl)
                    //    {
                    //        if (proofOfSourceOfFundsImageCustomer != null && utility.IsValidImage(proofOfSourceOfFundsImageCustomer))
                    //        {
                    string extension = "jpg";
                    string newFileName = customer.NIC + "-FundsCL" + flagCounter + extension;
                    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                    //proofOfSourceOfFundsImageCustomer.SaveAs(path);
                    listOfSourceOfFundsDocs.Add(newFileName);
                    flagCounter++;
                    //        }
                    //        //else
                    //        //{
                    //        //    statusArray[0] = "101";
                    //        //    statusArray[1] = "Invalid Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB."; ;
                    //        //    return Json(statusArray, JsonRequestBehavior.AllowGet);
                    //        //}
                    //        // customer.ProofOfSourceOfFundsDocInCustomerLanguage = customer.ProofOfSourceOfFundsDocInCustomerLanguage + newFileName + ";";
                    //        // flagCounter++;
                    //    }
                    //}

                    // Payment info

                    var payments = new List<Payment>();
                    var payment = new Payment();
                    if (userSession != null && userSession.IsCustomer)
                    {
                        //string cardTransactionNo = Request.Form["txtCardTransactionNo"];
                        //string cardAccountNo = Request.Form["txtCardAccountNo"];
                        //string cardReqCustName = Request.Form["txtCardReqCustName"];
                        //string cardPaidAmount = Request.Form["txtCardPaidAmount"];
                        //if (hfsubmitActionName != "Online" && !string.IsNullOrEmpty(cardTransactionNo) && !string.IsNullOrEmpty(cardAccountNo) && !string.IsNullOrEmpty(cardReqCustName) && !string.IsNullOrEmpty(cardPaidAmount))
                        //{
                        //    var exchangeRate = new ExchangeRateService().GetExchangeRate(settingService.CustomerDefaultCurrency.Id).Rate;

                        //    var tempActualAmt = totalAmount * exchangeRate;
                        //    tempActualAmt = tempActualAmt + ((tempActualAmt / 100) * settingService.ExchangeRateServiceCharges); // Applying exchange rate services charges
                        //    var tempToVerifyAmt = Convert.ToInt32(cardPaidAmount);

                        //    if (tempToVerifyAmt < tempActualAmt)
                        //    {
                        //        statusArray[0] = "101";
                        //        statusArray[1] = "Invalid amount entered in Amount Textbox.";
                        //        return Json(statusArray, JsonRequestBehavior.AllowGet);
                        //    }
                        //    payment.Amount = Convert.ToInt32(cardPaidAmount);
                        //    payment.CreatedOn = DateTime.UtcNow;
                        //    payment.TransactionAccountNo = cardAccountNo;
                        //    payment.TransactionName = cardReqCustName;
                        //    payment.TransactionNo = cardTransactionNo;
                        //    payment.PaymentMethod = PaymentMethod.Bank;
                        //    payment.PaymentMethodId = (int)PaymentMethod.Bank;



                        //}
                        //else if (hfsubmitActionName == "Online")
                        //{
                        //    var exchangeRate = new ExchangeRateService().GetExchangeRate(settingService.CustomerDefaultCurrency.Id).Rate;
                        //    //var tempActualAmt = totalAmount * exchangeRate;
                        //    //tempActualAmt = tempActualAmt + ((tempActualAmt / 100) * settingService.ExchangeRateServiceCharges); // Applying exchange rate services charges

                        //    payment.Amount = totalAmount * exchangeRate;//tempActualAmt;
                        //    payment.CreatedOn = DateTime.UtcNow;
                        //    //payment.TransactionAccountNo = cardAccountNo;
                        //    //payment.TransactionName = cardReqCustName;
                        //    //payment.TransactionNo = cardTransactionNo;
                        //    payment.PaymentMethod = PaymentMethod.Online;
                        //    payment.PaymentMethodId = (int)PaymentMethod.Online;

                        //}
                        //else
                        //{
                        //    statusArray[0] = "101";
                        //    statusArray[1] = "Invalid Transaction information.";
                        //    return Json(statusArray, JsonRequestBehavior.AllowGet);
                        //}

                    }
                    else
                    {
                        payment.Amount = 192;
                        payment.CreatedOn = DateTime.UtcNow;
                        payment.PaymentMethod = PaymentMethod.RAHYAB;
                        payment.PaymentMethodId = (int)PaymentMethod.RAHYAB;
                        payment.ConfirmedOn = DateTime.Now;
                    }

                    payments.Add(payment);

                    if (cardPurchased)
                    {
                        try
                        {
                            int res = 0;
                            if (userSession != null && userSession.IsCustomer)
                            {
                                //    if (hfsubmitActionName == "Online" && payments.Count > 0)
                                //    {
                                //        var tempActualAmt = payments[0].Amount; //* exchangeRate;
                                //        tempActualAmt = tempActualAmt + ((tempActualAmt / 100) * settingService.ExchangeRateServiceCharges);

                                //        var totalAmtIncludingAllCharges = Convert.ToInt32(tempActualAmt);//(payments[0].Amount);
                                //        var mabnaService = new MabnaPaymentGateway().Payment(totalAmtIncludingAllCharges, new RequestForm().ApplicationNumber, 0, "/Customer/MabnaResponseForCardRequestsPayment");
                                //        //var mabnaService = new MabnaPaymentGateway().Payment(1000, new RequestForm().ApplicationNumber, 0, "/Customer/MabnaResponseForCardRequestsPayment");
                                //        if (mabnaService[0] == "0" && !string.IsNullOrEmpty(mabnaService[1]))
                                //        {
                                //            var received_token_value = mabnaService[1];
                                //            var tempContainer = new CustomerNewRequestContainer();
                                //            tempContainer.RequestFormObj = requestForm;
                                //            tempContainer.CardRequestCollection = cardRequests;
                                //            tempContainer.Payments = payments;
                                //            tempContainer.ProofOfAddressType = proofOfAddressType;
                                //            tempContainer.ListOfProofOfAddrss = listOfProofOfAddrss;
                                //            tempContainer.SourcceOfFundType = sourcceOfFundType;
                                //            tempContainer.ListOfSourceOfFundsDocs = listOfSourceOfFundsDocs;
                                //            tempContainer.CardQty = cardQuantity;
                                //            tempContainer.CardTypeIds = cardTypeIds;
                                //            Session["TempNewCardRequest"] = tempContainer;



                                //            statusArray[0] = "205"; // in case of url redidrect set status 205
                                //            statusArray[1] = utility.HtmlFormSubmissionStructure(received_token_value);
                                //            return Json(statusArray, JsonRequestBehavior.AllowGet);
                                //        }
                                //        else
                                //        {
                                //            statusArray[0] = "206";
                                //            statusArray[1] = mabnaService[0] + ":" + mabnaService[1];
                                //            return Json(statusArray, JsonRequestBehavior.AllowGet);
                                //        }

                                //    }
                                //    else
                                //    {

                                //        res = new CardService().AddCardRequestForm(requestForm, cardRequests, payments, true,
                                //             proofOfAddressType, listOfProofOfAddrss, sourcceOfFundType, listOfSourceOfFundsDocs);
                                //    }

                            }
                            else
                            {
                                //res = new CardServiceTest().AddCardRequestForm_Test(requestForm, cardRequests, payments, true,
                                //                                    proofOfAddressType, listOfProofOfAddrss, sourcceOfFundType, listOfSourceOfFundsDocs);
                            }
                            if (res > 0)
                            {
                                if (userSession != null && userSession.IsCustomer)
                                {
                                    //var content = new Utility().DownloadCustomerFilledForm(customer, userSession, cardTypeIds, cardQuantity, cardName, sourceOfFundTypeName, proofOfAddressTypeName, requestForm.ApplicationNumber);
                                    //statusArray[3] = content[0];
                                    //statusArray[4] = content[1];

                                    //statusArray[0] = "200";
                                    //statusArray[1] = "" + res;//"<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Card request submitted successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/MyProfile'>Go back</a></span>";
                                    //statusArray[2] = "Step5"; //Changing this to step3 from Step4
                                    //return Json(statusArray, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    statusArray[0] = "200";
                                    statusArray[1] = new Utility().UrlSafeEncrypt(hfCustomerId);//res; Send customer id back to client for next step
                                    statusArray[2] = "Step3";
                                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                                }

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

        //public JsonResult NewCustomerSignedForm()
        //{
        //    var statusArray = new string[5];
        //    var customerSession = Session["CurrentCustomer"] as Customer;
        //    LocalizationService localizationService = null;
        //    string hfCustomerId = null;
        //    if (customerSession == null)
        //    {
        //        localizationService = new LocalizationService();
        //    }
        //    else
        //    {
        //        localizationService = new LocalizationService(customerSession.LanguageId);
        //    }
        //    //if ((string.IsNullOrEmpty(Request.Form["hfIsCallForDownload"]) || Request.Form["hfIsCallForDownload"] == "false") && !string.IsNullOrEmpty(hfCustomerId) && customerSignedCopyEng != null && customerSignedCopyCl != null)
        //    //{

        //    //hfCustomerId = new Utility().UrlSafeDecrypt(hfCustomerId);
        //    hfCustomerId = "2000";
        //    if (string.IsNullOrEmpty(hfCustomerId))
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Invalid value please try again.";
        //        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    }
        //    var documentService = new DocumentService();
        //    var customerService_Test = new CustomerServiceTest();
        //    var utility = new Utility();
        //    var customer = customerService_Test.GetCustomer_Test(Convert.ToInt32(hfCustomerId));
        //    string customerSignedCopyPath = string.Empty;
        //    string customerSignedCopyClPath = string.Empty;
        //    //if (customerSignedCopyEng != null && !utility.IsValidImage(customerSignedCopyEng))
        //    //{
        //    //    statusArray[0] = "101";
        //    //    statusArray[1] = "Invalid Customer Signed English Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
        //    //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    //}
        //    //else if (customerSignedCopyCl != null && !utility.IsValidImage(customerSignedCopyCl))
        //    //{
        //    //    statusArray[0] = "101";
        //    //    statusArray[1] =
        //    //        "Invalid Customer Signed Persian Document, please make sure its format should be .Jpeg or .jpg or .png and size must be less than 1MB.";
        //    //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    //}
        //    //if (customerSignedCopyEng != null && customerSignedCopyEng.ContentLength > 0)
        //    //{
        //    string extensions = "jpg";
        //    string newFileNames = customer.NIC + "-KYCCustomerSigned" + extensions;
        //    var paths = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileNames);
        //    //customerSignedCopyEng.SaveAs(paths);
        //    customerSignedCopyPath = newFileNames;
        //    //customer.NICDocInCustomerLanguage = newFileName;
        //    //}
        //    //else
        //    //{
        //    //    statusArray[0] = "101";
        //    //    statusArray[1] = "Invalid File. Please try again.";
        //    //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    //}
        //    //if (customerSignedCopyCl != null && customerSignedCopyCl.ContentLength > 0)
        //    //{
        //    string extension = "jpg";
        //    string newFileName = customer.NIC + "-KYCCustomerSignedCL" + extension;
        //    var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
        //    //customerSignedCopyCl.SaveAs(path);
        //    customerSignedCopyClPath = newFileName;
        //    //customer.NICDocInCustomerLanguage = newFileName;
        //    //}
        //    //else
        //    //{
        //    //    statusArray[0] = "101";
        //    //    statusArray[1] = "Invalid File. Please try again.";
        //    //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    //}
        //    try
        //    {
        //        var res = customerService_Test.UploadCustomerSignedForm_Test(Convert.ToInt32(hfCustomerId), customerSignedCopyPath, customerSignedCopyClPath);
        //        if (res)
        //        {
        //            statusArray[0] = "200";
        //            statusArray[2] = "";
        //            var userSession = Session["CurrentSession"] as User;
        //            statusArray[1] = userSession != null && userSession.IsCustomer // In case of customer new card requests
        //                ? "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i>" + localizationService.GetResource("Cnc.Complete", null, "Complete") + "</strong></span><br />" + localizationService.GetResource("Cnc.CustomerSignedDocsSavedSuccessfully", null, "Customer signed documents has been saved successfully!") + "<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/MyProfile'>" + localizationService.GetResource("Cnc.GoBack", null, "Go Back") + "</a></span>"
        //                : "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> " + localizationService.GetResource("Cnc.Complete", null, "Complete") + "</strong></span><br />" + localizationService.GetResource("Cnc.KycFormSavedSuccessfully", null, "KYC form has been saved successfully!") + "<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/NewCustomer'>" + localizationService.GetResource("Cnc.GoBack", null, "Go Back") + "</a></span>";
        //            return Json(statusArray, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            statusArray[0] = "101";
        //            statusArray[1] = "Unable to process request.";
        //        }
        //    }
        //    catch (UserException exp)
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Failed: " + exp.Message;
        //    }
        //    catch (Exception exp)
        //    {
        //        statusArray[0] = "101";
        //        statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
        //    }
        //    //}
        //    //else if (!string.IsNullOrEmpty(hfCustomerId) && !string.IsNullOrEmpty(Request.Form["hfIsCallForDownload"]) && Request.Form["hfIsCallForDownload"] == "true")
        //    //{
        //    //    //Download
        //    //    var userSession = Session["CurrentSession"] as User;
        //    //    if (userSession != null && userSession.IsCustomer)
        //    //    {
        //    //        var customer = new CustomerService().GetCustomer(userSession.Id, false);
        //    //        var content = new Utility().DownloadCustomerFilledForm(customer, userSession, "", "", "", "", "", "");
        //    //        statusArray[3] = content[0];
        //    //        statusArray[4] = content[1];

        //    //        statusArray[0] = "200";
        //    //        statusArray[1] = "" + customer.UserId;//"<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Card request submitted successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Customer/MyProfile'>Go back</a></span>";
        //    //        statusArray[2] = "Step5"; //Changing this to step3 from Step4
        //    //        return Json(statusArray, JsonRequestBehavior.AllowGet);
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    statusArray[0] = "101";
        //    //    statusArray[1] = "Missing required info.";
        //    //}

        //    return Json(statusArray, JsonRequestBehavior.AllowGet);
        //}



        public ActionResult EditCustomer()
        {
            string email = "haider.mustafa581@gmail.com";
            var user = new User();
            user.Email = email;
            var editId = "23";
            user.Username = user.Email;
            user.Status = (int)UserStatus.Active;

            var settingService = new SettingService();
            var documentService = new DocumentService();
            int defaultCurrencyId = settingService.CustomerDefaultCurrency.Id;

            string engFirstName = "Haider";
            string engLastName = "Mustafa";
            string engDateOfBirth = "22/06/1993";
            string engAddress1 = "Test Address1";
            string engAddress2 = "Test Address2";

            string perFirstName = "persian first name";
            string perLastName = "persian Last name";
            string perDateOfBirth = "22/06/1993";
            string perAddress1 = "persian address 1";
            string perAddress2 = "persian address 2";

            string cityId = "1";
            string contactNo = "03207818199";
            string countryId = "1";
            string gender = "Male";
            string maritalStatus = "Single";
            string nationality = "Iran";
            string nic = "1234567890";
            string passportNo = "A34567892";
            string postalCode = "32999";
            var utility = new Utility();

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

            string newFileName = "abc.jpg";
            customer.PassportDocInCustomerLanguage = newFileName;

            string newFileNameNic = "xyz.jpg";
            customer.NICDocInCustomerLanguage = newFileNameNic;

            string newFileNameSignedForm = "form.jpg";
            customer.CustomerSignedForm = newFileNameSignedForm;

            string newFileSignFormInCustLanguage = "signedForm.jpg";
            customer.CustomerSignedFormInCustomerLanguage = newFileSignFormInCustLanguage;

            var customerService_Test = new CustomerServiceTest();
            try
            {
                //check here whether it is from update or add

                editId = utility.UrlSafeDecrypt(editId);
                if (!string.IsNullOrEmpty(editId) && !string.IsNullOrEmpty(editId))
                {
                    //update 
                    user.Id = Convert.ToInt32(editId);
                    int updateRes = customerService_Test.UpdateCustomer_Test(customer, user);
                    //if (updateRes > 0)
                    //{
                    //    statusArray[0] = "200";
                    //    statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Information updated successfully!<br/> <a id='lkGoBack' style='font-size: 14px; ' href='/Customer/CustomerValidation'>Go Back</a></span>";//"" + res;
                    //    statusArray[2] = "102";//Update
                    //}
                }

                else
                {
                    //statusArray[0] = "101";
                    //statusArray[1] = "Unable to process request.";
                }

            }
            catch (UserException exp)
            {
                //statusArray[0] = "101";
                //statusArray[1] = "Failed: " + exp.Message;
            }
            return View();
        }
    }
}

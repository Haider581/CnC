using CnC.Core.Accounts;
using CnC.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CnC.Core.Customers;
using CnC.Service;
using Image = System.Drawing.Image;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using CnC.Core.Cards;
using log4net;

namespace CnC.Web.Helper
{
    public class Utility
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(Utility));
        public string GetCountries()
        {
            //   var sessionKey = HttpContext.Current.Session["CurrentSession"] as User;
            var commonService = new CnC.Service.CommonService();
            var countries = commonService.GetCountries();
            return Newtonsoft.Json.JsonConvert.SerializeObject(countries);
        }

        public static bool HasPermission(string permissionName)
        {
            //CurrentSession.Instance.LoginUser.r
            return false;
        }
        public static bool IsAlphaNumeric(string strToCheck)
        {
            var rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        public static bool IsAlphabet(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, @"^[a-zA-Z]+$");
        }

        public static bool IsAlphabetWithSpace(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, @"^[a-zA-Z ]+$");
        }
        public static bool IsNumeric(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, @"^[0-9]+$");
        }
        public bool IsValidImage(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null) { return false; }
            // var allowedSize=AppSer
            string imageSize = ConfigurationManager.AppSettings["ImageSize"];

            if (file.ContentLength > Convert.ToInt32(imageSize) * 1024 * 1024)
            { return false; }

            try
            {
                var allowedFormats = new[]
                {
                ImageFormat.Jpeg,
                ImageFormat.Png
            };

                using (var img = Image.FromStream(file.InputStream))
                {
                    return allowedFormats.Contains(img.RawFormat);
                }
            }
            catch { }
            return false;
        }

        public bool IsValidImageClientSideUploading(object value)
        {
            var file = value as System.Web.HttpPostedFile;
            if (file == null) { return false; }
            // var allowedSize=AppSer
            string imageSize = ConfigurationManager.AppSettings["ImageSize"];

            if (file.ContentLength > Convert.ToInt32(imageSize) * 1024 * 1024)
            { return false; }

            try
            {
                var allowedFormats = new[]
                {
                ImageFormat.Jpeg,
                ImageFormat.Png
            };

                using (var img = Image.FromStream(file.InputStream))
                {
                    return allowedFormats.Contains(img.RawFormat);
                }
            }
            catch { }
            return false;
        }

        public string[] DownloadCustomerFilledForm(Customer customer, User user, CardRequestForm cardRequestForm)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer cannot be null");

            if (user == null)
                throw new ArgumentNullException("User cannot be null");

            if (cardRequestForm == null)
                throw new ArgumentNullException("Card Request Form cannot be null");

            if (cardRequestForm.CardRequests == null || cardRequestForm.CardRequests.Count == 0)
                throw new ArgumentNullException("Card Requests cannot be null");

            var cardRequestsGroupBy = cardRequestForm.CardRequests.GroupBy(cr => cr.CardTypeId);

            string cardTypeIds = "";
            string cardsQuantity = "";

            foreach (var cardRequestGroupBy in cardRequestsGroupBy)
            {
                cardTypeIds = cardTypeIds + cardRequestGroupBy.First().CardTypeId + ",";
                cardsQuantity = cardsQuantity + cardRequestGroupBy.Count() + ",";
            }

            return DownloadCustomerFilledForm(customer, user, cardTypeIds.Trim(','), cardsQuantity.Trim(','),
                cardRequestForm.ApplicationNumber);
        }

        public string[] DownloadCustomerFilledForm(Customer customer, User user, string selectedCardTypeIds
            , string selectedCardsQuantity, string applicationNumber)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer cannot be null");

            if (user == null)
                throw new ArgumentNullException("User cannot be null");

            selectedCardTypeIds = selectedCardTypeIds.Trim();

            if (string.IsNullOrEmpty(selectedCardTypeIds))
                throw new ArgumentNullException("Card Type Ids cannot be null");

            selectedCardsQuantity = selectedCardsQuantity.Trim();

            if (string.IsNullOrEmpty(selectedCardsQuantity))
                throw new ArgumentNullException("Card Quantity cannot be null");

            var filledCustomerTemplate = new string[2];

            string englishContent = File.ReadAllText(
                System.Web.Hosting.HostingEnvironment.MapPath("~/Content/KYCFormEnglish.html")
                , System.Text.Encoding.UTF8);

            englishContent = englishContent.Replace("{[FirstName]}", user.FirstName);
            englishContent = englishContent.Replace("{[LastName]}", user.LastName);
            englishContent = englishContent.Replace("{[ApplicationNo]}", applicationNumber);

            var city = new CommonService().GetCity(customer.CityId).Name;

            MaritalStatus maritalStatus = (MaritalStatus)customer.MaritalStatusId;
            englishContent = englishContent.Replace(maritalStatus == MaritalStatus.Married ? "{[MaritalMarriedCheckbox]}" : "{[MaritalSingleCheckbox]}", "checked");

            Gender gender = (Gender)customer.GenderId;
            englishContent = englishContent.Replace(gender == Gender.Male ? "{[MaleCheckbox]}" : "{[FemaleCheckbox]}", "checked");


            englishContent = englishContent.Replace("{[DateOfBirth]}", "" + customer.DateOfBirth.ToString("yyyy/MM/dd"));//.ToString("dd/mm/yyyy"));

            englishContent = englishContent.Replace("{[IranianNationCheckbox]}", "checked"); //customer.Nationality

            englishContent = englishContent.Replace("{[Nationality]}", customer.Nationality);
            englishContent = englishContent.Replace("{[Address]}", customer.Address);
            englishContent = englishContent.Replace("{[City]}", "" + new CommonService().GetCity(customer.CityId).Name);
            englishContent = englishContent.Replace("{[FullName]}", user.FirstName + "&nbsp" + user.LastName);
            englishContent = englishContent.Replace("{[NationalIdentificationNumber]}", customer.NIC);
            englishContent = englishContent.Replace("{[PostalCode]}", customer.PostalCode);
            englishContent = englishContent.Replace("{[E-mailAddress]}", user.Email);
            englishContent = englishContent.Replace("{[MobileNumber]}", customer.ContactNo);
            englishContent = englishContent.Replace("{[Date]}", DateTime.Now.ToString("yyyy/MM/dd"));//.ToString("dd/mm/yyyy"));
            englishContent = englishContent.Replace("{[PassportNumber]}", customer.PassportNo);

            //English
            string tableHeaderEnglish = "<th>Narration</th>";
            string tableFirstRowEnglish = "<td>Quantity</td>";
            string tableSecondRowEnglish = "<td>Quality</td>";
            string tableThirdRowEnglish = "<td>Card Creation Fee</td>";
            string tableFourthRowEnglish = "<td>Documents Required " + "<br />" + " (please see the guidelines)</td>";
            string kycFormEnglishCardsTableHtml;

            //Persian
            string persianContent = File.ReadAllText(
                System.Web.Hosting.HostingEnvironment.MapPath("~/Content/KYCFormPersian.html")
                , System.Text.Encoding.UTF8);

            string tableHeaderPersian = "";
            string tableFirstRowPersian = "";
            string tableSecondRowPersian = "";
            string tableThirdRowPersian = "";
            string tableFourthRowPersian = "";
            string kycFormPersianCardsTableHtml;

            var typesOfCards = new CardService().GetCardTypes(customer: customer);

            var selectedCardTypeIdsList = selectedCardTypeIds.Split(',').ToList();
            var arrSelectedCardsQty = selectedCardsQuantity.Split(',');

            //for (var j = typesOfCards.Count; j > 0; j--)
            //{ tableHeaderPersian = tableHeaderPersian + "<th>" + typesOfCards[j-1].Name + "</th>"; }
            foreach (var cardType in typesOfCards)
            {
                var indexOfSelectCardTypeId = selectedCardTypeIdsList.IndexOf(cardType.Id.ToString());

                string cardQty = "0";

                if (indexOfSelectCardTypeId >= 0)
                    cardQty = arrSelectedCardsQty[indexOfSelectCardTypeId];

                //if (Convert.ToInt32(cardQty) < 0) continue;

                tableHeaderEnglish = tableHeaderEnglish + "<th>" + cardType.Name + "</th>";
                tableHeaderPersian = tableHeaderPersian + "<th>" + cardType.Name + "</th>";

                if (cardType.IsProofOfAddressRequired)
                {
                    if (customer.ProofOfAddressDocType == "1")
                    {
                        englishContent = englishContent.Replace("{[PropertyOwnershiporLeaseLegalDocument]}", "checked");
                        persianContent = persianContent.Replace("{[PropertyOwnershiporLeaseLegalDocument]}", "checked");

                    }
                    else if (customer.ProofOfAddressDocType == "2")
                    {
                        englishContent = englishContent.Replace("{[Latestelectricitybill]}", "checked");
                        persianContent = persianContent.Replace("{[Latestelectricitybill]}", "checked");
                    }
                    else if (customer.ProofOfAddressDocType == "3")
                    {
                        englishContent = englishContent.Replace("{[LatestGasbill]}", "checked");
                        persianContent = persianContent.Replace("{[LatestGasbill]}", "checked");
                    }
                }
                if (cardType.IsProofOfSourceOfFundsRequired)
                {
                    if (customer.ProofOfSourceOfFundsDocType == "1")
                    {
                        englishContent = englishContent.Replace("{[Employment]}", "checked");
                        persianContent = persianContent.Replace("{[Employment]}", "checked");
                    }
                    else if (customer.ProofOfSourceOfFundsDocType == "2")
                    {
                        englishContent = englishContent.Replace("{[Pension]}", "checked");
                        persianContent = persianContent.Replace("{[Pension]}", "checked");
                    }
                    else if (customer.ProofOfSourceOfFundsDocType == "3")
                    {
                        englishContent = englishContent.Replace("{[PaymentofDividendsasUBO]}", "checked");
                        persianContent = persianContent.Replace("{[PaymentofDividendsasUBO]}", "checked");
                    }
                    else if (customer.ProofOfSourceOfFundsDocType == "4")
                    {
                        englishContent = englishContent.Replace("{[LoanAgreement]}", "checked");
                        persianContent = persianContent.Replace("{[LoanAgreement]}", "checked");
                    }
                    else if (customer.ProofOfSourceOfFundsDocType == "5")
                    {
                        englishContent = englishContent.Replace("{[SaleContract]}", "checked");
                        persianContent = persianContent.Replace("{[SaleContract]}", "checked");
                    }
                    else if (customer.ProofOfSourceOfFundsDocType == "6")
                    {
                        englishContent = englishContent.Replace("{[CommissionbasedAssociateAgreement]}", "checked");
                        persianContent = persianContent.Replace("{[CommissionbasedAssociateAgreement]}", "checked");
                    }
                    else if (customer.ProofOfSourceOfFundsDocType == "7")
                    {
                        englishContent = englishContent.Replace("{[InsuranceCompensation]}", "checked");
                        persianContent = persianContent.Replace("{[InsuranceCompensation]}", "checked");
                    }
                    else if (customer.ProofOfSourceOfFundsDocType == "8")
                    {
                        englishContent = englishContent.Replace("{[LotteryWinning123]}", "checked");
                        persianContent = persianContent.Replace("{[LotteryWinning123]}", "checked");
                    }
                    else if (customer.ProofOfSourceOfFundsDocType == "9")
                    {
                        englishContent = englishContent.Replace("{[OtherSourceofFunds]}", "checked");
                        persianContent = persianContent.Replace("{[OtherSourceofFunds]}", "checked");
                    }
                }

                tableFirstRowEnglish = tableFirstRowEnglish + "<td>" + cardQty + "<br/>" + "(MAX" + "&nbsp"
                    + cardType.MaxQuantity + ")" + "</td>";
                tableFirstRowPersian = tableFirstRowPersian + "<td>" + cardQty + "<br/>" + "(MAX" + "&nbsp"
                    + cardType.MaxQuantity + ")" + "</td>";
                if (cardType.IsReloadable)
                {
                    tableSecondRowEnglish = tableSecondRowEnglish + "<td>" + "Reloadable up to" + "&nbsp €"
                        + cardType.ReloadLimit.ToString("###,###.##") + "&nbsp" + "/Y</td>";
                    tableSecondRowPersian = tableSecondRowPersian + "<td>" + "بارگذاري تا" + "&nbsp"
                        + cardType.ReloadLimit.ToString("###,###.##") + "&nbsp" + "يورو در سال</td>";
                }
                else
                {
                    tableSecondRowEnglish = tableSecondRowEnglish + "<td>" + "Gift card MAX €"
                        + cardType.ReloadLimit.ToString("###,###.##") + "</td>";
                    tableSecondRowPersian = tableSecondRowPersian + "<td>" + "کارت هديه حداکثر"
                        + "&nbsp" + cardType.ReloadLimit.ToString("###,###.##") + "&nbsp" + "يورو</td>";
                }

                tableThirdRowEnglish = tableThirdRowEnglish + "<td>€" + cardType.Fee + "</td>";
                tableThirdRowPersian = tableThirdRowPersian + "<td>€" + cardType.Fee + "</td>";
                if (cardType.IsProofOfAddressRequired || cardType.IsProofOfSourceOfFundsRequired)
                {
                    tableFourthRowEnglish = tableFourthRowEnglish + "<td>✔</td>";
                    tableFourthRowPersian = tableFourthRowPersian + "<td>✔</td>";
                }
                else
                {
                    tableFourthRowEnglish = tableFourthRowEnglish + "<td>Nil</td>";
                    tableFourthRowPersian = tableFourthRowPersian + "<td>نيازي نيست</ td>";
                }
            }

            tableHeaderPersian = tableHeaderPersian + "<th>شرح</th>";
            tableFirstRowPersian = tableFirstRowPersian + "<td>تعداد</td>";
            tableSecondRowPersian = tableSecondRowPersian + "<td>امکانات</td>";
            tableThirdRowPersian = tableThirdRowPersian + "<td>هزينه صدور کارت</td>";
            tableFourthRowPersian = tableFourthRowPersian + "<td>ضرورت ارسال اسناد <span>:</span> به دستورالعمل مراجعه شود</td>";
            kycFormEnglishCardsTableHtml = "<table class=" + "table table-bordered" + "style=" + "padding: 10px; " + "><thead style=" + "background - color:#eee" + "><tr>" + tableHeaderEnglish + "</tr></thead><tbody><tr>" + tableFirstRowEnglish + "</tr><tr>" + tableSecondRowEnglish + "</tr><tr>" + tableThirdRowEnglish + "</tr>" + "<tr>" + tableFourthRowEnglish + "</tr></tbody></table>";
            kycFormPersianCardsTableHtml = "<table class=" + "table table-bordered" + "style=" + "padding: 10px; " + "><thead style=" + "background - color:#eee" + "><tr>" + tableHeaderPersian + "</tr></thead><tbody><tr>" + tableFirstRowPersian + "</tr><tr>" + tableSecondRowPersian + "</tr><tr>" + tableThirdRowPersian + "</tr>" + "<tr>" + tableFourthRowPersian + "</tr></tbody></table>";
            englishContent = englishContent.Replace("{[DynamicTable]}", kycFormEnglishCardsTableHtml);
            persianContent = persianContent.Replace("{[DynamicTable]}", kycFormPersianCardsTableHtml);

            filledCustomerTemplate[0] = englishContent;

            if (customer != null && customer.CardRequestForms != null)
                persianContent = persianContent.Replace("{[ApplicationNo]}"
                    , customer.CardRequestForms[0].ApplicationNumber);
            else
                persianContent = persianContent.Replace("{[ApplicationNo]}", applicationNumber);


            persianContent = persianContent.Replace("{[FirstName]}", user.FirstNameInCustomerLanguage);
            persianContent = persianContent.Replace("{[LastName]}", user.LastNameInCustomerLanguage);


            MaritalStatus maritalStatusPer = (MaritalStatus)customer.MaritalStatusId;
            persianContent = persianContent.Replace(maritalStatusPer == MaritalStatus.Married ? "{[MaritalMarriedCheckbox]}" : "{[MaritalSingleCheckbox]}", "checked");

            Gender genderPer = (Gender)customer.GenderId;
            persianContent = persianContent.Replace(genderPer == Gender.Male ? "{[MaleCheckbox]}" : "{[FemaleCheckbox]}", "checked");

            //persianContent = persianContent.Replace("{[Black]}", "5");
            //persianContent = persianContent.Replace("{[Platinum]}", "3");
            //persianContent = persianContent.Replace("{[Gold]}", "2");

            persianContent = persianContent.Replace("{[DateOfBirth]}", "" + customer.DateOfBirthInCustomerLanguage);
            persianContent = persianContent.Replace("{[IranianNationCheckbox]}", "checked"); //customer.Nationality
            persianContent = persianContent.Replace("{[Nationality]}", customer.Nationality);
            persianContent = persianContent.Replace("{[Address]}", customer.AddressInCustomerLanguage);
            //TODO : do this dynamic currently no transalation found
            persianContent = city == "Tehran" ? persianContent.Replace("{[City]}", "تهران") : persianContent.Replace("{[City]}", "" + customer.CityId);

            persianContent = persianContent.Replace("{[FullName]}", user.FirstNameInCustomerLanguage + "&nbsp" + user.LastNameInCustomerLanguage);
            persianContent = persianContent.Replace("{[NationalIdentificationNumber]}", customer.NIC);
            persianContent = persianContent.Replace("{[PostalCode]}", customer.PostalCode);
            persianContent = persianContent.Replace("{[E-mailAddress]}", user.Email);
            persianContent = persianContent.Replace("{[MobileNumber]}", customer.ContactNo);
            persianContent = persianContent.Replace("{[Date]}", DateTime.Now.ToString("yyyy/MM/dd"));
            persianContent = persianContent.Replace("{[PassportNumber]}", customer.PassportNo);
            filledCustomerTemplate[1] = persianContent;
            return filledCustomerTemplate;
        }

        public string UrlSafeEncrypt(string clearText)
        {
            var encrypt = Encrypt(clearText);
            //if (encrypt == null) throw new ArgumentNullException(nameof(encrypt));
            encrypt = HttpUtility.UrlEncode(encrypt);
            return encrypt;
        }
        private string Encrypt(string clearText)
        {
            try
            {
                string encryptionKey = ConfigurationManager.AppSettings["SecretKey"];
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    if (encryptor != null)
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(clearBytes, 0, clearBytes.Length);
                                cs.Close();
                            }
                            clearText = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return clearText;
        }


        public string UrlSafeDecrypt(string cipherText)
        {
            var decrypt = Decrypt(HttpUtility.UrlDecode(cipherText));
            return decrypt;
        }

        private string Decrypt(string cipherText)
        {
            try
            {
                string encryptionKey = ConfigurationManager.AppSettings["SecretKey"];
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    if (encryptor != null)
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(cipherBytes, 0, cipherBytes.Length);
                                cs.Close();
                            }
                            cipherText = Encoding.Unicode.GetString(ms.ToArray());
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return cipherText;
        }

        public DataTable GetTable(IList<RequestForm> model)
        {
            // NIC, Full Name, Transaction Number, Account Number and Amount etc.
            // Here we create a DataTable with four columns.
            DataTable table = new DataTable();
            table.TableName = "PendingPayments";

            table.Columns.Add("RequestNo", typeof(string));
            table.Columns.Add("FullName", typeof(string));
            table.Columns.Add("NIC", typeof(string));
            table.Columns.Add("TransactionNumber", typeof(string));
            table.Columns.Add("AccountNumber", typeof(string));
            table.Columns.Add("Amount", typeof(string));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("PaymentMode", typeof(string));
            table.Columns.Add("RequestDate", typeof(string));

            for (int i = 0; i < model.Count; i++)
            {
                string paymentDesc = string.Empty;
                foreach (var payment in model[i].Payments)
                {
                    paymentDesc = paymentDesc + "" +
                                    (payment.PaymentMethodId == (int)PaymentMethod.RAHYAB
                                    || payment.PaymentMethodId == (int)PaymentMethod.Admin
                                    ? "Manual" : ((PaymentMethod)payment.PaymentMethodId).ToString())
                                    + ",";
                    paymentDesc = paymentDesc.Trim(',');
                }
                table.Rows.Add(model[i].Id, model[i].Customer.User.FullName, model[i].Customer.NIC, model[i].Payments[0].TransactionNo, model[i].Payments[0].TransactionAccountNo
                    , Math.Round((model[i].Payments[0].Amount), 2), (RequestFormType)model[i].TypeId, paymentDesc, model[i].CreatedOn.ToString("yyyy/MM/dd HH:mm"));
            }
            // Here we add five DataRows.
            //table.Rows.Add(1 "Indocin", "David", DateTime.Now);
            //table.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
            //table.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
            //table.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
            //table.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);
            return table;
        }

        public DataTable GetBalaceAndTransactionTable(Card card)
        {
            DataTable table = new DataTable();
            table.TableName = "BalanceTransaction";

            //table.Rows.Add("Balance and Transactions");
            //table.Rows.Add("Balance" + ":" + card.Balance.ToString());

            table.Columns.Add("No", typeof(string));
            table.Columns.Add("Date", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Amount", typeof(string));
            table.Columns.Add("Fee", typeof(string));
            int i = 0;
            foreach (var paymentTransaction in card.PaymentTransactions)
            {
                i++;
                table.Rows.Add(i, paymentTransaction.CreatedOn.ToString("yyyy-mm-dd HH:mm:ss 'GMT'")
                    , paymentTransaction.Description, paymentTransaction.Status
                    , paymentTransaction.IsDebit ? "Out" : "In", paymentTransaction.Amount.ToString("0.00")
                    , paymentTransaction.AccountServiceFee);
            }
            return table;
        }

        public string ConvertImgToBase64(string imageFile)
        {
            try
            {
                if (!string.IsNullOrEmpty(imageFile))
                {
                    var b = System.IO.File.ReadAllBytes(imageFile);
                    string imageBase64 = Convert.ToBase64String(b);
                    return string.Format("data:image/gif;base64,{0}", imageBase64);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public void Save_EngKycImage(byte[] imgArray, Customer customer)
        {
            try
            {
                Image image;
                var documentService = new DocumentService();
                string newFileName = customer.NIC + "-KycFormInEnglish" + ".Jpeg";
                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                using (MemoryStream inStream = new MemoryStream())
                {
                    inStream.Write(imgArray, 0, imgArray.Length);
                    image = Bitmap.FromStream(inStream);
                    image.Save(path, ImageFormat.Jpeg);
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
            }
        }

        public void Save_PersKycImage(byte[] imgArray, Customer customer)
        {
            try
            {
                Image image;
                var documentService = new DocumentService();
                string newFileName = customer.NIC + "-KycFormInPersian" + ".Jpeg";
                var path = Path.Combine(documentService.GetDocumentDirectoryPath(customer.NIC), newFileName);
                using (MemoryStream inStream = new MemoryStream())
                {
                    inStream.Write(imgArray, 0, imgArray.Length);
                    image = Bitmap.FromStream(inStream);
                    image.Save(path, ImageFormat.Jpeg);
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
            }
        }
        public void ExportHtml(string content, string filePath)
        {
            try
            {
                StreamWriter swXLS = new StreamWriter(filePath);
                swXLS.Write(content.ToString());
                swXLS.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
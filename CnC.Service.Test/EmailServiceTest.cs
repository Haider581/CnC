using CnC.Core.Accounts;
using CnC.Core.Customers;
using CnC.Core.Messages;
using CnC.Data;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Service.Test
{
    [TestClass]
    public class EmailServiceTest
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        [TestMethod]
        public void Email_Test()
        {
            //var user = new User();
            //user.Email = "Haider.mustafa581@gmail.com";
            var customer = new Customer();
            customer.Address = "its address";
            customer.AddressInCustomerLanguage = "its persian address";
            customer.Answer = "Test Answer";
            //customer.AuthoritySignedForm = "Test Authority Signed Form";
            customer.BillingAddress = "its billing address";
            customer.CardIssuerRespondedOn = DateTime.Now;
           // customer.CardServiceProviderClientId = "12345678";
            customer.CityId = 1;
            customer.ContactNo = "03292090330";
            customer.CountryId = 1;
            customer.CustomerSignedForm = "Customer signed form Test";
            customer.CustomerSignedFormInCustomerLanguage = "its customer signed form in customer language";
            customer.DateOfBirth = DateTime.Now;
            customer.EmailForShopping = "hello@gmail.com";
            customer.NIC = "2345345634";
            customer.PassportNo = "A09090937";
            customer.PostalCode = "222000";
            customer.User = new User();
            //customer.UserId = 1111;
            //customer.User.Email = user.Email;
            SendCustomerWelcomeMessage_Test(customer, "Haider_m9", 1, true);
        }
        public int SendCustomerWelcomeMessage_Test(Customer customer, string password, int languageId, bool isOnline)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            //if (customer.Id <= 0)
            //    throw new ArgumentException("Customer Id is invalid");

            try
            {
                //if (customer.User == null)
                //    customer.User = new UserService().GetUser(customer.UserId);

                string templateName = "Customer.WelcomeMessage";

                if (isOnline)
                    templateName = "Customer.WelcomeMessage.Online";

                var localizedMessageTemplate = this.GetLocalizedMessageTemplate_Test(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount_Test(localizedMessageTemplate.EmailAccountId);

                string subject = localizedMessageTemplate.Subject;

                string body;
                if (isOnline)
                {
                    body = localizedMessageTemplate.Body.Replace("{[CustomerName]}", "Mr Test")
                  .Replace("{[Url]}", new SettingService().CustomerVerificationUrl + new CommonService().UrlSafeEncrypt("1111"))
                  .Replace("{[UserName]}", "Haider.mustafa581@gmail.com")
                  .Replace("{[Password]}", password);
                }
                else
                {
                    body = localizedMessageTemplate.Body.Replace("{[CustomerName]}", customer.User.FullName)
                   .Replace("{[Url]}", new SettingService().CustomerLoginUrl)
                   .Replace("{[UserName]}", customer.User.Email)
                   .Replace("{[Password]}", password);
                }

                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = "ounabbas_me@hotmail.com";

                int queuedEmailId = AddQueuedEmail_Test(5, from, fromName, to, "Mr Test", string.Empty, string.Empty
                    , subject, body, DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail_Test(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, "Mr Test",
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return -1;
            }
        }
        public bool SendEmail_Test(string host, int port, string emailUsername, string emailPassword, bool enableSSL
           , string from, string fromName, string to, string toName, string cc, string bcc, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(from))
                    throw new ArgumentNullException("From is null");

                if (string.IsNullOrEmpty(to))
                    throw new ArgumentNullException("To is null");

                using (MailMessage mail = new MailMessage())
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TestEmail"]))
                        to = ConfigurationManager.AppSettings["TestEmail"];

                    mail.From = new MailAddress(from);

                    foreach (var item in to.Trim().Split(','))
                        mail.To.Add(item.Trim());

                    if (!string.IsNullOrEmpty(cc))
                        foreach (var item in cc.Trim().Split(','))
                            mail.CC.Add(item.Trim());

                    if (!string.IsNullOrEmpty(bcc))
                        foreach (var item in bcc.Trim().Split(','))
                            mail.Bcc.Add(item.Trim());

                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailBCC"]))
                        foreach (var item in ConfigurationManager.AppSettings["EmailBCC"].Trim().Split(','))
                            mail.Bcc.Add(item.Trim());

                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(host, port))
                    {
                        smtp.Credentials = new NetworkCredential(emailUsername, emailPassword);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                    return true;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return false;
            }
        }
        public int AddQueuedEmail_Test(int priority, string from,
            string fromName, string to, string toName, string cc, string bcc,
            string subject, string body, DateTime createdOn, int sendTries,
            DateTime? sentOn, int emailAccountId)
        {
            CommonService commonService = new CommonService();

            from = commonService.EnsureNotNull(from);
            from = commonService.EnsureMaximumLength(from, 500);
            fromName = commonService.EnsureNotNull(fromName);
            fromName = commonService.EnsureMaximumLength(fromName, 500);
            to = commonService.EnsureNotNull(to);
            to = commonService.EnsureMaximumLength(to, 500);
            toName = commonService.EnsureNotNull(toName);
            toName = commonService.EnsureMaximumLength(toName, 500);
            cc = commonService.EnsureNotNull(cc);
            cc = commonService.EnsureMaximumLength(cc, 500);
            bcc = commonService.EnsureNotNull(bcc);
            bcc = commonService.EnsureMaximumLength(bcc, 500);
            subject = commonService.EnsureNotNull(subject);
            subject = commonService.EnsureMaximumLength(subject, 500);
            body = commonService.EnsureNotNull(body);


            var queuedEmail = new QueuedEmail();
            queuedEmail.Priority = priority;
            queuedEmail.From = from;
            queuedEmail.FromName = fromName;
            queuedEmail.To = to;
            queuedEmail.ToName = toName;
            queuedEmail.CC = cc;
            queuedEmail.Bcc = bcc;
            queuedEmail.Subject = subject;
            queuedEmail.Body = body;
            queuedEmail.CreatedOn = createdOn;
            queuedEmail.SendTries = sendTries;
            queuedEmail.SentOn = sentOn;
            queuedEmail.EmailAccountId = emailAccountId;

            try
            {
                using (var context = new EntityContext())
                {
                    context.QueuedEmails.Add(queuedEmail);
                    context.SaveChanges();
                }

                return queuedEmail.Id;
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return -1;
            }
        }
        public LocalizedMessageTemplate GetLocalizedMessageTemplate_Test(string name, int languageId)
        {
            try
            {
                var settingService = new SettingService();
                if (!settingService.IsEmailLocalizationEnabled)
                    languageId = settingService.SystemLanguage.Id;

                using (var context = new EntityContext())
                {
                    var query = from lmt in context.LocalizedMessageTemplates
                                join mt in context.MessageTemplates on lmt.MessageTemplateId equals mt.Id
                                where lmt.LanguageId == languageId/* && lmt.IsActive==true*/ &&
                                mt.Name == name
                                select lmt;
                    var localizedMessageTemplate = query.FirstOrDefault();
                    return localizedMessageTemplate;
                }
            }
            catch (Exception exception)
            {
                //ILog.Error(exception);
                return null;
            }
        }

        public EmailAccount GetEmailAccount_Test(int emailAccountId)
        {
            if (emailAccountId == 0)
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    return context.EmailAccounts.Where(ea => ea.Id == emailAccountId).SingleOrDefault();
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }
        }
    }
}

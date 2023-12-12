using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Cards;
using CnC.Core.Common;
using CnC.Core.Customers;
using CnC.Core.Exceptions;
using CnC.Core.Messages;
using CnC.Core.Payments;
using CnC.Core.TopUps;
using CnC.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Service
{
    public class MessageService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        private SettingService settingService;

        public MessageService()
        {
            settingService = new SettingService();
        }

        #region Message Template

        public int AddLocalizedMessageTemplate(LocalizedMessageTemplate localizedMessageTemplate)
        {
            if (localizedMessageTemplate == null)
                throw new ArgumentNullException("localizedMessageTemplate");

            CommonService commonService = new CommonService();

            localizedMessageTemplate.Subject = commonService.EnsureNotNull(localizedMessageTemplate.Subject);
            localizedMessageTemplate.Subject = commonService.EnsureMaximumLength(localizedMessageTemplate.Subject, 200);
            localizedMessageTemplate.Body = commonService.EnsureNotNull(localizedMessageTemplate.Body);

            try
            {
                using (var context = new EntityContext())
                {
                    context.LocalizedMessageTemplates.Add(localizedMessageTemplate);
                    context.SaveChanges();
                }
                return localizedMessageTemplate.Id;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public LocalizedMessageTemplate GetLocalizedMessageTemplate(int localizedMessageTemplateId)
        {
            if (localizedMessageTemplateId == 0)
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from localizedMessage in context.LocalizedMessageTemplates
                                  join
                                 messageTemplates in context.MessageTemplates on
                                 localizedMessage.MessageTemplateId equals messageTemplates.Id
                                  join language in context.Languages on
                                  localizedMessage.LanguageId equals language.Id
                                  where localizedMessage.Id == localizedMessageTemplateId
                                  select new
                                  {
                                      localizedMessage,
                                      messageTemplates,
                                      language
                                  }).SingleOrDefault();
                    var loclalizedMessageTemplate = new LocalizedMessageTemplate();
                    loclalizedMessageTemplate = result.localizedMessage;
                    loclalizedMessageTemplate.MessageTemplate = result.messageTemplates;
                    loclalizedMessageTemplate.Language = result.language;
                    return loclalizedMessageTemplate;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public bool UpdateLoclizedMessageTemplate(LocalizedMessageTemplate localizedMessageTemplate)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (localizedMessageTemplate != null)
                    {
                        var localizedMessage = context.LocalizedMessageTemplates
                            .SingleOrDefault(c => c.Id == localizedMessageTemplate.Id);
                        localizedMessage.LanguageId = localizedMessageTemplate.LanguageId;
                        localizedMessage.Subject = localizedMessageTemplate.Subject;
                        localizedMessage.Body = localizedMessageTemplate.Body;

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

        public List<LocalizedMessageTemplate> GetLocalizedMessageTemplates(out int totalCount, string templateName = null
            , string subject = null, int? languageId = null, int pageIndex = 0, int? pageSize = null)
        {
            totalCount = 0;
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;
            int skipRows = pageSize.Value * pageIndex;
            if (languageId == 0)
                languageId = null;
            try
            {
                using (var context = new EntityContext())
                {
                    var data = (from lmt in context.LocalizedMessageTemplates
                                join mt in context.MessageTemplates on lmt.MessageTemplateId equals mt.Id
                                join l in context.Languages on lmt.LanguageId equals l.Id
                                join email in context.EmailAccounts on lmt.EmailAccountId equals email.Id
                                where
                                (true == (!string.IsNullOrEmpty(templateName)
                                ? mt.Name.Contains(templateName.ToLower()) : true))
                                && (true == (!string.IsNullOrEmpty(subject)
                                ? lmt.Subject.Contains(subject.ToLower()) : true))
                                && l.Id == (languageId.HasValue ? languageId.Value : l.Id)
                                select new
                                {
                                    localizedMessageTemplates = lmt,
                                    messageTemplates = mt,
                                    lanaguages = l,
                                    emailAccount = email
                                }).OrderBy(c => c.messageTemplates.Id);
                    totalCount = data.Count();
                    var result = data.Skip(skipRows).Take(pageSize.Value).ToList();
                    var listLocalizedMessageTemplates = new List<LocalizedMessageTemplate>();
                    if (data != null && data.Count() > 0)
                    {
                        foreach (var localizedMessage in data)
                        {
                            var localizedMessageTemplate = new LocalizedMessageTemplate();
                            localizedMessageTemplate = localizedMessage.localizedMessageTemplates;
                            localizedMessageTemplate.MessageTemplate = localizedMessage.messageTemplates;
                            localizedMessageTemplate.Language = localizedMessage.lanaguages;
                            localizedMessageTemplate.EmailAccount = localizedMessage.emailAccount;
                            listLocalizedMessageTemplates.Add(localizedMessageTemplate);
                        }
                    }
                    return listLocalizedMessageTemplates;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<LocalizedMessageTemplate> GetLocalizedMessageTemplates()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.LocalizedMessageTemplates.OrderBy(t => t.Id).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public LocalizedMessageTemplate GetLocalizedMessageTemplate(string name, int languageId)
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
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<MessageTemplate> GetMessagesTemplates()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.MessageTemplates.OrderBy(c => c.Id).ToList();
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

        #region Email Account

        /// <summary>
        /// Add an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public bool AddEmailAccount(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            CommonService commonService = new CommonService();

            emailAccount.Email = commonService.EnsureNotNull(emailAccount.Email);
            emailAccount.DisplayName = commonService.EnsureNotNull(emailAccount.DisplayName);
            emailAccount.Host = commonService.EnsureNotNull(emailAccount.Host);
            emailAccount.Username = commonService.EnsureNotNull(emailAccount.Username);
            emailAccount.Password = commonService.EnsureNotNull(emailAccount.Password);

            emailAccount.Email = emailAccount.Email.Trim();
            emailAccount.DisplayName = emailAccount.DisplayName.Trim();
            emailAccount.Host = emailAccount.Host.Trim();
            emailAccount.Username = emailAccount.Username.Trim();
            emailAccount.Password = emailAccount.Password.Trim();

            emailAccount.Email = commonService.EnsureMaximumLength(emailAccount.Email, 255);
            emailAccount.DisplayName = commonService.EnsureMaximumLength(emailAccount.DisplayName, 255);
            emailAccount.Host = commonService.EnsureMaximumLength(emailAccount.Host, 255);
            emailAccount.Username = commonService.EnsureMaximumLength(emailAccount.Username, 255);
            emailAccount.Password = commonService.EnsureMaximumLength(emailAccount.Password, 255);
            emailAccount.EnableSSL = emailAccount.EnableSSL;
            emailAccount.Port = emailAccount.Port;

            try
            {
                using (var context = new EntityContext())
                {
                    context.EmailAccounts.Add(emailAccount);
                    if (context.SaveChanges() <= 0)
                    {
                        return false;
                    }
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

        public EmailAccount GetEmailAccount(int emailAccountId)
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
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public List<EmailAccount> GetEmailAccounts(out int totalCount, string email = null, string displayName = null
                                                 , int pageIndex = 0, int? pageSize = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var emailAccounts = (from query in context.EmailAccounts
                                         where true == (!string.IsNullOrEmpty(email) ? query.Email.ToLower()
                                         .Contains(email.ToLower()) : true)
                                  && true == (!string.IsNullOrEmpty(displayName) ? query.DisplayName.ToLower()
                                  .Contains(displayName.ToLower()) : true)
                                         select query);
                    totalCount = emailAccounts.Count();
                    if (emailAccounts != null && emailAccounts.Count() > 0)
                    {

                        return emailAccounts.OrderBy(ea => ea.Id).Skip(skipRows).Take(pageSize.Value).ToList();
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

        public bool UpdateEmailAccount(EmailAccount emailAccount)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (emailAccount != null)
                    {
                        var emailAccounts = context.EmailAccounts.SingleOrDefault(c => c.Id == emailAccount.Id);

                        CommonService commonService = new CommonService();

                        emailAccounts.Email = commonService.EnsureNotNull(emailAccount.Email);
                        emailAccounts.DisplayName = commonService.EnsureNotNull(emailAccount.DisplayName);
                        emailAccounts.Host = commonService.EnsureNotNull(emailAccount.Host);
                        emailAccounts.Username = commonService.EnsureNotNull(emailAccount.Username);
                        emailAccounts.Password = commonService.EnsureNotNull(emailAccount.Password);

                        emailAccounts.Email = emailAccount.Email.Trim();
                        emailAccounts.DisplayName = emailAccount.DisplayName.Trim();
                        emailAccounts.Host = emailAccount.Host.Trim();
                        emailAccounts.Username = emailAccount.Username.Trim();
                        emailAccounts.Password = emailAccount.Password.Trim();

                        emailAccounts.Email = commonService.EnsureMaximumLength(emailAccount.Email, 255);
                        emailAccounts.DisplayName = commonService.EnsureMaximumLength(emailAccount.DisplayName, 255);
                        emailAccounts.Host = commonService.EnsureMaximumLength(emailAccount.Host, 255);
                        emailAccounts.Username = commonService.EnsureMaximumLength(emailAccount.Username, 255);
                        emailAccounts.Password = commonService.EnsureMaximumLength(emailAccount.Password, 255);
                        emailAccounts.EnableSSL = emailAccount.EnableSSL;
                        emailAccounts.Port = emailAccount.Port;
                        if (context.SaveChanges() <= 0)
                            return false;

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
        #endregion

        #region Queued Email
        public int AddQueuedEmail(int priority, string from,
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
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }
        public bool UpdateQueuedEmail(QueuedEmail queuedEmail)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (queuedEmail != null)
                    {
                        var queuedEmails = context.QueuedEmails.SingleOrDefault(qe => qe.Id == queuedEmail.Id);
                        CommonService commonService = new CommonService();

                        queuedEmails.From = commonService.EnsureNotNull(queuedEmail.From);
                        queuedEmails.From = commonService.EnsureMaximumLength(queuedEmail.From, 500);
                        queuedEmails.FromName = commonService.EnsureNotNull(queuedEmail.FromName);
                        queuedEmails.FromName = commonService.EnsureMaximumLength(queuedEmail.FromName, 500);
                        queuedEmails.To = commonService.EnsureNotNull(queuedEmail.To);
                        queuedEmails.To = commonService.EnsureMaximumLength(queuedEmail.To, 500);
                        queuedEmails.ToName = commonService.EnsureNotNull(queuedEmail.ToName);
                        queuedEmails.ToName = commonService.EnsureMaximumLength(queuedEmail.ToName, 500);
                        queuedEmails.CC = commonService.EnsureNotNull(queuedEmail.CC);
                        queuedEmails.CC = commonService.EnsureMaximumLength(queuedEmail.CC, 500);
                        queuedEmails.Bcc = commonService.EnsureNotNull(queuedEmail.Bcc);
                        queuedEmails.Bcc = commonService.EnsureMaximumLength(queuedEmail.Bcc, 500);
                        queuedEmails.Subject = commonService.EnsureNotNull(queuedEmail.Subject);
                        queuedEmails.Subject = commonService.EnsureMaximumLength(queuedEmail.Subject, 500);
                        queuedEmails.Body = commonService.EnsureNotNull(queuedEmail.Body);

                        queuedEmails.Priority = queuedEmail.Priority;
                        queuedEmails.CreatedOn = queuedEmail.CreatedOn;
                        queuedEmails.SendTries = queuedEmail.SendTries;
                        queuedEmails.SentOn = queuedEmail.SentOn;
                        queuedEmails.EmailAccountId = queuedEmail.EmailAccountId;

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
        public bool AddQueuedEmail(QueuedEmail queuedEmail)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (queuedEmail != null)
                    {

                        context.QueuedEmails.Add(queuedEmail);
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
        public QueuedEmail GetQueuedEmail(int queuedEmailId)
        {
            if (queuedEmailId == 0)
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    return context.QueuedEmails.Where(ea => ea.Id == queuedEmailId).SingleOrDefault();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public List<QueuedEmail> GetQueuedEmails(out int totalCount, string subject = null, string fromSender = null, string to = null, int pageIndex = 0, int? pageSize = null)
        {
            CommonService commonService = new CommonService();
            fromSender = commonService.EnsureNotNull(fromSender);
            fromSender = commonService.EnsureMaximumLength(fromSender, 500);
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var queuedEmails = (from query in context.QueuedEmails
                                        where true == (!string.IsNullOrEmpty(subject) ? query.Subject.ToLower().Contains(subject.ToLower()) : true)
                                         && true == (!string.IsNullOrEmpty(fromSender) ? query.From.ToLower().Contains(fromSender.ToLower()) : true)
                                         && true == (!string.IsNullOrEmpty(to) ? query.To.ToLower().Contains(to.ToLower()) : true)
                                        select query);
                    totalCount = queuedEmails.Count();
                    if (queuedEmails != null && queuedEmails.Count() > 0)
                    {

                        return queuedEmails.OrderBy(qe => qe.Id).Skip(skipRows).Take(pageSize.Value).ToList();
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
        #endregion

        #region Send Email

        public bool SendEmail(string host, int port, string emailUsername, string emailPassword, bool enableSSL
           , string from, string fromName, string to, string toName, string cc, string bcc, string subject, string body,
            string attachments = null)
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

                    if (!string.IsNullOrEmpty(attachments))
                        foreach (var attachment in attachments.Trim().Split(','))
                            mail.Attachments.Add(new Attachment(attachment.Trim()));

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
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        #region Customer

        public int SendCustomerWelcomeMessage(Customer customer, string password, int languageId, bool isOnline,
            string domain = "")
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.Id <= 0)
                throw new ArgumentException("Customer Id is invalid");

            try
            {
                if (customer.User == null)
                    customer.User = new UserService().GetUser(customer.UserId);

                string templateName = "Customer.WelcomeMessage";

                if (isOnline)
                    templateName = "Customer.WelcomeMessage.Online";

                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string subject = localizedMessageTemplate.Subject;

                string body;
                if (isOnline)
                {
                    body = localizedMessageTemplate.Body.Replace("{[CustomerName]}", customer.User.FullName)
                  .Replace("{[Url]}", domain + settingService.CustomerVerificationUrl +
                  new CommonService().UrlSafeEncrypt(customer.UserId.ToString()))
                  .Replace("{[UserName]}", customer.User.Email)
                  .Replace("{[Password]}", password);
                }
                else
                {
                    body = localizedMessageTemplate.Body.Replace("{[CustomerName]}", customer.User.FullName)
                   .Replace("{[Url]}", settingService.CustomerLoginUrl)
                   .Replace("{[UserName]}", customer.User.Email)
                   .Replace("{[Password]}", password);
                }

                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty
                    , subject, body, DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int SendCustomerValidationFailedMessage(Customer customer, string reason, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            if (customer.Id <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException("Reason is required");

            try
            {
                string templateName = "Customer.ValidationFailed";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);

                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (customer.User == null)
                    customer.User = new UserService().GetUser(customer.UserId);

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string body = localizedMessageTemplate.Body
                    .Replace("{[CustomerName]}", customer.User.FullName)
                    .Replace("{[Reasons]}", reason);

                string subject = localizedMessageTemplate.Subject;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int SendCustomerCardIssuerDeclinedMessage(Customer customer, string reason, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer is required");

            if (customer.Id <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException("Reason is required");

            try
            {
                string templateName = "Customer.CardIssuerDeclined";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);

                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (customer.User == null)
                    customer.User = new UserService().GetUser(customer.UserId);

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string body = localizedMessageTemplate.Body
                    .Replace("{[CustomerName]}", customer.User.FullName)
                    .Replace("{[Reasons]}", reason);

                string subject = localizedMessageTemplate.Subject;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        #endregion

        #region ChangePassword

        public int SendChangePasswordMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("User");

            if (user.Id <= 0)
                throw new ArgumentException("User Id is invalid");

            try
            {
                string templateName = "Customer.ChangePassword";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string subject = localizedMessageTemplate.Subject;
                string body = localizedMessageTemplate.Body.Replace("{[CustomerName]}", user.FullName);
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = user.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, user.FullName, string.Empty, string.Empty
                    , subject, body, DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, user.FullName,
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int SendChangePasswordMessageToUser(User user, User customer, string password, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("User");

            if (user.Id <= 0)
                throw new ArgumentException("User Id is invalid");

            try
            {
                string templateName = "User.ChangePassword";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string subject = localizedMessageTemplate.Subject;
                string body = localizedMessageTemplate.Body.Replace("{[CustomerName]}", customer.FullName)
                    .Replace("{[User]}", user.FullName)
                    .Replace("{[Password]}", password)
                    .Replace("{[UserName]}", customer.Username)
                    .Replace("{[Email]}", customer.Email);

                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = user.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, user.FullName, string.Empty, string.Empty
                    , subject, body, DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, user.FullName,
                    string.Empty, localizedMessageTemplate.BccEmailAddresses, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int SendForgotPasswordCodeMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer");


            try
            {
                string templateName = "Customer.ForgotPasswordCode";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string subject = localizedMessageTemplate.Subject;
                string body = localizedMessageTemplate.Body;
                body = localizedMessageTemplate.Body.Replace("{{VerificationCode}}", customer.VerificationCode)
                    .Replace("{[CustomerName]}", customer.User.FullName);
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, string.Empty,
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }
        #endregion ChangePassword

        #region Send Top Up Request Message

        public int SendCustomerNewTopUpRequestMessage(Customer customer, RequestForm requestForm
            , List<TopUpRequest> topUpRequests,
            int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.Id <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (requestForm == null)
                throw new ArgumentNullException("requestForm");

            if (requestForm.Id <= 0)
                throw new ArgumentException("Request Form Id is invalid");

            if (topUpRequests == null || topUpRequests.Count == 0)
                throw new ArgumentNullException("topUpRequests");

            //if (topUpRequests.Any(tr => tr.Id <= 0))
            //    throw new ArgumentException("Top Up Requests Ids are invalid");

            try
            {
                string templateName = "Customer.NewTopUpRequest";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string body = localizedMessageTemplate.Body;

                StringBuilder requestDetail = new StringBuilder();
                Currency systemCurrency = settingService.SystemCurrency;
                Currency customerCurrency = settingService.CustomerDefaultCurrency;
                CardService cardService = new CardService();
                decimal TotalAmount = 0;
                string tempCardNumber = string.Empty;
                foreach (TopUpRequest topUpRequest in topUpRequests)
                {
                    Card card = topUpRequest.Card;

                    if (card == null)
                        card = cardService.GetCard(topUpRequest.CardId);

                    requestDetail.AppendLine(string.Format("Card Number : {0}", card.Number));
                    requestDetail.AppendLine(string.Format("Amount ({0}) : {1}", systemCurrency.Code, topUpRequest.Amount));
                    requestDetail.AppendLine(string.Format("Exchange Rate : 1 {0} = {1} {2}",
                        systemCurrency.Code, requestForm.ExchangeRate, customerCurrency.Code));
                    //requestDetail.AppendLine(string.Format("Amount ({0}) : {1}",
                    //    customerCurrency.Code, topUpRequest.Amount * requestForm.ExchangeRate));
                    tempCardNumber = card.Number + "," + tempCardNumber;
                    TotalAmount = TotalAmount + topUpRequest.Amount; //* requestForm.ExchangeRate;
                }


                //       body = body.Replace("{[RequestDate]}", requestForm.CreatedOn.ToString("MM:dd:yyyy HH:mm"))
                //           .Replace("{[RequestDetail]}", requestDetail.ToString());

                body = localizedMessageTemplate.Body.Replace("{[CustomerName]}", customer.User.FullName)
                   .Replace("{[CardNumber]}", tempCardNumber.ToString())
                  .Replace("{[Amount]}", systemCurrency.Code + " " + TotalAmount.ToString());
                //   .Replace("{[UserName]}", customer.User.Email)
                //   .Replace("{[Password]}", password);

                string subject = localizedMessageTemplate.Subject;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, localizedMessageTemplate.BccEmailAddresses, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int SendCustomerTopUpRequestCompletedMessage(Customer customer, RequestForm requestForm
            , TopUpRequest topUpRequest, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (requestForm == null)
                throw new ArgumentNullException("request Form");

            if (requestForm.Id <= 0)
                throw new ArgumentException("Request Form Id is invalid");

            if (topUpRequest == null)
                throw new ArgumentNullException("Top Up Request");

            if (topUpRequest.Id <= 0)
                throw new ArgumentException("Top Up Id is invalid");

            try
            {
                string templateName = "Customer.TopUpRequestCompleted";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (customer.User == null)
                    customer.User = new UserService().GetUser(customer.UserId);

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                Currency systemCurrency = settingService.SystemCurrency;
                Currency customerCurrency = settingService.CustomerDefaultCurrency;
                CardService cardService = new CardService();

                Card card = topUpRequest.Card;

                if (card == null)
                    card = cardService.GetCard(topUpRequest.CardId);

                string body = localizedMessageTemplate.Body
                .Replace("{[CardNumber]}", card.Number)
                .Replace("{[CustomerName]}", customer.User.FullName)
                .Replace("{[SystemCurrency]}", systemCurrency.Code)
                .Replace("{[AmountInSystemCurrency]}", topUpRequest.Amount.ToString())
                .Replace("{[ExchangeRate]}", requestForm.ExchangeRate.ToString())
                .Replace("{[CustomerCurrency]}", customerCurrency.Code)
                .Replace("{[AmountInCustomerCurrency]}", (topUpRequest.Amount * requestForm.ExchangeRate).ToString())
                .Replace("{[RequestDate]}", requestForm.CreatedOn.ToString("MM:dd:yyyy HH:mm"));

                string subject = localizedMessageTemplate.Subject;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, localizedMessageTemplate.BccEmailAddresses, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int SendCustomerTopUpRequestFailedMessage(Customer customer, RequestForm requestForm
            , TopUpRequest topUpRequest,
            string reason, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.Id <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (requestForm == null)
                throw new ArgumentNullException("request Form");

            if (requestForm.Id <= 0)
                throw new ArgumentException("Request Form Id is invalid");

            if (topUpRequest == null)
                throw new ArgumentNullException("Top Up Request");

            if (topUpRequest.Id <= 0)
                throw new ArgumentException("Top Up Request Id is invalid");

            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException("Reason is required");

            try
            {
                string templateName = "Customer.TopUpRequestFailed";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (customer.User == null)
                    customer.User = new UserService().GetUser(customer.UserId);

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                Currency systemCurrency = settingService.SystemCurrency;
                Currency customerCurrency = settingService.CustomerDefaultCurrency;
                CardService cardService = new CardService();

                Card card = topUpRequest.Card;

                if (card == null)
                    card = cardService.GetCard(topUpRequest.CardId);

                string body = localizedMessageTemplate.Body
                .Replace("{[CardNumber]}", card.Number)
                .Replace("{[CustomerName]}", customer.User.FullName)
                .Replace("{[SystemCurrency]}", systemCurrency.Code)
                .Replace("{[AmountInSystemCurrency]}", topUpRequest.Amount.ToString())
                .Replace("{[ExchangeRate]}", requestForm.ExchangeRate.ToString())
                .Replace("{[CustomerCurrency]}", customerCurrency.Code)
                .Replace("{[AmountInCustomerCurrency]}", (topUpRequest.Amount * requestForm.ExchangeRate).ToString())
                .Replace("{[RequestDate]}", requestForm.CreatedOn.ToString("MM:dd:yyyy HH:mm"))
                .Replace("{[Reasons]}", reason);

                string subject = localizedMessageTemplate.Subject;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        #endregion Send Top Up Request Message

        #region Send Card Request Message

        public int SendCustomerNewCardRequestMessage(Customer customer, RequestForm requestForm
            , List<CardRequest> cardRequests,
            int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.Id <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (requestForm == null)
                throw new ArgumentNullException("requestForm");

            if (requestForm.Id <= 0)
                throw new ArgumentException("Request Form Id is invalid");

            if (cardRequests == null || cardRequests.Count == 0)
                throw new ArgumentNullException("cardRequests");

            //if (cardRequests.Any(cr => cr.Id <= 0))
            //    throw new ArgumentException("Card Requests Ids are invalid");

            try
            {
                string templateName = "Customer.NewCardRequest";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string body = localizedMessageTemplate.Body;

                StringBuilder requestDetail = new StringBuilder();
                Currency systemCurrency = settingService.SystemCurrency;
                Currency customerCurrency = settingService.CustomerDefaultCurrency;
                CardService cardService = new CardService();

                var cardRequestsGroupBy = cardRequests.GroupBy(cr => cr.CardTypeId);
                foreach (var cardRequestGroupBy in cardRequestsGroupBy)
                {
                    var cardRequest = cardRequestGroupBy.First();
                    CardType cardType = cardRequest.CardType;

                    if (cardType == null)
                        cardType = cardService.GetCardType(cardRequest.CardTypeId);

                    requestDetail.AppendLine(string.Format("Card Type : {0}", cardType.Name));
                    requestDetail.AppendLine(string.Format("Quantity : {0}", cardRequestGroupBy.Sum(cr => cr.CardQty)));
                }

                body = body.Replace("{[RequestDate]}", requestForm.CreatedOn.ToString("MM:dd:yyyy HH:mm"))
                     .Replace("{[CustomerName]}", customer.User.FullName)
                    .Replace("{[RequestDetail]}", requestDetail.ToString());

                string subject = localizedMessageTemplate.Subject;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, localizedMessageTemplate.BccEmailAddresses, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int SendCustomerCardRequestCompletedMessage(Customer customer, Card card, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.Id <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (card == null)
                throw new ArgumentNullException("card");

            if (card.CardRequestId <= 0)
                throw new ArgumentException("Card Id is invalid");

            try
            {
                string templateName = "Customer.CardRequestCompleted";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (customer.User == null)
                    customer.User = new UserService().GetUser(customer.UserId);

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                CardService cardService = new CardService();

                if (card.CardRequest == null)
                    card.CardRequest = cardService.GetCardRequest(card.CardRequestId);

                if (card.CardRequest.CardType == null)
                    card.CardRequest.CardType = cardService.GetCardType(card.CardRequest.CardTypeId);

                if (card.CardRequest.CardRequestForm == null)
                    card.CardRequest.CardRequestForm = cardService.GetCardRequestForm(card.CardRequest.CardRequestFormId);

                string body = localizedMessageTemplate.Body
                    .Replace("{[CardNumber]}", card.Number)
                    .Replace("{[CustomerName]}", customer.User.FullName)
                    .Replace("{[CardType]}", card.CardRequest.CardType.Name)
                    .Replace("{[RequestDate]}", card.CardRequest.CardRequestForm.CreatedOn.ToString("MM:dd:yyyy HH:mm"));

                string subject = localizedMessageTemplate.Subject;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, localizedMessageTemplate.BccEmailAddresses, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int SendCustomerCardRequestDeclinedMessage(Customer customer, CardRequest cardRequest, string reason
                                                         , int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.Id <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (cardRequest == null)
                throw new ArgumentNullException("cardRequest");

            if (cardRequest.Id <= 0)
                throw new ArgumentException("Card Request Id is invalid");

            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException("Reason is required");

            try
            {
                string templateName = "Customer.CardRequestFailed";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (customer.User ==
                    null)
                    customer.User = new UserService().GetUser(customer.UserId);

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                CardService cardService = new CardService();

                if (cardRequest.CardType == null)
                    cardRequest.CardType = cardService.GetCardType(cardRequest.CardTypeId);

                if (cardRequest.CardRequestForm == null)
                    cardRequest.CardRequestForm = cardService.GetCardRequestForm(cardRequest.CardRequestFormId);

                string body = localizedMessageTemplate.Body
                    .Replace("{[CardType]}", cardRequest.CardType.Name)
                    .Replace("{[CustomerName]}", customer.User.FullName)
                    .Replace("{[RequestDate]}", cardRequest.CardRequestForm.CreatedOn.ToString("MM:dd:yyyy HH:mm"))
                    .Replace("{[Reason]}", reason);

                string subject = localizedMessageTemplate.Subject;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        #endregion Send Card Request Message

        #region Send Payment Message

        public int SendCustomerPaymentConfirmationFailedMessage(Customer customer, Payment payment,
            string reason, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.Id <= 0)
                throw new ArgumentException("Customer Id is invalid");

            if (payment == null)
                throw new ArgumentNullException("Payment is required");

            if (payment.Id <= 0)
                throw new ArgumentException("Payment Id is invalid");

            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException("Reason is required");

            try
            {
                string templateName = "Customer.PaymentConfirmationFailed";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (customer.User == null)
                    customer.User = new UserService().GetUser(customer.UserId);

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                Currency systemCurrency = settingService.SystemCurrency;
                Currency customerCurrency = settingService.CustomerDefaultCurrency;
                CardService cardService = new CardService();

                string body = localizedMessageTemplate.Body
                    .Replace("{[CustomerName]}", customer.User.FullName)
                    .Replace("{[PaymentMethod]}", ((PaymentMethod)payment.PaymentMethodId).ToString())
                    .Replace("{[CustomerCurrency]}", customerCurrency.Code)
                    .Replace("{[Amount]}", payment.Amount.ToString())
                    .Replace("{[TransactionAccountNo]}", string.IsNullOrEmpty(payment.TransactionAccountNo)
                    ? "N/A" : payment.TransactionAccountNo)
                    .Replace("{[TransactionName]}", string.IsNullOrEmpty(payment.TransactionName) ? "N/A" : payment.TransactionName)
                    .Replace("{[TransactionNo]}", string.IsNullOrEmpty(payment.TransactionNo) ? "N/A" : payment.TransactionNo)
                    .Replace("{[PaymentDate]}", payment.CreatedOn.ToString("yyyy:MM:dd HH:mm"))
                    .Replace("{[Reasons]}", reason);

                string subject = localizedMessageTemplate.Subject;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        #endregion Send Payment Message



        #region
        public int SendSecurityQuestionMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("User");

            if (user.Id <= 0)
                throw new ArgumentException("User Id is invalid");

            try
            {
                string templateName = "Customer.ChangeSecurityQuestion";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string subject = localizedMessageTemplate.Subject;
                string body = localizedMessageTemplate.Body;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = user.Email;

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, user.FullName, string.Empty, string.Empty
                    , subject, body, DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, user.FullName,
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }
        public int SendExceptionMessage(Exception exception)
        {
            int langaugeId = new CommonService().GetLanguages().Where(l => l.IsSystemLanguage)
                                            .Select(c => c.Id).SingleOrDefault();
            try
            {
                string templateName = "Exception.Email";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, langaugeId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string subject = localizedMessageTemplate.Subject;
                string body = localizedMessageTemplate.Body;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = new SettingService().ExcpetionEmailAddress;

                body = body.Replace("{[Exception]}", exception.ToString());

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, "Haider", string.Empty, string.Empty
                    , subject, body, DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, "",
                    string.Empty, string.Empty, subject, body);

                return queuedEmailId;
            }
            catch (Exception e)
            {
                new MessageService().SendExceptionMessage(e);
                log.Error(e);
                return -1;
            }
        }

        public int SendTestEmailMessage(string emailAccount, string emailAddressTo)
        {
            if (string.IsNullOrEmpty(emailAccount))
                throw new UserException("Email account is Required");

            if (string.IsNullOrEmpty(emailAddressTo))
                throw new UserException("Email Address is Required");

            int langaugeId = new CommonService().GetLanguages().Where(l => l.IsSystemLanguage)
                                                .Select(c => c.Id).SingleOrDefault();

            try
            {
                string templateName = "CnC.TestEmail";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, langaugeId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                localizedMessageTemplate.EmailAccount =
                    GetEmailAccount(emailAccountId: Convert.ToInt32(emailAccount));

                string subject = localizedMessageTemplate.Subject;
                string body = localizedMessageTemplate.Body;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = emailAddressTo;

                body = body.Replace("{[TestEmail]}", "This is Test Email");

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, "",
                    string.Empty, string.Empty, subject, body);

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, "Test", string.Empty, string.Empty
                    , subject, body, DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        #endregion

        #region Send Customer KycForms As attachment

        public int SendCustomerKycForms(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("User");

            string engFileName = customer.NIC + "-KycFormInEnglish" + ".Jpeg";
            string perFileName = customer.NIC + "-KycFormInPersian" + ".Jpeg";

            string kycFormEnglish = Path.Combine(new DocumentService().GetDocumentDirectoryPath(customer.NIC)
                                                                 , engFileName);
            string kycFormPersian = Path.Combine(new DocumentService().GetDocumentDirectoryPath(customer.NIC)
                                                                 , perFileName);
            if (!File.Exists(kycFormEnglish) && !File.Exists(kycFormPersian))
                throw new ArgumentNullException("Email cannot be sent.");

            try
            {
                string templateName = "Customer.SendKycForms";
                var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, customer.LanguageId);
                if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                    return -1;

                if (localizedMessageTemplate.EmailAccount == null)
                    localizedMessageTemplate.EmailAccount = GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                string subject = localizedMessageTemplate.Subject;
                string body = localizedMessageTemplate.Body;
                string from = localizedMessageTemplate.EmailAccount.Email;
                string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                string to = customer.User.Email;

                body = localizedMessageTemplate.Body
                   .Replace("{[CustomerName]}", customer.User.FullName);

                int queuedEmailId = AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty, subject, body,
                    DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);

                SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                    localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                    localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                    string.Empty, string.Empty, subject, body, attachments: kycFormEnglish + "," + kycFormPersian);
                return queuedEmailId;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        #endregion

        #region Replace Token

        private string ReplaceMessageTemplateTokens(Dictionary<string, string> tokenValues)
        {
            return null;
        }

        #endregion

        #endregion
    }
}

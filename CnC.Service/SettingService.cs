using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Caching;
using CnC.Core.Common;
using CnC.Core.Customers;
using CnC.Core.Exceptions;
using CnC.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CnC.Service
{
    public class SettingService
    {
        //Setting service
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        #region Url

        public string AppUrl
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("CnC.WebApp.Url"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "CnC.WebApp.Url");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                            return null;
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("CnC.WebApp.Url");
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return null;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public string AdminLoginUrl
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("CnC.WebApp.Url"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "CnC.WebApp.Url");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                            return null;
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("CnC.WebApp.Url");

                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return null;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public string CustomerLoginUrl
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("CnC.WebApp.Url"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "CnC.WebApp.Url");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                            return null;
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("CnC.WebApp.Url");
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return null;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public string CustomerVerificationUrl
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("CnC.CustomerVerification.Url"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "CnC.CustomerVerification.Url");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                            return null;

                        }
                    }
                    else
                        return new CachingProvider().Get<string>("CnC.CustomerVerification.Url");

                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return null;
                }
            }
            set
            {
                // Save in DB
            }
        }

        #endregion Url

        #region System

        public Language SystemLanguage
        {
            get
            {
                try
                {
                    using (var context = new EntityContext())
                    {
                        return context.Languages.SingleOrDefault(c => c.IsSystemLanguage == true);
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

        public Currency SystemCurrency
        {
            get
            {
                try
                {
                    using (var context = new EntityContext())
                    {
                        return context.Currencies.SingleOrDefault(c => c.IsSystemCurrency == true);
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

        #endregion System

        #region Customer

        public Language CustomerDefaultLanguage
        {
            get
            {
                try
                {
                    using (var context = new EntityContext())
                    {
                        var defaultLanguage = context.CnCSettings
                            .SingleOrDefault(s => s.Name == "Customer.DefaultLanguage").Value;

                        return context.Languages.SingleOrDefault(l => l.Name == defaultLanguage);
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

        public bool IsEmailLocalizationEnabled
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Email.Localization.Enabled"))
                    {
                        using (var context = new EntityContext())
                        {
                            var emailLoacalizationEnabled = context.CnCSettings.
                                SingleOrDefault(s => s.Name == "Email.Localization.Enabled");

                            if (emailLoacalizationEnabled != null)
                            {
                                new CachingProvider()
                                .Set(emailLoacalizationEnabled.Name, emailLoacalizationEnabled.Value);
                                return Convert.ToBoolean(emailLoacalizationEnabled.Value);
                            }
                            else
                                return false;

                        }
                    }
                    else
                        return Convert.ToBoolean(new CachingProvider().Get<string>("Email.Localization.Enabled"));
                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return false;
                }
            }
        }

        public Currency CustomerDefaultCurrency
        {
            get
            {
                try
                {
                    using (var context = new EntityContext())
                    {
                        var defaultCurrency = context.CnCSettings.SingleOrDefault(s => s.Name == "Customer.DefaultCurrency").Value;
                        return context.Currencies.SingleOrDefault(c => c.Code == defaultCurrency);
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

        public int NINLength
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Customer.NIN.Length"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Customer.NIN.Length");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToInt32(data.Value);
                            }
                            else
                                return 0;
                        }
                    }
                    else
                        return Convert.ToInt32(new CachingProvider().Get<string>("Customer.NIN.Length"));

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return 10;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public int PassportNoLength
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Customer.PassportNo.Length"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Customer.PassportNo.Length");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToInt32(data.Value);
                            }
                            else
                                return 0;
                        }
                    }
                    else
                        return Convert.ToInt32(new CachingProvider().Get<string>("Customer.PassportNo.Length"));

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return 9;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public int CustomerMinimumAge
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Customer.Age.Minimum"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Customer.Age.Minimum");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToInt32(data.Value);
                            }
                            else
                                return 0;
                        }
                    }
                    else
                        return Convert.ToInt32(new CachingProvider().Get<string>("Customer.Age.Minimum"));
                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return 18;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public int CustomerAddressMinimumLength
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Customer.Address.MinimumLength"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Customer.Address.MinimumLength");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToInt32(data.Value);
                            }
                            else
                                return 0;
                        }
                    }
                    else
                        return Convert.ToInt32(new CachingProvider().Get<string>("Customer.Address.MinimumLength"));

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return 6;
                }
            }
            set
            {
                // Save in DB
            }
        }

        #endregion Customer

        #region Localization

        public bool LocalizationEnable
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Settings.Localization.Enable"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Settings.Localization.Enable");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToBoolean(data.Value);
                            }
                            else
                                return false;
                        }
                    }
                    else
                        return Convert.ToBoolean(new CachingProvider().Get<string>("Settings.Localization.Enable"));

                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return true;
            }
            set
            {
                // Save in DB
            }
        }

        #endregion

        public int ResultPageSize
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Result.PageSize"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Result.PageSize");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToInt32(data.Value);
                            }
                            return 0;
                        }
                    }
                    else
                        return Convert.ToInt32(new CachingProvider().Get<string>("Result.PageSize"));

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return 10;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public int AdminPageSize
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Result.AdminPageSize"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Result.AdminPageSize");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToInt32(data.Value);
                            }
                            return 0;
                        }
                    }
                    else
                        return Convert.ToInt32(new CachingProvider().Get<string>("Result.AdminPageSize"));

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return 10;
                }
            }
            set
            {
                // Save in DB
            }
        }

        /// <summary>
        /// Exchange Rate Service Charges Percentage
        /// </summary>
        public decimal ExchangeRateServiceCharges
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("ServiceFee.ExchangeRate"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "ServiceFee.ExchangeRate");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToDecimal(data.Value);
                            }
                            return 0;
                        }
                    }
                    else
                        return Convert.ToDecimal(new CachingProvider().Get<string>("ServiceFee.ExchangeRate"));

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return 3;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public CnCSetting GetLocalizationSetting()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Settings.Localization.Enable");
                    if (data != null)
                    {
                        return data;
                    }
                    else
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

        public bool UpdateLocalizationSetting(CnCSetting cncSetting)
        {
            if (cncSetting == null)
                throw new ArgumentNullException("Localization data is required");

            try
            {
                using (var context = new EntityContext())
                {
                    var localizationSetting = context.CnCSettings
                                                     .SingleOrDefault(c => c.Name == "Settings.Localization.Enable");

                    if (localizationSetting == null)
                        throw new UserException("localization with this Id not found");

                    localizationSetting.Name = cncSetting.Name;
                    localizationSetting.Value = cncSetting.Value;
                    localizationSetting.Description = cncSetting.Description;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");


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

        #region CnCSettings
        public List<CnCSetting> GetCnCSettings(out int totalCount, string name = null, string value = null, string description = null, int pageIndex = 0, int? pageSize = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var settings = (from query in context.CnCSettings
                                    where true == (!string.IsNullOrEmpty(name) ? query.Name.ToLower().Contains(name.ToLower()) : true)
                                  && true == (!string.IsNullOrEmpty(value) ? query.Value.ToLower().Contains(value.ToLower()) : true)
                                  && true == (!string.IsNullOrEmpty(description) ? query.Description.ToLower().Contains(description.ToLower()) : true)
                                    select query);
                    totalCount = settings.Count();
                    if (settings != null && settings.Count() > 0)
                    {

                        return settings.OrderBy(c => c.Name).Skip(skipRows).Take(pageSize.Value).ToList();
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
        public CnCSetting GetCnCSetting(string id)
        {
            if (id == null)
                throw new ArgumentNullException("Setting Id is required");

            try
            {
                using (var context = new EntityContext())
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        int settingId = Convert.ToInt32(id);
                        var result = context.CnCSettings.SingleOrDefault(c => c.Id == settingId);
                        if (result != null)
                            return result;

                        return null;
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
            //return 
        }
        public bool UpdateCnCSetting(CnCSetting setting)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (setting != null)
                    {
                        var cncSetting = context.CnCSettings.SingleOrDefault(c => c.Id == setting.Id);
                        cncSetting.Name = setting.Name;
                        cncSetting.Value = setting.Value;
                        cncSetting.Description = setting.Description;

                        if (context.SaveChanges() <= 0)
                            return false;

                        new CachingProvider().Clear();
                        return true;
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
        public bool AddCnCSetting(CnCSetting setting)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (setting != null)
                    {
                        context.CnCSettings.Add(setting);
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
                return true;
            }

        }
        #endregion

        #region LAMDA

        public string LamdaPhpUrl
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Lamda.PhpUrl"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Lamda.PhpUrl");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                            return null;
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("Lamda.PhpUrl");

                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return null;
            }
            set
            {
                // Save in DB
            }
        }

        public string LamdaMerchantId
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Lamda.MerchantId"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Lamda.MerchantId");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                            else
                                return null;
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("Lamda.MerchantId");
                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return null;
            }
            set
            {
                // Save in DB
            }
        }

        public string LamdaTerminalId
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Lamda.TerminalId"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Lamda.TerminalId");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                            else
                                return null;
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("Lamda.TerminalId");
                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return null;
            }
            set
            {
                // Save in DB
            }
        }

        public string LamdaTerminalPassword
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Lamda.TerminalPassword"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Lamda.TerminalPassword");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("Lamda.TerminalPassword");

                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return null;
            }
            set
            {
                // Save in DB
            }
        }

        public bool LamdaTestMode
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Lamda.TestMode"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Lamda.TestMode");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToBoolean(data.Value);
                            }
                        }
                    }
                    else
                        return Convert.ToBoolean(new CachingProvider().Get<string>("Lamda.TestMode"));
                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return true;
            }
            set
            {
                // Save in DB
            }
        }

        public string LamdaPhpUrlTest
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Lamda.PhpUrl.Test"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Lamda.PhpUrl.Test");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("Lamda.PhpUrl.Test");

                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return null;
            }
            set
            {
                // Save in DB
            }
        }

        public string LamdaMerchantIdTest
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Lamda.MerchantId.Test"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Lamda.MerchantId.Test");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("Lamda.MerchantId.Test");
                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return null;
            }
            set
            {
                // Save in DB
            }
        }

        public string LamdaTerminalIdTest
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Lamda.TerminalId.Test"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Lamda.TerminalId.Test");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("Lamda.TerminalId.Test");
                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return null;
            }
            set
            {
                // Save in DB
            }
        }

        public string LamdaTerminalPasswordTest
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Lamda.TerminalPassword.Test"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Lamda.TerminalPassword.Test");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("Lamda.TerminalPassword.Test");
                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return null;
            }
            set
            {
                // Save in DB
            }
        }

        #endregion

        #region
        public bool PaymentReconfirmationRequired
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Payment.ReconfirmationRequired"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.
                                SingleOrDefault(s => s.Name == "Payment.ReconfirmationRequired");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToBoolean(data.Value);
                            }
                        }
                    }
                    else
                        return Convert.ToBoolean(new CachingProvider().Get<string>("Payment.ReconfirmationRequired"));
                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return false;
            }
            set
            {
                // Save in DB
            }
        }

        public bool TopUpApprovalRequired
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Topup.Approval.Required"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.
                                SingleOrDefault(s => s.Name == "Topup.Approval.Required");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToBoolean(data.Value);
                            }
                        }
                    }
                    else
                        return Convert.ToBoolean(new CachingProvider().Get<string>("Topup.Approval.Required"));
                }
                catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
                return false;
            }
            set
            {
                // Save in DB
            }
        }

        public int DefaultUserCount
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Users.DefaultCount"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Users.DefaultCount");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToInt32(data.Value);
                            }
                            else
                                return 0;
                        }
                    }
                    else
                        return Convert.ToInt32(new CachingProvider().Get<string>("Users.DefaultCount"));

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return 9;
                }
            }
            set
            {
                // Save in DB
            }
        }        

        public string ExcpetionEmailAddress
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("CnC.ExceptionEmailAddress"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "CnC.ExceptionEmailAddress");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("CnC.ExceptionEmailAddress");
                }
                catch (Exception exception) { new MessageService().SendExceptionMessage(exception); }
                return null;
            }
            set
            {
                // Save in DB
            }
        }     

        #endregion

        #region WebAPiSecretKey

        public string WebAPISecretKey
        {
            get
            {
                using (var context = new EntityContext())
                {
                    string secretKey = context.CnCSettings.SingleOrDefault(s => s.Name == "CnC.API.SecretKey").Value;
                    if (secretKey != null)
                        return secretKey;
                    else
                        return null;
                }
            }
        }

        #endregion

        #region Proxy Server

        public string ProxyServerSmartAppVersion
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("ProxyServer.SmartApp.Version"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "ProxyServer.SmartApp.Version");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return data.Value;
                            }
                            else
                                return null;
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("ProxyServer.SmartApp.Version");

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return null;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public string ProxyServerSmartAppDownloadUrl
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("ProxyServer.SmartApp.DownloadUrl"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "ProxyServer.SmartApp.DownloadUrl");
                            return data.Value;
                        }
                    }
                    else
                        return new CachingProvider().Get<string>("ProxyServer.SmartApp.DownloadUrl");

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return null;
                }
            }
            set
            {
                // Save in DB
            }
        }

        public int LatestExchangeRatesSize
        {
            get
            {
                try
                {
                    if (!new CachingProvider().IsSet("Cnc.Latest.ExchangeRates"))
                    {
                        using (var context = new EntityContext())
                        {
                            var data = context.CnCSettings.SingleOrDefault(s => s.Name == "Cnc.Latest.ExchangeRates");
                            if (data != null)
                            {
                                new CachingProvider().Set(data.Name, data.Value);
                                return Convert.ToInt32(data.Value);
                            }
                            return 0;
                        }
                    }
                    else
                        return Convert.ToInt32(new CachingProvider().Get<string>("Cnc.Latest.ExchangeRates"));

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return 10;
                }
            }
            set
            {
                // Save in DB
            }
        }

        #endregion

    }


}

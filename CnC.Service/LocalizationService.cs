using CnC.Core.Localized;
using CnC.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using CnC.Core.Caching;

namespace CnC.Service
{
    public class LocalizationService : ILocalizationService
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(LocalizationService));
        private int _languageId;

        public LocalizationService(int? languageId = null)
        {
            if (languageId != null)
            {
                _languageId = Convert.ToInt16(languageId);
            }
            else
            {
                _languageId = new SettingService().SystemLanguage.Id;
            }
        }
        public string GetResource(string key, int? languageId = null, string defaultValue = null)
        {
            if (!new SettingService().LocalizationEnable)
            {
                return (defaultValue == null ? key : defaultValue);
            }
            else
            {
                if (string.IsNullOrEmpty(key))
                    return null;

                if (!languageId.HasValue)
                    languageId = _languageId;

                using (var context = new EntityContext())
                {
                    try
                    {
                        if (!new CachingProvider().IsSet(key + "-" + languageId.ToString()))
                        {
                            var localizedData = (from localizedStringResources in context.LocalizedStringResources
                                                 where localizedStringResources.ResourceName.ToLower().Equals(key.ToLower())
                                                 && localizedStringResources.LanguageId == languageId
                                                 select localizedStringResources).SingleOrDefault();
                            if (localizedData != null)
                            {
                                new CachingProvider().Set(localizedData.ResourceName + "-" + languageId.ToString(), localizedData.ResourceValue);
                                return localizedData.ResourceValue;
                            }
                            else
                                return defaultValue;
                        }
                        else
                            return new CachingProvider().Get<string>(key + "-" + languageId.ToString());
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

        public string GetValidationFile()
        {
            if (!new SettingService().LocalizationEnable)
            {
                return "ValidationLocalization_English.js";
            }

            using (var context = new EntityContext())
            {
                try
                {
                    if (!new CachingProvider().IsSet(_languageId.ToString()))
                    {
                        var localizedResourceFile = (from query in context.Languages
                                                     where query.Id == _languageId
                                                     select query.ValidationFileName).SingleOrDefault();
                        if (localizedResourceFile != null)
                        {
                            new CachingProvider().Set(_languageId.ToString(), localizedResourceFile);
                            return localizedResourceFile;
                        }
                        else
                            return null;
                    }
                    else
                        return new CachingProvider().Get<string>(_languageId.ToString());

                }
                catch (Exception exception)
                {
                    new MessageService().SendExceptionMessage(exception);
                    log.Error(exception);
                    return null;
                }
            }
        }
        public virtual void ClearCache()
        {
            new CachingProvider().Clear();
        }

        public List<LocalizedStringResource> GetAllResourceValues(int languageId)
        {
            throw new NotImplementedException();
        }
    }
}

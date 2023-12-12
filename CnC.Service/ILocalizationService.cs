using CnC.Core.Localized;
using System.Collections.Generic;

namespace CnC.Service
{
    public interface ILocalizationService
    {
        List<LocalizedStringResource> GetAllResourceValues(int languageId);
        string GetResource(string resourceKey, int? languageId, string defaultValue = null);
        void ClearCache();
    }
}

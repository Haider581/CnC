using System;
using CnC.Core.Common;
using System.Collections.Generic;
using System.Linq;
using CnC.Data;
using log4net;

namespace CnC.Service
{
    public class CurrencyService

    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

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

        public Currency GetCurrency(int currencyId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.Currencies.SingleOrDefault(c => c.Id == currencyId);
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public Currency GetCurrencyByCountryId(int countryId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return (from cur in context.Currencies
                            join c in context.Countries on cur.Id equals c.CurrencyId
                            where c.Id == countryId
                            select cur).SingleOrDefault();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<Currency> GetCurrencies()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.Currencies.ToList();
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

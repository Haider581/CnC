using CnC.Core.Common;
using CnC.Core.Exceptions;
using CnC.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnC.Service
{
    public class ExchangeRateService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        public int AddCurrencyRate(CurrencyRate currencyRate)
        {
            if (currencyRate.CurrencyId <= 0)
                throw new UserException("Currency Id is invalid");

            if (currencyRate.Rate <= 0)
                throw new UserException("Currency Rate cannot be less than or equal to zero");

            try
            {
                using (var context = new EntityContext())
                {
                    var currency = context.Currencies.SingleOrDefault(c => c.Id == currencyRate.CurrencyId);

                    if (currency == null)
                        throw new UserException("Currency by given Id does not exist");

                    context.CurrencyRates.Add(currencyRate);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return currencyRate.Id;
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                throw new UserException("Unable to process request");
            }
        }

        /// <summary>
        /// Return current Exchange Rate after applying Service Charges
        /// </summary>        
        public CurrencyRate GetExchangeRate(int currencyId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var currencyRate = (from cr in context.CurrencyRates
                                        join c in context.Currencies on cr.CurrencyId equals c.Id
                                        where c.Id == currencyId
                                        orderby cr.CreatedOn descending
                                        select new { Currency = c, CurrencyRate = cr }).FirstOrDefault();

                    if (currencyRate != null)
                    {
                        currencyRate.CurrencyRate.Currency = currencyRate.Currency;

                        // Apply Exchange Rate Service Charges
                        //  currencyRate.CurrencyRate.Rate +=
                        //      (currencyRate.CurrencyRate.Rate * new SettingService().ExchangeRateServiceCharges / 100);



                        return currencyRate.CurrencyRate;
                    }
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
            }

            return null;
        }

        /// <summary>
        /// Return current and preivous Exchange Rates 
        /// </summary>        
        public List<CurrencyRate> GetExchangeRates(int currencyId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var currencyRates = (from cr in context.CurrencyRates
                                         join c in context.Currencies on cr.CurrencyId equals c.Id
                                         where c.Id == currencyId
                                         orderby cr.CreatedOn descending
                                         select new { Currency = c, CurrencyRate = cr })
                                         .Take(new SettingService().LatestExchangeRatesSize).ToList();

                    if (currencyRates.Count > 0)
                    {
                        foreach (var currencyRate in currencyRates)
                            currencyRate.CurrencyRate.Currency = currencyRate.Currency;

                        return currencyRates.Select(c => c.CurrencyRate).ToList();
                    }
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
            }

            return null;
        }

        /// <summary>
        /// Return current Exchange Rates of every Currency
        /// </summary>        
        public List<CurrencyRate> GetExchangeRates()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var currencyRates = (from cr in context.CurrencyRates
                                         join c in context.Currencies on cr.CurrencyId equals c.Id
                                         orderby cr.CreatedOn descending
                                         group new { Currency = c, CurrencyRate = cr }
                                         by c.Id).ToList();

                    if (currencyRates.Count > 0)
                    {
                        var exchangeRates = new List<CurrencyRate>();

                        foreach (var currencyRate in currencyRates)
                        {
                            var firstRecord = currencyRate.Last();
                            firstRecord.CurrencyRate.Currency = firstRecord.Currency;
                            exchangeRates.Add(firstRecord.CurrencyRate);
                        }

                        return exchangeRates;
                    }
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
            }

            return null;
        }

    }
}

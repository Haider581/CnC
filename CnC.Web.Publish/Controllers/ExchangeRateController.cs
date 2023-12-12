using CnC.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Dev;

namespace CnC.Web.Controllers
{
    [RoleActionValidator]
    public class ExchangeRateController : Controller
    {
        // GET: ExchangeRate
        public ActionResult Index()
        {
            return View();
        }

        public CurrencyRate GetExchangeRate(int currencyId)
        {
            return new CnC.Service.ExchangeRateService().GetExchangeRate(currencyId);
        }
    }
}
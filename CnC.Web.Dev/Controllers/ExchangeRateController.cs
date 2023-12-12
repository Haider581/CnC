using CnC.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Helper;
using CnC.Service;
using CnC.Core.Exceptions;

namespace CnC.Web.Controllers
{
    [Authorize]
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
        public ActionResult ExchangeRates()
        {
            var exchangeRates = new Service.ExchangeRateService().GetExchangeRates();
            return View(exchangeRates);
        }

        [HttpGet]
        public JsonResult UpdateExchangeRate(string currencyId, string currencyRate)
        {
            var statusArray = new string[3];
            
            if (!string.IsNullOrEmpty(currencyId) && !string.IsNullOrEmpty(currencyRate))
            {
                try
                {
                    CurrencyRate currencyRat = new CurrencyRate();
                    currencyRat.CurrencyId = Convert.ToInt32(currencyId);
                    currencyRat.Rate = Convert.ToDecimal(currencyRate);
                    var exchangeRateService = new ExchangeRateService();
                    int response = exchangeRateService.AddCurrencyRate(currencyRat);
                    if (response > 0)
                    {
                        //success
                        statusArray[0] = "200";
                        statusArray[1] = statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Success</strong></span><br />Exchange Rate has been changed successfully.!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/ExchangeRate/ExchangeRates'>Go back</a></span>";
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "<strong>Fail!</strong> An error occured changing the Exchange Rate. Please try again.";
                        // Failure
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
                    statusArray[1] = "Sorry, we are unable to process your request. Valid Input required";
                }
            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Sorry, we are unable to process your request. Missing Required Values.";
            }

            //string messageDesc = "Updated Successfully!";
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetExchangeRateHistory(string currencyId)
        {
            var statusArray = new object[3];
            if (!string.IsNullOrEmpty(currencyId))
            {
                try
                {
                    var exchangeRates = new Service.ExchangeRateService().GetExchangeRates(Convert.ToInt32(currencyId));
                    if (exchangeRates != null && exchangeRates.Count > 0)
                    {
                        statusArray[0] = "200";
                        statusArray[1] = exchangeRates;
                       return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                    }
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                }
                catch (Exception)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                }
            }
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Mising required value.";
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }
    }
}
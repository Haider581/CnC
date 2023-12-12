using CnC.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Dev;

namespace CnC.Web.Controllers
{
    //[RoleActionValidator]
    public class CommonController : Controller
    {
        public string GetCities(int countryId)
        {
            var commonServer = new CnC.Service.CommonService();
            var cities = commonServer.GetCities(countryId);
            return Newtonsoft.Json.JsonConvert.SerializeObject(cities);
            //return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public List<Country> GetCountries()
        {
            var commonService = new CnC.Service.CommonService();
            var countries = commonService.GetCountries();
            return countries;
        }
    }
}
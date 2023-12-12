using CnC.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Helper;
using log4net;
using System.Web.Security;

namespace CnC.Web.Controllers
{
    //[RoleActionValidator]
    public class CommonController : Controller
    {

        [Route("Error")]
        public ActionResult Error()
        {
            return View();
        }

        [Route("404")]
        public ActionResult Error404()
        {
            ILog log = LogManager.GetLogger(typeof(HomeController));

            var url = HttpUtility.HtmlEncode(Request.Url.AbsoluteUri);
            var err = string.Format("Page not found: {0}", url);
            log.Error(err);
            return View();
        }

        [Route("Unauthorize")]
        public ActionResult Unauthorize()
        {
            var userSession = Session["CurrentSession"] as CnC.Core.Accounts.User;
            if (userSession != null)
            {
                FormsAuthentication.SignOut();
                Session.Abandon();

                // clear authentication cookie
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie1);
            }
            return View();
        }

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
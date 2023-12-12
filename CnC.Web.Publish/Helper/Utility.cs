using CnC.Core.Accounts;
using CnC.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CnC.Web.Dev
{
    public class Utility
    {
        //public string GetCities(int countryId)
        //{
        //    var commonServer = new CnC.Service.CommonService();
        //    var cities = commonServer.GetCitiesByCountryId(countryId);
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(cities);
        //    //return Json(cities, JsonRequestBehavior.AllowGet);
        //}

     public string GetCountries()
        {
         //   var sessionKey = HttpContext.Current.Session["CurrentSession"] as User;
            var commonService = new CnC.Service.CommonService();
            var countries = commonService.GetCountries();
            return Newtonsoft.Json.JsonConvert.SerializeObject(countries);
        }

        public static bool HasPermission(string permissionName)
        {
            //CurrentSession.Instance.LoginUser.r
            return false;
        }
        public static bool IsAlphaNumeric(string strToCheck)
        {
            var rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        public static bool IsAlphabet(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, @"^[a-zA-Z]+$");
        }

        public static bool IsAlphabetWithSpace(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, @"^[a-zA-Z ]+$");
        }
        public static bool IsNumeric(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, @"^[0-9]+$");
        }
        public bool IsValidImage(object value)
        {
            var file = value as  HttpPostedFileBase;
            if (file == null) { return false; }
           // var allowedSize=AppSer
                string imageSize = ConfigurationManager.AppSettings["ImageSize"];
            
            if (file.ContentLength > Convert.ToInt32(imageSize) * 1024 * 1024)
            { return false; }

            try
            {
                var allowedFormats = new[]
                {
                ImageFormat.Jpeg,
                ImageFormat.Png
            };

                using (var img = Image.FromStream(file.InputStream))
                {
                    return allowedFormats.Contains(img.RawFormat);
                }
            }
            catch { }
            return false;
        }

        public bool IsValidImageClientSideUploading(object value)
        {
            var file = value as System.Web.HttpPostedFile;
            if (file == null) { return false; }
            // var allowedSize=AppSer
            string imageSize = ConfigurationManager.AppSettings["ImageSize"];

            if (file.ContentLength > Convert.ToInt32(imageSize) * 1024 * 1024)
            { return false; }

            try
            {
                var allowedFormats = new[]
                {
                ImageFormat.Jpeg,
                ImageFormat.Png
            };

                using (var img = Image.FromStream(file.InputStream))
                {
                    return allowedFormats.Contains(img.RawFormat);
                }
            }
            catch { }
            return false;
        }
    }
}
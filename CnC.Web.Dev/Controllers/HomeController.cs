using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Core.Accounts;
using System.Configuration;
using System.IO;
using System.Web.UI.WebControls;
using CnC.Web.Helper;
using log4net;

namespace CnC.Web.Controllers
{
    public class HomeController : Controller
    {
        //[RoleActionValidator]
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account");
            //return View();
        }


        [AllowAnonymous]
        //[HttpPost]
        public ActionResult Test()//string TOKEN
        {
            return Content("<form action='http://localhost:13049/Customer/PaymentGatewayResponse' id='frmTest' method='post'><input type='hidden' name='RESCODE' value='01' /><input type='hidden' name='CRN' value='45645783452' /><input type='hidden' name='TRN' value='6666345345' /></form><script>document.getElementById('frmTest').submit();</script>");
            //return Content("<form action='http://localhost:13049/Customer/MabnaResponseForCardRequestsPayment' id='frmTest' method='post'><input type='hidden' name='RESCODE' value='01' /><input type='hidden' name='CRN' value='45645783452' /><input type='hidden' name='TRN' value='6666345345' /></form><script>document.getElementById('frmTest').submit();</script>");
            //return Content("<form action='http://localhost:13049/TopUp/MabnaResponseForTopUpRequest' id='frmTest' method='post'><input type='hidden' name='RESCODE' value='01' /><input type='hidden' name='CRN' value='45645783452' /><input type='hidden' name='TRN' value='6666345345' /></form><script>document.getElementById('frmTest').submit();</script>");
            //ActionResult action = new CustomerController().NewCustomerCardRequests("00", "23423445553", "5555634534");
            //return action;
        }

        [AllowAnonymous]
        public JsonResult IsValidCall(string callFrom)
        {
            var statusArray = new string[2];
            var userSession = Session["CurrentSession"] as User;
            if (userSession == null)
                statusArray[0] = "0";
            else
                statusArray[0] = "1";
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [RoleActionValidator]
        [HttpGet]
        public ActionResult Downloads()
        {
            return View();
        }

        public FileResult DownloadItem(string ItemName)
        {
            return File(ItemName, System.Net.Mime.MediaTypeNames.Application.Octet, ItemName);
        }

        public List<ListItem> GetFilesToDownload()
        {
            var filesDirectory = ConfigurationManager.AppSettings["DownloadItems"];
            string[] filePaths = Directory.GetFiles(filesDirectory);
            List<ListItem> files = new List<ListItem>();
            foreach (string filePath in filePaths)
            {
                files.Add(new ListItem(Path.GetFileName(filePath), filePath));
            }
            //FileInfo f = new FileInfo(files[0].Value);
            //var length = f.Length/1024;
            return files;
        }
    }
}
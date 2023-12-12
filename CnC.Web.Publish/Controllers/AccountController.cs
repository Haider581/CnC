using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Models;
using CnC.Core.Accounts;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using reCAPTCHA.MVC;
using System.Configuration;

namespace CnC.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //[CaptchaValidator(
        //PrivateKey = "6Ld6fRkUAAAAACWakhQxu64hPV7W4zqMRD2hAuEQ",
        //ErrorMessage = "Invalid input captcha.",
        //RequiredMessage = "The captcha field is required.")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //Validate Google recaptcha here
            int attempt = Convert.ToInt32(Session["time"]);
            var status = true;
            if (attempt >= 2)
            {
                var response = Request["g-recaptcha-response"];
                string secretKey = ConfigurationManager.AppSettings["reCaptchaPrivateKey"];
                string reCaptchaAPIRequestURL = ConfigurationManager.AppSettings["reCaptchaAPIRequestURL"]+ "?secret={0}&response={1}";
                var client = new WebClient();
                var result = client.DownloadString(string.Format(reCaptchaAPIRequestURL, secretKey, response));
                var obj = JObject.Parse(result);
                status = (bool)obj.SelectToken("success");
                ViewBag.Message = status ? "Captcha validation success" : "Captcha Failed! prove you are not robot";

            }          

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userService = new CnC.Service.UserService();
            var userId = 0;
            if (status)
            {
               
                 userId = userService.SignIn(model.Username.ToUpper(), model.Password);
            }

            //Response.Write("Wrong times: 1");
     
            if (userId > 0)
            {
                int time = 1;
                Session["time"] = time;
                var user = userService.GetUser(userId);
                List<CnCAction> actions = userService.GetUserActions(userId);

                if (actions != null && actions.Count > 0)
                {
                    var action = actions.FirstOrDefault(a => a.DefaultAction != null);
                    var defaultAction = action != null ? action.DefaultAction : null;
                    Session["UserActions"] = actions;
                    Session["CurrentSession"] = user;
                    FormsAuthentication.SetAuthCookie(user.FullName, false);
                    var authTicket = new FormsAuthenticationTicket(1, user.FullName, DateTime.Now, DateTime.Now.AddMinutes(20), false, user.LastName);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    HttpContext.Response.Cookies.Add(authCookie);

                    if (defaultAction != null)
                        return RedirectToAction(defaultAction.ActionName, defaultAction.ControllerName);
                    else
                        return RedirectToAction(actions[0].ActionName, actions[0].ControllerName);
                }
                else
                {
                    ModelState.AddModelError("", user.FullName + " has no actions defined.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");

                int time = Convert.ToInt32(Session["time"]);

                Session["time"] = time + 1;
                //Response.Write("Wrong times: " + time.ToString());
                if (time >= 2)
                {
                    // Show the captcha.
                }
                return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            // AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

    }
}
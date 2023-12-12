using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Models;
using CnC.Core.Accounts;
using System.Web.Security;
using System.Configuration;
using CnC.Service;
using BotDetect.Web.Mvc;

namespace CnC.Web.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var userActions = Session["UserActions"] as List<CnCAction>;

            if (userActions != null)
            {
                var action = userActions.FirstOrDefault(a => a.DefaultAction != null);
                var defaultAction = action != null ? action.DefaultAction : null;

                if (defaultAction != null)
                    return RedirectToAction(defaultAction.ActionName, defaultAction.ControllerName);
                else
                    return RedirectToAction(userActions[0].ActionName, userActions[0].ControllerName);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaValidation("CaptchaCode", "CaptchaLogin", "Incorrect CAPTCHA code!")]
        //[CaptchaValidator(
        //PrivateKey = "6Ld6fRkUAAAAACWakhQxu64hPV7W4zqMRD2hAuEQ",
        //ErrorMessage = "Invalid input captcha.",
        //RequiredMessage = "The captcha field is required.")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            string isCaptchaValidationActive = ConfigurationManager.AppSettings["IsCaptchaValidationActive"];
            if (isCaptchaValidationActive == "1")
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.capchaMessage = "Incorrect CAPTCHA code!";
                    return View(model);
                }
            }

            var userService = new UserService();
            var customerService = new CustomerService();
            var userId = 0;
            try
            {
                userId = userService.SignIn(model.Username, model.Password);
                if (userId > 0)
                {
                    int time = 1;
                    Session["time"] = time;
                    var user = userService.GetUser(userId);
                    List<CnCAction> actions = userService.GetUserActions(userId);
                    var customer = customerService.GetCustomer(userId);

                    if (actions != null && actions.Count > 0)
                    {
                        var action = actions.FirstOrDefault(a => a.DefaultAction != null);
                        var defaultAction = action != null ? action.DefaultAction : null;
                        user.Role = userService.GetUserRole(user.Id);
                        Session["UserActions"] = actions;
                        Session["CurrentSession"] = user;
                        Session["CurrentCustomer"] = customer;

                        FormsAuthentication.SetAuthCookie(user.FullName, false);
                        var authTicket = new FormsAuthenticationTicket(1, user.FullName, DateTime.Now
                            , DateTime.Now.AddMinutes(20), false, user.LastName);
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
                    return View(model);
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("", exception.Message);
                return View(model);
            }
        }

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
using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Affiliates;
using CnC.Core.Audit;
using CnC.Core.Caching;
using CnC.Core.Cards;
using CnC.Core.Customers;
using CnC.Core.Discounts;
using CnC.Core.Exceptions;
using CnC.Core.Messages;
using CnC.Service;
using CnC.Web.Helper;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CnC.Web.Controllers
{
    [Authorize]
    [RoleActionValidator]
    public class AdminController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(AdminController));
        public ActionResult Actions(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var actions = new AdminService().GetAllActions(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(actions);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult Actions(CnCAction cncAction, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);

            if (cncAction != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var actions = new AdminService().GetAllActions(out totalCount, actionName: cncAction.ActionName, controllerName: cncAction.ControllerName,
                        displayName: cncAction.DisplayName, createdDateFrom: cncAction.CreatedDateFrom, createdDateTo: cncAction.CreatedDateTo, pageIndex: page, pageSize: pageSize);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(actions);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditAction(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var cncAction = new AdminService().GetCncAction(id);
                    return View(cncAction);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            else { return RedirectToAction("Actions"); }
        }

        [HttpPost]
        public ActionResult EditAction(CnCAction cncAction, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());
            if (ModelState.IsValid)
            {
                if (cncAction != null)
                {
                    try
                    {
                        bool isSaved = new AdminService().UpdateCncAction(cncAction);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            return View(cncAction);
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View(cncAction);
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(cncAction);
        }

        [HttpGet]
        public ActionResult AddNewCncAction()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AddNewCncAction(CnCAction cncAction, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            cncAction.CreatedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    int isSaved = new AdminService().AddCncAction(cncAction);
                    if (isSaved > 0)
                    {
                        ViewBag.MessageSuccess = "Record Added Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Saved,Please try again";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(cncAction);
        }

        [HttpGet]
        public ActionResult Roles(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var model = new AdminService().GetAllRoles(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(model);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]

        public ActionResult Roles(Role role, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            if (role != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var model = new AdminService().GetAllRoles(out totalCount, name: role.Name, createdDateFrom: role.CreatedDateFrom,
                        createdDateTo: role.CreatedDateTo, pageIndex: page, pageSize: pageSize);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(model);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpGet]

        public ActionResult AddNewRole()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AddNewRole(Role role, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());
            role.CreatedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    int isSaved = new AdminService().AddRole(role);
                    if (isSaved > 0)
                    {
                        ViewBag.MessageSuccess = "Record Added Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Saved,Please try again";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(role);
        }

        [HttpGet]

        public ActionResult EditRole(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var role = new UserService().GetRole(Convert.ToInt32(id));
                    return View(role);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            else { return RedirectToAction("Roles"); }
            //return View();
        }

        [HttpPost]

        public ActionResult EditRole(Role role, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());
            if (ModelState.IsValid)
            {
                if (role != null)
                {
                    try
                    {
                        bool isSaved = new AdminService().UpdateRole(role);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            //ModelState.Clear();
                            return View();
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View();
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(role);
        }

        [HttpGet]
        public ActionResult RoleActions(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var model = new AdminService().GetAllRoleActions(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(model);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult RoleActions(RoleActionsRole roleActionRole, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            if (roleActionRole != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    if (roleActionRole.RoleAction != null)
                    {
                        var model = new AdminService().GetAllRoleActions(out totalCount, roleId: roleActionRole.RoleAction.RoleId,
                            actionId: roleActionRole.RoleAction.ActionId, pageIndex: page, pageSize: pageSize);
                        this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                        this.ViewBag.Page = page;
                        return View(model);
                    }
                    else
                    {
                        var model = new AdminService().GetAllRoleActions(out totalCount, pageIndex: page, pageSize: pageSize);
                        this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                        this.ViewBag.Page = page;
                        return View(model);
                    }
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpGet]

        public ActionResult EditRoleActions(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var ids = id.Split('-');
                    int roleId = Convert.ToInt32(ids[0]);
                    int actionId = Convert.ToInt32(ids[1]);
                    var roleAction = new AdminService().GetRoleActionToUpdate(actionId: actionId, roleId: roleId);
                    return View(roleAction);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpPost]

        public ActionResult EditRoleActions(RoleActionsRole roleActionRole, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());
            if (ModelState.IsValid)
            {
                if (roleActionRole != null)
                {
                    try
                    {
                        bool isSaved = new AdminService().UpdateRoleAction(roleActionRole);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            return View(roleActionRole);
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View(roleActionRole);
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(roleActionRole);
        }

        [HttpGet]

        public ActionResult CreateNewRoleAction()
        {
            return View();
        }

        [HttpPost]

        public ActionResult CreateNewRoleAction(RoleAction roleAction, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());
            try
            {
                int isSaved = new AdminService().AddRoleAction(roleAction);
                if (ModelState.IsValid)
                {
                    if (isSaved > 0)
                    {
                        ViewBag.MessageSuccess = "Record saved Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Saved,Please try Again";
                    return View();
                }
                return View(roleAction);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]

        public ActionResult ChangePassword(User user, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (user != null)
            {
                try
                {
                    bool isSaved = new CustomerService().ChangePassword(user.NewPassword, user.Email);
                    if (isSaved)
                    {
                        ViewBag.MessageSuccess = "Password Changed Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Email not found";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        #region Settings
        [HttpGet]

        public ActionResult Settings(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var settings = new SettingService().GetCnCSettings(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(settings);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult Settings(CnCSetting setting, FormCollection formCollection)
        {
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);

            if (setting != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var settings = new SettingService().GetCnCSettings(out totalCount, name: setting.Name, value: setting.Value, description: setting.Description, pageIndex: page, pageSize: pageSize);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(settings);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();

        }

        [HttpGet]
        public ActionResult EditSetting(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var setting = new SettingService().GetCnCSetting(id);
                    return View(setting);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditSetting(CnCSetting setting, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                if (setting != null)
                {
                    try
                    {
                        bool isSaved = new SettingService().UpdateCnCSetting(setting);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            return View();
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View();
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(setting);
        }

        [HttpGet]
        public ActionResult AddSetting()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddSetting(CnCSetting setting, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    bool isSaved = new SettingService().AddCnCSetting(setting);
                    if (isSaved)
                    {
                        ViewBag.MessageSuccess = "Record Added Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Saved,Please try again";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(setting);
        }
        #endregion

        #region CardTypes
        [HttpGet]

        public ActionResult CardTypes(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var cardTypes = new CardService().GetAllCardTypes(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cardTypes);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult CardTypes(CardType cardType, FormCollection formCollection)
        {
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);

            if (cardType != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var cardTypes = new CardService().GetAllCardTypes(out totalCount, name: cardType.Name, pageIndex: page, pageSize: pageSize);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(cardTypes);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();

        }

        [HttpGet]
        public ActionResult EditCardType(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var cardType = new CardService().GetCardType(Convert.ToInt32(id));
                    return View(cardType);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }
        [HttpPost]

        public ActionResult EditCardType(CardType cardType, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                if (cardType != null)
                {
                    try
                    {
                        bool isSaved = new CardService().UpdateCardType(cardType);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            //ModelState.Clear();
                            return View();
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View();
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(cardType);
        }
        [HttpGet]

        public ActionResult AddCardType()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AddCardType(CardType cardType, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                if (cardType != null)
                {
                    try
                    {
                        bool isSaved = new CardService().AddCardType(cardType);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Added Successfully";
                            ModelState.Clear();
                            return View();
                        }
                        ViewBag.MessageFailure = "Record not Saved,Please try again";
                        return View();
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(cardType);
        }
        #endregion

        #region Email Accounts

        [HttpGet]
        public ActionResult EmailAccounts(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var emailAccounts = new MessageService().GetEmailAccounts(out totalCount, pageIndex: page, pageSize: pageSize);
                if (emailAccounts != null && emailAccounts.Count() > 0)
                {
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(emailAccounts);
                }
                return View();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult EmailAccounts(EmailAccount emailAccounts, FormCollection formCollection)
        {
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);

            if (emailAccounts != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var result = new MessageService().GetEmailAccounts(out totalCount, email: emailAccounts.Email
                        , displayName: emailAccounts.DisplayName, pageIndex: page, pageSize: pageSize);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(result);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditEmailAccount(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var emailAccount = new MessageService().GetEmailAccount(Convert.ToInt32(id));
                    return View(emailAccount);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            else { return RedirectToAction("EmailAccounts"); }
            //return View();
        }

        [HttpPost]
        public ActionResult EditEmailAccount(EmailAccount emailAccount, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                if (emailAccount != null)
                {
                    try
                    {
                        bool isSaved = new MessageService().UpdateEmailAccount(emailAccount);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            var result = new MessageService().GetEmailAccount(Convert.ToInt32(emailAccount.Id));
                            return View(result);
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View();
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(emailAccount);
        }

        [HttpGet]
        public ActionResult AddEmailAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEmailAccount(EmailAccount emailAccount, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    bool isSaved = new MessageService().AddEmailAccount(emailAccount);
                    if (isSaved)
                    {
                        ViewBag.MessageSuccess = "Record Added Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Saved,Please try again";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(emailAccount);
        }

        public JsonResult SendEmailFromEmailAccount(string emailAccountId, string email, string subject, string body)
        {
            string[] statusArray = new string[3];

            if (!string.IsNullOrEmpty(emailAccountId))
            {
                try
                {
                    var emailAccount = new MessageService().GetEmailAccount(Convert.ToInt32(emailAccountId));
                    if (emailAccount != null)
                    {
                        bool isSend = new MessageService().SendEmail(emailAccount.Host, emailAccount.Port
                                , emailAccount.Username, emailAccount.Password, emailAccount.EnableSSL
                                , emailAccount.Email, emailAccount.DisplayName, email, "", "", "", subject, body, null);
                        if (isSend)
                        {
                            statusArray[0] = "100";
                            statusArray[1] = "An Email has been send successfully";

                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }

                        statusArray[0] = "101";
                        statusArray[1] = "Email Cannot be Sent right now";
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
                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                }

            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Queued Email

        [HttpGet]
        public ActionResult QueuedEmails(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var queuedEmails = new MessageService().GetQueuedEmails(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(queuedEmails);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult QueuedEmails(QueuedEmail queuedEmail, FormCollection formCollection)
        {
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);

            if (queuedEmail != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var queuedEmails = new MessageService().GetQueuedEmails(out totalCount, subject: queuedEmail.Subject, fromSender: queuedEmail.From, to: queuedEmail.To, pageIndex: page, pageSize: pageSize);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(queuedEmails);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }
        [HttpGet]

        public ActionResult EditQueuedEmail(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var queuedEmail = new MessageService().GetQueuedEmail(Convert.ToInt32(id));
                    return View(queuedEmail);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            else { return RedirectToAction("QueuedEmails"); }

            //return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditQueuedEmail(QueuedEmail queuedEmail, FormCollection formCollection)
        {
            if (!String.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                if (queuedEmail != null)
                {
                    try
                    {
                        bool isSaved = new MessageService().UpdateQueuedEmail(queuedEmail);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            //ModelState.Clear();
                            return View();
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View();
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }

            }
            return View(queuedEmail);
        }
        [HttpGet]

        public ActionResult AddQueuedEmail()
        {
            return View();
        }
        [HttpPost]

        public ActionResult AddQueuedEmail(QueuedEmail queuedEmail, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    bool isSaved = new MessageService().AddQueuedEmail(queuedEmail);
                    if (isSaved)
                    {
                        ViewBag.MessageSuccess = "Record Added Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Saved,Please try again";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(queuedEmail);
        }
        #endregion

        #region Affiliate
        [HttpGet]

        public ActionResult Affiliates(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var affiliates = new AffiliateService().GetAffiliates(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(affiliates);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult Affiliates(Affiliate affiliate, FormCollection formCollection)
        {
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);

            if (affiliate != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var affiliates = new AffiliateService().GetAffiliates(out totalCount, pageIndex: page
                        , firstName: affiliate.Address.FirstName, lastName: affiliate.Address.LastName
                        , email: affiliate.Address.Email, pageSize: pageSize, showInActive: affiliate.Active);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(affiliates);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditAffiliate(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var affiliate = new AffiliateService().GetAffiliate(Convert.ToInt32(id));
                    return View(affiliate);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditAffiliate(Affiliate affiliate, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                if (affiliate != null)
                {
                    try
                    {
                        bool isSaved = new AffiliateService().UpdateAffiliate(affiliate);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            ModelState.Clear();
                            return RedirectToAction("Affiliates");
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View(affiliate);
                    }
                    catch (Exception exception)
                    {
                        ViewBag.MessageFailure = exception.Message;
                        log.Error(exception);
                        return View(affiliate);
                    }
                }
            }
            return View(affiliate);
        }
        public ActionResult AddAddress()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddAffiliate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAffiliate(Affiliate affiliate, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            affiliate.Active = true;

            if (ModelState.IsValid)
            {
                try
                {
                    int affiliateId = new AffiliateService().AddAffiliate(affiliate);
                    if (affiliateId > 0)
                    {
                        ViewBag.MessageSuccess = "Record Added Successfully";
                        return View();
                    }

                    ViewBag.MessageFailure = "Record not Saved,Please try again";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    ViewBag.MessageFailure = exception;
                    return View();
                }
            }
            return View(affiliate);
        }
        #endregion

        [HttpGet]
        public ActionResult CustomerDocuments(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var customers = new CustomerService().GetCustomers(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                if (customers != null && customers.Count() > 0)
                {
                    return View(customers);
                }
                return View();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult CustomerDocuments(Customer customer, FormCollection formCollection)
        {
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            try
            {
                int pageSize = new SettingService().AdminPageSize;
                var customers = new CustomerService().GetCustomers(out totalCount, nic: customer.NIC, passportNo: customer.PassportNo,
                    firstName: customer.User.FirstName, lastName: customer.User.LastName, emailAddress: customer.User.Email,
                    createdDateFrom: customer.User.CreatedDateFrom, createdDateTo: customer.User.CreatedDateTo, pageIndex: page,
                    pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                if (customers != null && customers.Count() > 0)
                {
                    return View(customers);
                }
                return View();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult SendEmail()
        {
            return View();
        }

        public JsonResult GetTemplate(string email, string templateName)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var customer = new CustomerService().GetCustomerByEmail(email);
                if (customer != null)
                {
                    var templates = new MessageService().GetLocalizedMessageTemplate(templateName, customer.LanguageId);
                    if (templates != null)
                    {
                        templates.Body = templates.Body.Replace("{[CustomerName]}", customer.User.FullName)
                    .Replace("{[Url]}", "")
                    .Replace("{[UserName]}", customer.User.Email)
                    .Replace("{[Password]}", "")
                    .Replace("{[Reasons]}", "")
                    .Replace("{[CardNumber]}", "")
                  .Replace("{[Amount]}", "")
                  .Replace("{[SystemCurrency]}", "")
                .Replace("{[AmountInSystemCurrency]}", "")
                .Replace("{[ExchangeRate]}", "")
                .Replace("{[CustomerCurrency]}", "")
                .Replace("{[AmountInCustomerCurrency]}", "")
                .Replace("{[RequestDate]}", "")
                .Replace("{[RequestDetail]}", "")
                .Replace("{[CardType]}", "")
                .Replace("{[PaymentMethod]}", "")
                .Replace("{[TransactionAccountNo]}", "")
                    .Replace("{[TransactionName]}", "")
                    .Replace("{[TransactionNo]}", "")
                    .Replace("{[PaymentDate]}", "");
                        return Json(templates, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendEmailMessage(FormCollection formCollection)
        {
            string emailAddress = string.Empty;
            string templateName = string.Empty;
            string subject = string.Empty;
            string body = string.Empty;

            if (!string.IsNullOrEmpty(formCollection["Email Address"]))
                emailAddress = Request.Form["Email Address"];
            if (!string.IsNullOrEmpty(formCollection["templates"]))
                templateName = Request.Form["templates"];
            if (!string.IsNullOrEmpty(formCollection["Subject"]))
                subject = Request.Form["Subject"];
            if (!string.IsNullOrEmpty(formCollection["Body"]))
                body = Request.Form["Body"];

            try
            {
                var customer = new CustomerService().GetCustomerByEmail(emailAddress);
                if (customer != null)
                {
                    var localizedMessageTemplate = new MessageService().GetLocalizedMessageTemplate(templateName, customer.LanguageId);
                    if (localizedMessageTemplate != null)
                    {
                        if (localizedMessageTemplate.EmailAccount == null)
                            localizedMessageTemplate.EmailAccount = new MessageService().GetEmailAccount(localizedMessageTemplate.EmailAccountId);

                        string from = localizedMessageTemplate.EmailAccount.Email;
                        string fromName = localizedMessageTemplate.EmailAccount.DisplayName;
                        string to = customer.User.Email;
                        new MessageService().AddQueuedEmail(5, from, fromName, to, customer.User.FullName, string.Empty, string.Empty
                         , subject, body, DateTime.UtcNow, 0, null, localizedMessageTemplate.EmailAccount.Id);
                        bool isSend = new MessageService().SendEmail(localizedMessageTemplate.EmailAccount.Host, localizedMessageTemplate.EmailAccount.Port,
                            localizedMessageTemplate.EmailAccount.Username, localizedMessageTemplate.EmailAccount.Password,
                            localizedMessageTemplate.EmailAccount.EnableSSL, from, fromName, to, customer.User.FullName,
                            string.Empty, string.Empty, subject, body);
                        if (isSend)
                        {
                            ViewBag.MessageSuccess = "Email Send Successfully";
                            return RedirectToAction("SendEmail");
                        }
                        ViewBag.MessageFailure = "Email not Send! please try again";
                        return RedirectToAction("SendEmail");
                    }
                }
                ViewBag.MessageFailure = "Email not Send! please try again";
                return RedirectToAction("SendEmail");
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult TestEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TestEmail(FormCollection formCollection)
        {
            string emailAddressTo = string.Empty;
            string emailAccount = string.Empty;

            if (!string.IsNullOrEmpty(formCollection["emailTo"]))
                emailAddressTo = Request["emailTo"];

            if (!string.IsNullOrEmpty(formCollection["emailAccounts"]))
                emailAccount = Request["emailAccounts"];

            try
            {
                int result = new MessageService().SendTestEmailMessage(emailAccount: emailAccount
                                                 , emailAddressTo: emailAddressTo);
                if (result > 0)
                {
                    ViewBag.MessageSuccess = "Mail sent Successfully";
                    return View();
                }
                ViewBag.MessageFailure = "Mail not sent ! Please try again later";
                return View();

            }
            catch (Exception exception)
            {
                ViewBag.MessageFailure = exception.Message;
                log.Error(exception);
                return View();
            }
        }

        [HttpGet]
        public ActionResult LocalizedMessageTemplates(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var localizedMessages = new MessageService()
                    .GetLocalizedMessageTemplates(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(localizedMessages);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult LocalizedMessageTemplates(LocalizedMessageTemplate localizedMessage, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            try
            {
                int pageSize = new SettingService().AdminPageSize;
                if (localizedMessage != null)
                {
                    if (localizedMessage.MessageTemplate != null)
                    {
                        var localizedMessages = new MessageService()
                                            .GetLocalizedMessageTemplates(out totalCount, templateName: localizedMessage.MessageTemplate.Name,
                                            subject: localizedMessage.Subject, languageId: localizedMessage.LanguageId, pageIndex: page, pageSize: pageSize);
                        this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                        this.ViewBag.Page = page;
                        return View(localizedMessages);
                    }
                    else
                    {
                        var localizedMessages = new MessageService()
                                            .GetLocalizedMessageTemplates(out totalCount, subject: localizedMessage.Subject, languageId: localizedMessage.LanguageId
                                            , pageIndex: page, pageSize: pageSize);
                        this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                        this.ViewBag.Page = page;
                        return View(localizedMessages);
                    }

                }
                return View();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult EditLocalizedMessageTemplates(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var localizedMessage = new MessageService().GetLocalizedMessageTemplate(Convert.ToInt32(id));
                    return View(localizedMessage);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]

        public ActionResult EditLocalizedMessageTemplates(LocalizedMessageTemplate locaLizedmessage)
        {
            if (ModelState.IsValid)
            {
                if (locaLizedmessage != null)
                {
                    try
                    {
                        bool isSaved = new MessageService().UpdateLoclizedMessageTemplate(locaLizedmessage);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            //ModelState.Clear();
                            return View();
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View();
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(locaLizedmessage);
        }

        [HttpGet]

        public ActionResult AddLocalizedMessageTemplates()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AddLocalizedMessageTemplates(LocalizedMessageTemplate localizedMessageTempate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int result = new MessageService().AddLocalizedMessageTemplate(localizedMessageTempate);
                    if (result > 0)
                    {
                        ViewBag.MessageSuccess = "Record Added Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Saved,Please try again";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(localizedMessageTempate);
        }

        [HttpGet]

        public ActionResult PaymentGateway()
        {
            try
            {
                var paymentGateways = new PaymentGatewayInfoService().GetPaymentGateways();
                return View(paymentGateways);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult AddPaymentGateway()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPaymentGateway(PaymentGatewayInfo paymentGateway)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int result = new PaymentGatewayInfoService().AddPaymentGateway(paymentGateway);
                    if (result > 0)
                    {
                        ViewBag.MessageSuccess = "Record Added Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Saved,Please try again";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(paymentGateway);
        }

        [HttpGet]
        public ActionResult EditPaymentGateway(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var localizedMessage = new PaymentGatewayInfoService().GetPaymentGateway(id);
                    return View(localizedMessage);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditPaymentGateway(PaymentGatewayInfo paymentGateway)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isSaved = new PaymentGatewayInfoService().UpdatePaymentGateway(paymentGateway);
                    if (isSaved)
                    {
                        ViewBag.MessageSuccess = "Record Updated Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Updated,Please try again";
                    return View();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(paymentGateway);
        }
        public ActionResult LocalizationEnableDissable()
        {
            return View(new SettingService().GetLocalizationSetting());
        }

        [HttpPost]
        public ActionResult LocalizationEnableDissable(CnCSetting cncSetting)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isSaved = new SettingService().UpdateLocalizationSetting(cncSetting);
                    if (isSaved)
                    {
                        new CachingProvider().Clear();
                        ViewBag.MessageSuccess = "Localization Setting is Updated Successfully";
                        return View(cncSetting);
                    }
                    ViewBag.MessageFailure = "Localization Setting is not Updated,Please try again";
                    return View(cncSetting);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(cncSetting);
        }

        public ActionResult AffiliateDiscounts(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var affiliateDiscounts = new AffiliateService()
                    .GetAffiliateDiscounts(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(affiliateDiscounts);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult AffiliateDiscounts(AffiliateDiscount affiliateDiscount, FormCollection formCollection)
        {
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);

            if (affiliateDiscount != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var affiliateDiscounts = new AffiliateService()
                        .GetAffiliateDiscounts(out totalCount, pageIndex: page, pageSize: pageSize
                        , cardTypeId: Convert.ToInt16(affiliateDiscount.CardTypeId));
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;

                    return View(affiliateDiscounts);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();

        }

        [HttpGet]
        public ActionResult EditAffiliateDiscount(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    //id = new Utility().UrlSafeDecrypt(id);
                    var affiliateDiscount = new AffiliateService().GetAffiliateDiscount(id);
                    return View(affiliateDiscount);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            else { return RedirectToAction("Actions"); }
        }

        [HttpPost]
        public ActionResult EditAffiliateDiscount(AffiliateDiscount affiliateDiscount, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());
            if (ModelState.IsValid)
            {
                if (affiliateDiscount != null)
                {
                    try
                    {
                        bool isSaved = new AffiliateService().UpdateAffiliateDiscount(affiliateDiscount);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            return View(affiliateDiscount);
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View(affiliateDiscount);
                    }
                    catch (UserException exception)
                    {
                        log.Error(exception);
                        ViewBag.MessageFailure = exception.Message;
                        return View(affiliateDiscount);
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(affiliateDiscount);
        }

        [HttpGet]
        public ActionResult AddAffiliateDiscount(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var affiliates = new AffiliateService().GetAffiliates(out totalCount, pageIndex: page
                                                , pageSize: pageSize, showInActive: true);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(affiliates);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddAffiliateDiscount(Affiliate affiliate, FormCollection formCollection)
        {
            try
            {
                int page = 0;
                if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                    return Redirect("AddAffiliateDiscount");

                if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                    page = Convert.ToInt32(Request.Form["btnPagination"]);

                if (affiliate != null)
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var affiliates = new AffiliateService()
                        .GetAffiliates(out totalCount, pageIndex: page, pageSize: pageSize
                        , firstName: affiliate.Address.FirstName, lastName: affiliate.Address.LastName
                        , showInActive: affiliate.Active);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(affiliates);
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        public JsonResult AddNewAffiliateDiscounts(List<AffiliateDiscount> affiliateDiscounts)
        {
            string[] statusArray = new string[4];

            try
            {
                int result = new AffiliateService().AddAffiliateDiscount(affiliateDiscounts);
                if (result > 0)
                {
                    statusArray[0] = "200";
                    statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Discount Added successfully!<br/> <a id='lkGoBack' style='font-size: 14px; ' href='/Admin/AddAffiliateDiscount'>Go Back</a></span>";//"" + res;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                statusArray[0] = "201";
                statusArray[1] = "Unable to Save";
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                statusArray[0] = "201";
                statusArray[1] = exception.Message;
                return Json(statusArray, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult DiscountPromotions(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var discountPromotions = new DiscountsPromotionService().GetAllDiscountsPromotions(out totalCount
                                                                        , pageSize: pageSize, pageIndex: page);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(discountPromotions);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult DiscountPromotions(DiscountPromotion discountPromotion, FormCollection formCollection)
        {
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                page = Convert.ToInt32(Request.Form["btnPagination"]);

            if (discountPromotion != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var discountPromotions = new DiscountsPromotionService()
                        .GetAllDiscountsPromotions(out totalCount, pageIndex: page, pageSize: pageSize);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;

                    return View(discountPromotions);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();

        }

        [HttpGet]
        public ActionResult EditDiscountPromotions(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    //id = new Utility().UrlSafeDecrypt(id);
                    var discountPromotions = new DiscountsPromotionService().GetDiscountPromotions(id);
                    return View(discountPromotions);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            else { return RedirectToAction("Actions"); }
        }

        [HttpPost]
        public ActionResult EditDiscountPromotions(DiscountPromotion discountPromotion, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());
            if (ModelState.IsValid)
            {
                if (discountPromotion != null)
                {
                    try
                    {
                        bool isSaved = new DiscountsPromotionService().UpdateDiscountPrmotion(discountPromotion);
                        if (isSaved)
                        {
                            ViewBag.MessageSuccess = "Record Updated Successfully";
                            return View(discountPromotion);
                        }
                        ViewBag.MessageFailure = "Record not Updated,Please try again";
                        return View(discountPromotion);
                    }
                    catch (UserException exception)
                    {
                        log.Error(exception);
                        ViewBag.MessageFailure = exception.Message;
                        return View(discountPromotion);
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View(discountPromotion);
        }

        [HttpGet]
        public ActionResult AddDiscountPromotion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDiscountPromotion(DiscountPromotion discountPromotion, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    int isSaved = new DiscountsPromotionService().AddDiscountPromotion(discountPromotion);
                    if (isSaved > 0)
                    {
                        ViewBag.MessageSuccess = "Record Added Successfully";
                        ModelState.Clear();
                        return View();
                    }
                    ViewBag.MessageFailure = "Record not Saved,Please try again";
                    return View(discountPromotion);
                }
                catch (UserException exception)
                {
                    log.Error(exception);
                    ViewBag.MessageFailure = exception.Message;
                    return View(discountPromotion);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return View(discountPromotion);
                }
            }
            return View(discountPromotion);
        }
        public ActionResult UserActivities(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var userActivities = new AdminService()
                    .GetUserActivities(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(userActivities);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult UserActivities(UserActivity userActivity, FormCollection formCollection)
        {
            try
            {
                int page = 0;
                if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                    return Redirect(Request.UrlReferrer.ToString());

                if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                {
                    page = Convert.ToInt32(Request.Form["btnPagination"]);
                    userActivity.CreatedDateFrom = null;
                    userActivity.CreatedDateTo = null;
                }

                if (userActivity != null)
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var userActivities = new AdminService()
                        .GetUserActivities(out totalCount, pageIndex: page, pageSize: pageSize
                        , createdDateFrom: userActivity.CreatedDateFrom, createdDateTo: userActivity.CreatedDateTo);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(userActivities);
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        public ActionResult AuditCardRequest(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var cardRequestForms = new AdminService()
                    .GetCardRequestAudit(out totalCount, pageIndex: page, pageSize: pageSize);
                this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                this.ViewBag.Page = page;
                return View(cardRequestForms);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult AuditCardRequest(CardRequestForm cardRequestForm, FormCollection formCollection)
        {
            try
            {
                int page = 0;
                if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                    return Redirect(Request.UrlReferrer.ToString());

                if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
                {
                    page = Convert.ToInt32(Request.Form["btnPagination"]);
                }

                if (cardRequestForm != null)
                {
                    int totalCount = 0;
                    //int? roleId = null;

                    //if (cardRequestForm.UserActivity.UserId <= 0)
                    //    roleId = null;
                    //else
                    //    roleId = cardRequestForm.UserActivity.UserId;

                    int pageSize = new SettingService().AdminPageSize;
                    var cardRequestForms = new AdminService()
                        .GetCardRequestAudit(out totalCount, nic: cardRequestForm.Customer.NIC, pageIndex: page
                        , pageSize: pageSize, createdDateFrom: cardRequestForm.CreatedDateFrom
                        , createdDateTo: cardRequestForm.CreatedDateTo);
                    this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
                    this.ViewBag.Page = page;
                    return View(cardRequestForms);
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return View();
            }
            return null;
        }
    }
}

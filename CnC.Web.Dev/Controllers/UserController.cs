using CnC.Core.Accounts;
using CnC.Service;
using System;
using System.Web.Mvc;
using CnC.Web.Helper;
using log4net;
using System.Collections.Generic;
using CnC.Core.Exceptions;

namespace CnC.Web.Controllers
{
    [Authorize]
    [RoleActionValidator]
    public class UserController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(AdminController));

        [HttpGet]
        public ActionResult Users(int page = 0)
        {
            try
            {
                int totalCount = 0;
                int pageSize = new SettingService().AdminPageSize;
                var model = new AdminService().GetUsers(out totalCount, pageIndex: page, pageSize: pageSize);
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
        public ActionResult Users(User user, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            if (user != null)
            {
                try
                {
                    int totalCount = 0;
                    int pageSize = new SettingService().AdminPageSize;
                    var model = new AdminService().GetUsers(out totalCount, firstName: user.FirstName
                        , lastName: user.LastName, email: user.Email, createdDateFrom: user.CreatedDateFrom
                        , createdDateTo: user.CreatedDateTo, roleId: user.RoleId, pageIndex: page, pageSize: pageSize);
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
        public ActionResult AddUser()
        {
            return View();
        }
        public JsonResult GeneratePassword()
        {
            string password = new CommonService().GeneratePassword(10);
            return Json(password, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUser(User user, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());
            if (user != null)
            {
                try
                {
                    string password = user.NewPassword;
                    int roleId = user.RoleId;
                    var userRole = new Role() { Id = roleId };
                    var userService = new UserService();
                    int userId = userService.AddUser(user, userRole, password);
                    if (userId > 0)
                    {
                        ViewBag.Message = "Record Updated Successfully";
                        ModelState.Clear();
                        return View();
                    }
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    ViewBag.Message = exception.Message;
                    return View();
                }
            }
            ViewBag.Message = "Record not Updated,Please try again";
            return View();
        }

        [HttpGet]
        public ActionResult EditUser(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    var user = new UserService().GetUser(email);
                    return View(user);
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
        public ActionResult EditUser(User user, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnCancel"]))
                return Redirect(Request.UrlReferrer.ToString());
            if (ModelState.IsValid)
            {
                try
                {
                    if (user != null)
                    {
                        bool isSaved = new UserService().UpdateUser(user);
                        if (isSaved)
                        {
                            ModelState.Clear();
                            return RedirectToAction("Users");
                        }
                        ViewBag.Message = "Record not Updated,Please try again";
                        return View(user);
                    }
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(user);
        }
        public JsonResult ResetPassword(int userId, string password)
        {
            string[] statusArray = new string[3];
            if (!string.IsNullOrEmpty(password))
            {
                try
                {
                    bool result = new UserService().ResetPassword(userId, password);
                    if (result)
                    {
                        statusArray[0] = "100";
                        statusArray[1] = "Password has been updated successfully";

                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    statusArray[0] = "101";
                    statusArray[1] = "Password cannot be changed !";
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
            else
            {
                statusArray[0] = "101";
                statusArray[1] = "Password is Required.";
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }
        public List<Role> GetRoles()
        {
            try
            {
                var userService = new UserService();
                var roles = userService.GetRoles();
                return roles;
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }
        }

    }
}
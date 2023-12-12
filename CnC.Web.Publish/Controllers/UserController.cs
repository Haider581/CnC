using CnC.Core.Accounts;
using CnC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Dev;
using CnC.Core;
using CnC.Web.Models;

namespace CnC.Web.Controllers
{
    [RoleActionValidator]
    public class UserController : Controller
    {
        public List<User> GetUsers()
        {
            try
            {
                var userService = new UserService();
                var users = userService.GetUsers();
                return users;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        //public ActionResult ResetPassword(int userId)
        //{
        //    return View();
        //}

        //public ActionResult ResetPassword(FormCollection formCollection)
        //{
        //    int userId = int.Parse(formCollection["userId"]);
        //    string password = Convert.ToString(formCollection["Password"]);

        //    var userService = new UserService();
        //    userService.ResetPassword(userId, password);
        //    return RedirectToAction("UserSearch", new { message = "success" });
        //}

      
        public JsonResult ResetPassword(int userId, string password)
        {
            var userService = new UserService();
            bool result = userService.ResetPassword(userId, password);

            string messageDesc = "Updated Successfully!";
            return Json(messageDesc, JsonRequestBehavior.AllowGet); ;
        }

        public ActionResult AddUser()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserSearch()
        {
            User user = new User();
            //var roles = new List<Role>();
            //roles.Add(new Role { Name = "RAHYAB" });
            //roles.Add(new Role { Name = "CallCenter" });
            //roles.Add(new Role { Name = "TBOAgent" });

            //user.Roles = roles;
            var userService = new UserService();
            List<User> result = userService.GetUsers();

            return View(result);
        }

        [HttpPost]
        public ActionResult UserSearch(FormCollection formCollection)
        {
            string firstName = Convert.ToString(formCollection["firstName"]);
            string lastName = Convert.ToString(formCollection["lastName"]);
            int? roleId = null;
            if (!string.IsNullOrWhiteSpace(formCollection["ddlRoles"]))
            {
                roleId = int.Parse(formCollection["ddlRoles"]);
            }
            string Email = Convert.ToString(formCollection["Email"]);
            UserStatus Status = formCollection["Status"] == "on" ? UserStatus.Active : UserStatus.Inactive;
            DateTime? registrationDateFrom = null;
            if (!string.IsNullOrWhiteSpace(Request.Form["txtRegistrationDateFrom"]))
            {
                registrationDateFrom = DateTime.Parse(Request.Form["txtRegistrationDateFrom"]);
            }

            DateTime? registrationDateTo = null;
            if (!string.IsNullOrWhiteSpace(Request.Form["txtRegistrationDateTo"]))
            {
                registrationDateTo = Convert.ToDateTime(Request.Form["txtRegistrationDateTo"]);
            }

            var userService = new UserService();
            List<User> result = userService.GetUsers(firstName, lastName, roleId, Status, registrationDateFrom, registrationDateTo);

            return View(result);
        }

        [HttpPost]
        public ActionResult AddUser(FormCollection formCollection)
        {
            User user = new User();
            user.FirstName = Convert.ToString(formCollection["FirstName"]);
            user.LastName = Convert.ToString(formCollection["LastName"]);
            user.Username = Convert.ToString(formCollection["Username"]);
            user.Email = Convert.ToString(formCollection["Email"]);
            string password = Convert.ToString(formCollection["Password"]);
            int roleId = int.Parse(Convert.ToString(formCollection["ddlRoles"]));
            user.Status = (formCollection["Status"] == "on") ? (int)UserStatus.Active : (int)UserStatus.Inactive;
            var userRole = new Role() { Id = roleId };
            var userService = new UserService();
            int userId = userService.AddUser(user, userRole, password);

            if (userId > 0)
            {
                ViewBag.ResponseDesc = "User has been added successfully";
                return PartialView("_Success");
            }
            return PartialView("_Success");
        }

        public List<Role> GetRoles(List<string> skipRoles)
        {
            try
            {
                var userService = new UserService();
                var roles = userService.GetRoles(skipRoles);
                return roles;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }
}
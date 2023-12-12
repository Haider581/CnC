using System;
using System.Linq;
using CnC.Core.Accounts;
using System.Collections.Generic;
using CnC.Core;
using CnC.Data;
using System.Data.Entity;
using log4net;
using System.Reflection;
using CnC.Core.Exceptions;

namespace CnC.Service
{
    public class UserService
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(UserService));

        private EntityContext context;

        public UserService()
        {
            context = new EntityContext();
        }

        #region Role

        public Role GetRole(int roleId)
        {
            try
            {
                return context.Roles.SingleOrDefault(r => r.Id == roleId);
            }
            catch (Exception excep) { new MessageService().SendExceptionMessage(excep); }
            return null;
        }
        public Role GetRole(string name)
        {
            try
            {
                return context.Roles.SingleOrDefault(r => r.Name.ToLower() == name.ToLower());
            }
            catch (Exception excep) { }
            return null;
        }
        public List<Role> GetRoles()
        {
            using (var context = new EntityContext())
            {
                var roles = context.Roles.ToList();
                return roles;
            }

        }
        public List<Role> GetRoles(List<string> skipRoles)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var roles = context.Roles.Where(r => !skipRoles.Contains(r.Name)).ToList();
                    return roles;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                throw new UserException("Sorry, we are unable to process your request. Please try again later.");
            }
        }

        #endregion

        #region User

        public int AddUser(User user, Role role, string password = "")
        {
            if (user == null)
                throw new ArgumentNullException("User is required");

            if (role == null)
                throw new ArgumentNullException("Role is required");

            if (!string.IsNullOrEmpty(password) && password.Length < 8)
                throw new ArgumentNullException("Password length must be at least 8 characters");

            user.RoleId = role.Id;

            ValidateUser(user, false);

            try
            {
                using (var context = new EntityContext())
                {
                    var checkUser = (from u in context.Users
                                     where u.Username.Equals(user.Username, StringComparison.CurrentCultureIgnoreCase)
                                     || u.Email.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase)
                                     select u)
                                     .SingleOrDefault();

                    if (checkUser != null)
                        throw new UserException("User with this email already exist");

                    var commonService = new CommonService();
                    password = string.IsNullOrEmpty(password) ? commonService.GeneratePassword(10) : password;
                    var hashKey = commonService.GeneratePassword(6);
                    user.HashedPassword = commonService.EncodeText(password, hashKey);
                    user.HashKey = hashKey;

                    context.Users.Add(user);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name, performedForUserId: user.Id);

                    return user.Id;
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                throw new UserException("Sorry, we are unable to process your request. Please try again later.");
            }
        }

        public int SignIn(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("Username is required");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Password is required");

            try
            {
                int userId = Authenticate(userName, password);

                if (userId <= 0)
                    return -1;

                new AuditService().LogSignIn(userId);
                return userId;
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public int Authenticate(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("Username is required");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Password is required");

            try
            {
                using (var context = new EntityContext())
                {
                    if (context.Users.Where(u => (u.Username.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                                 || u.Email.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
                                 && u.Status == (int)UserStatus.Inactive).Count() > 0)
                        throw new UserException("User is In-Active");

                    var user = (from u in context.Users
                                join role in context.Roles on u.RoleId equals role.Id
                                join customer in context.Customers on u.Id equals customer.UserId into customers
                                from customer in customers.DefaultIfEmpty()
                                where (u.Username.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                                || u.Email.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
                                && u.Status == (int)UserStatus.Active
                                select new { User = u, Role = role, Customer = customer }).SingleOrDefault();

                    if (user != null)
                    {
                        var hashCode = user.User.HashKey;
                        //Password Hashing Process
                        var commonService = new CommonService();
                        var encodingPasswordString = commonService.EncodeText(password, hashCode);

                        if (user.User.HashedPassword == encodingPasswordString)
                        {
                            if (user.Role.Name.Equals("customer", StringComparison.OrdinalIgnoreCase) && user.Customer == null)
                                return -1;

                            return user.User.Id;
                        }
                    }
                }
                return -1;
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public User GetUser(int userId)
        {
            try
            {
                return context.Users.SingleOrDefault(u => u.Id == userId);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }

        }

        public User GetUser(string email)
        {
            try
            {
                return (from u in context.Users where u.Email == email select u).SingleOrDefault();
            }
            catch (Exception excep)
            {
                new MessageService().SendExceptionMessage(excep);
            }
            return null;
        }

        public List<User> GetUsers(out int totalCount, string firstName = null, string lastName = null, int? roleId = null
            , UserStatus? userStatus = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null
            , int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        {
            totalCount = 0;
            try
            {
                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                if (createdDateTo.HasValue)
                    createdDateTo = createdDateTo.Value.AddDays(1);

                int skipRows = pageSize.Value * pageIndex;

                var result = (from user in context.Users
                              join role in context.Roles on user.RoleId equals role.Id
                              where
                              (true == (!string.IsNullOrEmpty(firstName) ? user.FirstName.Contains(firstName.ToLower()) : true))
                              && (true == (!string.IsNullOrEmpty(lastName) ? user.LastName.Contains(lastName.ToLower()) : true))
                              && user.Status == (userStatus.HasValue ? (int)userStatus.Value : user.Status)
                              && user.RoleId == (roleId.HasValue ? roleId.Value : user.RoleId)
                              && user.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : user.CreatedOn)
                              && user.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : user.CreatedOn)
                              select new { User = user, Role = role }).OrderBy(r => r.User.FirstName).ThenBy(r => r.User.LastName).ThenBy(r => r.Role.Name)
                             .Skip(skipRows).Take(pageSize.Value);

                foreach (var user in result)
                {
                    user.User.Role = user.Role;
                }

                return result.Select(u => u.User).ToList();
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public Role GetUserRole(int userId)
        {
            try
            {
                return (from role in context.Roles
                        join user in context.Users on role.Id equals user.RoleId
                        where user.Id == userId
                        orderby role.Name
                        select role).SingleOrDefault();
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }
        }

        public List<CnCAction> GetUserActions(int userId)
        {
            try
            {
                var userActions = (from user in context.Users
                                   join role in context.Roles on user.RoleId equals role.Id
                                   join roleAction in context.RoleActions on role.Id equals roleAction.RoleId
                                   join action in context.CnCActions on roleAction.ActionId equals action.Id
                                   where user.Id == userId
                                         && roleAction.IsAllowed
                                         && !string.IsNullOrEmpty(action.DisplayName)
                                   orderby roleAction.DisplayOrder
                                   select new
                                   {
                                       Action = action,
                                       IsDefault = roleAction.IsDefault,
                                       IsAllowed = roleAction.IsAllowed
                                   }).ToList();

                if (userActions.Count == 0)
                    return null;

                foreach (var userAction in userActions)
                {
                    userAction.Action.IsDefault = userAction.IsDefault;
                    userAction.Action.IsAllowed = userAction.IsAllowed;
                }

                var parentActions = userActions.Where(ua => ua.Action.ParentActionId == null).ToList();
                var userActionsAll = userActions.Select(ua => ua.Action).ToList();

                foreach (var parentAction in parentActions)
                {
                    GetAndSetSubActions(parentAction.Action, userActionsAll);
                }

                return parentActions.Select(ua => ua.Action).ToList();
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        private void GetAndSetSubActions(CnCAction parentAction, List<CnCAction> userActions)
        {
            var subActions = userActions
                .Where(ua => ua.ParentActionId != null && ua.ParentActionId == parentAction.Id).ToList();

            if (subActions.Count > 0)
            {
                parentAction.SubActions = subActions;

                foreach (var subAction in subActions)
                {
                    if (!parentAction.IsDefault
                        ||
                        (string.IsNullOrEmpty(parentAction.ActionName) ||
                         string.IsNullOrEmpty(parentAction.ControllerName)))
                    {
                        if (parentAction.DefaultAction == null && subAction.IsDefault
                        && !string.IsNullOrEmpty(subAction.ActionName) && !string.IsNullOrEmpty(subAction.ControllerName))
                        {
                            parentAction.DefaultAction = subAction;
                        }
                    }
                    GetAndSetSubActions(subAction, userActions);
                }
            }
        }
        public bool ResetPassword(int userId, string password)
        {
            if (userId <= 0)
                throw new ArgumentNullException("User is invalid");

            if (string.IsNullOrEmpty(password) || password.Length < 8)
                throw new UserException("Password length must be at least 8 characters");

            try
            {
                using (var context = new EntityContext())
                {
                    var user = (from u in context.Users where u.Id == userId select u).SingleOrDefault();

                    if (user == null)
                        throw new UserException("User not found");

                    var commonService = new CommonService();
                    var hashKey = commonService.GeneratePassword(6);
                    user.HashedPassword = commonService.EncodeText(password, hashKey);
                    user.HashKey = hashKey;
                    user.ChangePasswordRequired = true;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    var userSession = System.Web.HttpContext.Current.Session["CurrentSession"] as User;

                    new MessageService().SendChangePasswordMessageToUser(user: userSession, password: password
                                        , customer: user
                                        , languageId: new SettingService().CustomerDefaultLanguage.Id);

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name, performedForUserId: userId);

                    return true;
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                throw new UserException("Sorry, we are unable to process your request. Please try again later.");
            }
        }

        /// <summary>
        /// Validate Customer before adding in Database
        /// </summary>
        public void ValidateUser(User user, bool isCustomer)
        {
            var commonService = new CommonService();

            if (string.IsNullOrEmpty(user.Email) || !commonService.IsValidEmail(user.Email))
                throw new ArgumentException("Email is either missing or invalid");

            if (user.Email.Length > 50)
                throw new ArgumentException("Email length cannot be greater than 50");

            if (string.IsNullOrEmpty(user.FirstName))
                throw new ArgumentException("First Name is required");

            if ((user.FirstName.Length < 2 || user.FirstName.Length > 50))
                throw new ArgumentException(string.Format("First Name length must be between {0} and {1}"
                    , 2, 50));

            if (isCustomer && string.IsNullOrEmpty(user.FirstNameInCustomerLanguage))
                throw new ArgumentNullException("First Name in Customer Language is required");

            if (isCustomer && (user.FirstNameInCustomerLanguage.Length < 2 || user.FirstNameInCustomerLanguage.Length > 50))
                throw new ArgumentNullException(string.Format("First Name in Customer Language length must be between {0} and {1}"
                    , 2, 50));

            if (string.IsNullOrEmpty(user.LastName))
                throw new ArgumentNullException("Last Name is required");

            if (user.LastName.Length < 2 || user.LastName.Length > 50)
                throw new ArgumentNullException(string.Format("Last Name length must be between {0} and {1}"
                    , 2, 50));

            if (isCustomer && string.IsNullOrEmpty(user.LastNameInCustomerLanguage))
                throw new ArgumentNullException("Last Name in Customer Language is required");

            if (isCustomer && (user.LastNameInCustomerLanguage.Length < 2 || user.LastNameInCustomerLanguage.Length > 50))
                throw new ArgumentNullException(string.Format("Last Name in Customer Language length must be between {0} and {1}"
                    , 2, 50));

            if (user.RoleId <= 0)
                throw new ArgumentNullException("User Role is required");

            UserStatus userStatus;
            if (Enum.TryParse<UserStatus>(user.Status.ToString(), out userStatus) == false)
                throw new ArgumentNullException("Status is required");

            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentNullException("Username is required");
        }

        #endregion

        #region Action

        public CnCAction GetAction(int actionId)
        {
            try
            {
                return GetActions().FirstOrDefault(x => x.Id == actionId);
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public CnCAction GetAction(string name)
        {
            try
            {
                return GetActions().FirstOrDefault(x => x.DisplayName == name);
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<CnCAction> GetActions()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.CnCActions.ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<RoleAction> GetRoleActions(int roleId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.RoleActions.Where(ra => ra.RoleId == roleId).OrderBy(ra => ra.DisplayOrder).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        #endregion

        public bool UpdateCustomer(int userId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var user = (from u in context.Users
                                where u.Id == userId
                                select u).SingleOrDefault();
                    if (user != null)
                    {
                        user.Status = 1;
                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to update");
                    }
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }

            return true;
        }

        public bool UpdateUser(User user)
        {
            try
            {
                using (var context = new EntityContext())
                {

                    var result = (from u in context.Users
                                  where u.Id == user.Id
                                  select u).SingleOrDefault();
                    if (user != null)
                    {
                        result.FirstName = user.FirstName;
                        result.LastName = user.LastName;
                        result.Status = user.Status;
                        result.RoleId = user.RoleId;
                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to update");

                        return true;
                    }
                    return false;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }
    }

}
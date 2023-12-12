using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Audit;
using CnC.Core.Cards;
using CnC.Core.Exceptions;
using CnC.Data;
using log4net;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Service
{
    public class AdminService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(AdminService));
        public List<CnCAction> GetAllActions(out int totalCount, bool isActionCall = false, string displayName = null, string controllerName = null, string actionName = null,
            int pageIndex = 0, int? pageSize = null, DateTime? createdDateFrom = null,
            DateTime? createdDateTo = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;
            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from query in context.CnCActions
                                  where true == (!string.IsNullOrEmpty(displayName) ? query.DisplayName.ToLower().Contains(displayName.ToLower()) : true)
                                  && true == (!string.IsNullOrEmpty(actionName) ? query.ActionName.ToLower().Contains(actionName.ToLower()) : true)
                                  && true == (!string.IsNullOrEmpty(controllerName) ? query.ControllerName.ToLower().Contains(controllerName.ToLower()) : true)
                                   && query.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : query.CreatedOn)
                                  && query.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : query.CreatedOn)
                                  select query).ToList();
                    totalCount = result.Count();
                    var cncActions = new List<CnCAction>();
                    if (result != null && result.Count() > 0)
                    {
                        foreach (var item in result)
                        {
                            var cncAction = item;
                            int? actionId = item.ParentActionId;
                            if (actionId != null)
                            {
                                cncAction.ParentActionName = GetParentActionName(result, Convert.ToInt16(actionId));
                            }

                            cncActions.Add(cncAction);
                        }
                        if (isActionCall)
                        {
                            return cncActions.OrderBy(c => c.ParentActionId).ToList();
                        }
                        return cncActions.OrderBy(c => c.ActionName).Skip(skipRows).Take(pageSize.Value).ToList();
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public string GetParentActionName(List<CnCAction> result, int parentActionId)
        {
            string parentActionName = string.Empty;
            if (result != null && result.Count() > 0)
            {
                parentActionName = result.Where(c => c.Id == parentActionId).Select(c => c.ActionName).SingleOrDefault();
                return parentActionName;
            }
            return parentActionName;
        }

        public CnCAction GetParentAction(int parentActionId)
        {
            if (parentActionId < 0)
                throw new ArgumentNullException("Parent Action is Required");

            try
            {
                using (var context = new EntityContext())
                {
                    var parentAction = (from query in context.CnCActions
                                        where query.Id == parentActionId
                                        select query).SingleOrDefault();

                    return parentAction;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception.Message);
                return null;
            }
        }

        public CnCAction GetCncAction(string id)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        int actionId = Convert.ToInt32(id);
                        var result = context.CnCActions.SingleOrDefault(c => c.Id == actionId);
                        if (result != null)
                        {
                            return result;
                        }
                        return null;
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }

        }

        public bool UpdateCncAction(CnCAction cncAction)
        {
            if (cncAction == null)
                throw new ArgumentNullException("Action is required");

            try
            {
                using (var context = new EntityContext())
                {

                    var action = context.CnCActions.SingleOrDefault(c => c.Id == cncAction.Id);

                    if (action == null)
                        throw new UserException("Action with this Id not found");

                    action.ActionName = cncAction.ActionName;
                    action.ControllerName = cncAction.ControllerName;
                    action.CreatedOn = DateTime.Now;
                    action.DefaultAction = cncAction.DefaultAction;
                    action.DisplayName = cncAction.DisplayName;
                    action.IsAllowed = cncAction.IsAllowed;
                    action.IsDefault = cncAction.IsDefault;
                    action.Log = cncAction.Log;
                    action.ParentActionId = cncAction.ParentActionId;
                    action.ShowInMenu = cncAction.ShowInMenu;
                    action.ShowInTab = cncAction.ShowInTab;
                    action.SubActions = cncAction.SubActions;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return true;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }
        public int AddCncAction(CnCAction cncAction)
        {
            if (cncAction == null)
                throw new ArgumentNullException("Action is required");
            try
            {
                using (var context = new EntityContext())
                {
                    context.CnCActions.Add(cncAction);
                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return cncAction.Id;
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

        public List<Role> GetAllRoles(out int totalCount, bool isActionCall = false, string name = null, string controllerName = null, string actionName = null,
            int pageIndex = 0, int? pageSize = null, DateTime? createdDateFrom = null,
            DateTime? createdDateTo = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;
            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from query in context.Roles
                                  where true == (!string.IsNullOrEmpty(name) ? query.Name.ToLower().Equals(name.ToLower()) : true)
                                   && query.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : query.CreatedOn)
                                  && query.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : query.CreatedOn)
                                  select query);
                    totalCount = result.Count();
                    if (result != null && result.Count() > 0)
                    {
                        if (isActionCall)
                        {
                            return result.ToList();
                        }
                        return result.OrderByDescending(c => c.CreatedOn).Skip(skipRows).Take(pageSize.Value).ToList();
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public int AddRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("Role is required");

            try
            {
                using (var context = new EntityContext())
                {
                    context.Roles.Add(role);
                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return role.Id;
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

        public bool UpdateRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("Role is required");

            try
            {
                using (var context = new EntityContext())
                {
                    var cncRole = context.Roles.SingleOrDefault(c => c.Id == role.Id);

                    if (cncRole == null)
                        throw new UserException("Role with this Id not found");

                    cncRole.Name = role.Name;
                    cncRole.Description = role.Description;
                    cncRole.CreatedOn = DateTime.Now;
                    cncRole.CanViewOnlineRecords = role.CanViewOnlineRecords;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return true;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }
        public List<User> GetUsers(out int totalCount, string firstName = null, string lastName = null
            , string email = null, int? roleId = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null, int pageIndex = 0
            , int? pageSize = null)
        {
            totalCount = 0;

            try
            {
                if (roleId == 0)
                {
                    roleId = null;
                }
                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                if (createdDateTo.HasValue)
                    createdDateTo = createdDateTo.Value.AddDays(1);

                int skipRows = pageSize.Value * pageIndex;
                int defaultUsersCount = new SettingService().DefaultUserCount;

                using (var context = new EntityContext())
                {
                    var result = (from user in context.Users
                                  join role in context.Roles on user.RoleId equals role.Id
                                  where
                                  (true == (!string.IsNullOrEmpty(firstName) ? user.FirstName.Contains(firstName.ToLower()) : true))
                                  && (true == (!string.IsNullOrEmpty(lastName) ? user.LastName.Contains(lastName.ToLower()) : true))
                                  && (true == (!string.IsNullOrEmpty(email) ? user.Email.Contains(email.ToLower())
                                  : true))
                                  && user.RoleId == (roleId.HasValue ? roleId.Value : user.RoleId)
                                  && user.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : user.CreatedOn)
                                  && user.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : user.CreatedOn)
                                  && user.Id > defaultUsersCount
                                  select new { User = user, Role = role }).OrderByDescending(r => r.User.CreatedOn)
                                      .ThenBy(r => r.User.LastName)
                                      .ThenBy(r => r.Role.Name);
                    totalCount = result.Count();
                    var users = result.Skip(skipRows).Take(pageSize.Value);
                    foreach (var user in users)
                    {
                        user.User.Role = user.Role;
                    }

                    return users.Select(u => u.User).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public List<RoleActionsRole> GetAllRoleActions(out int totalCount, bool isActionCall = false, int pageIndex = 0
            , int? pageSize = null, int? actionId = null, int? roleId = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;
            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            if (actionId == 0)
                actionId = null;
            if (roleId == 0)
                roleId = null;
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from roleAction in context.RoleActions
                                  join role in context.Roles on roleAction.RoleId equals role.Id
                                  join action in context.CnCActions on roleAction.ActionId equals action.Id
                                  where roleAction.RoleId == (roleId.HasValue ? roleId.Value : roleAction.RoleId)
                                  && action.Id == (actionId.HasValue ? actionId.Value : action.Id)
                                  select new
                                  {
                                      roleAction,
                                      role,
                                      action
                                  });
                    totalCount = result.Count();
                    if (result != null && result.Count() > 0)
                    {
                        var listRoleActionsRole = new List<RoleActionsRole>();
                        foreach (var item in result)
                        {
                            var roleActionsRole = new RoleActionsRole();
                            roleActionsRole.Role = item.role;
                            roleActionsRole.RoleAction = item.roleAction;
                            roleActionsRole.CnCAction = item.action;

                            listRoleActionsRole.Add(roleActionsRole);
                        }
                        if (isActionCall)
                        {
                            return listRoleActionsRole.OrderBy(c => c.RoleAction.RoleId).ToList();
                        }
                        return listRoleActionsRole.OrderBy(c => c.RoleAction.RoleId).Skip(skipRows).Take(pageSize.Value).ToList();/* result.OrderByDescending(c => c.roleAction.RoleId)*/
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public RoleActionsRole GetRoleActionToUpdate(int? actionId = null, int? roleId = null)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from roleAction in context.RoleActions
                                  join role in context.Roles on roleAction.RoleId equals role.Id
                                  join action in context.CnCActions on roleAction.ActionId equals action.Id
                                  where roleAction.RoleId == (roleId.HasValue ? roleId.Value : roleAction.RoleId)
                                  && roleAction.ActionId == (actionId.HasValue ? actionId.Value : roleAction.ActionId)
                                  select new
                                  {
                                      roleAction,
                                      role,
                                      action
                                  }).SingleOrDefault();

                    if (result != null)
                    {
                        var roleActionsRole = new RoleActionsRole();
                        roleActionsRole.Role = result.role;
                        roleActionsRole.RoleAction = result.roleAction;
                        roleActionsRole.CnCAction = result.action;
                        return roleActionsRole;
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public RoleAction GetRoleAction(string id)
        {
            var ids = id.Split('-');
            int roleId = Convert.ToInt32(ids[0]);
            int actionId = Convert.ToInt32(ids[1]);
            try
            {
                using (var context = new EntityContext())
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        var result = context.RoleActions.SingleOrDefault(c => c.RoleId == roleId && c.ActionId == actionId);
                        if (result != null)
                        {
                            return result;
                        }
                        return null;
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public bool UpdateRoleAction(RoleActionsRole roleActionRole)
        {
            if (roleActionRole == null)
                throw new ArgumentNullException("Role and Action is required");

            try
            {
                using (var context = new EntityContext())
                {
                    var roleAction = context.RoleActions.SingleOrDefault(c => c.RoleId == roleActionRole.RoleAction.RoleId &&
                                                                    c.ActionId == roleActionRole.RoleAction.ActionId);

                    if (roleAction == null)
                        throw new UserException("Role Action with this Id not found");

                    roleAction.RoleId = roleActionRole.RoleAction.RoleId;
                    roleAction.ActionId = roleActionRole.RoleAction.ActionId;
                    roleAction.DisplayOrder = roleActionRole.RoleAction.DisplayOrder;
                    roleAction.IsAllowed = roleActionRole.RoleAction.IsAllowed;
                    roleAction.IsDefault = roleActionRole.RoleAction.IsDefault;
                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);
                    return true;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }
        public int AddRoleAction(RoleAction roleAction)
        {
            if (roleAction == null)
                throw new ArgumentNullException("Role Action is required");

            try
            {
                using (var context = new EntityContext())
                {
                    context.RoleActions.Add(roleAction);
                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);
                    return roleAction.ActionId;
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
        public List<UserActivity> GetUserActivities(out int totalCount, int pageIndex = 0
                                     , DateTime? createdDateFrom = null, DateTime? createdDateTo = null
                                     , int? pageSize = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from query in context.UserActivities
                                  where query.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value
                                  : query.CreatedOn)
                                  && query.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value
                                  : query.CreatedOn)
                                  select query).OrderByDescending(c => c.CreatedOn).ToList();
                    totalCount = result.Count();

                    if (result != null)
                    {
                        return result.Skip(skipRows).Take(pageSize.Value).ToList();
                    }
                    return null;
                }
            }

            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }

        }

        public List<CardRequestForm> GetCardRequestAudit(out int totalCount, int pageIndex = 0, string firstName = null
                                      , string lastName = null, string email = null, int? roleId = null, string nic = null
                                     , DateTime? createdDateFrom = null, DateTime? createdDateTo = null
                                     , int? pageSize = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from customer in context.Customers
                                  join user in context.Users on customer.UserId equals user.Id
                                  join requestForms in context.RequestForms on customer.UserId
                                  equals requestForms.CustomerId
                                  join userActivities in context.UserActivities on requestForms.CustomerId
                                  equals userActivities.PerformedForUserId
                                  join cardRequests in context.CardRequests on requestForms.Id
                                  equals cardRequests.CardRequestFormId
                                  join cardType in context.CardTypes on cardRequests.CardTypeId equals cardType.Id
                                  where userActivities.Action.Equals("AddCardRequestForm")
                                  //&& requestForms.TypeId == (int)RequestFormType.Card
                                  && userActivities.UserId == (roleId.HasValue ? roleId.Value : userActivities.UserId)
                                  && (true == (!string.IsNullOrEmpty(nic) ? customer.NIC
                                            .Equals(nic, StringComparison.OrdinalIgnoreCase) : true))
                                  && (true == (!string.IsNullOrEmpty(firstName) ? user.FirstName
                                            .Contains(firstName.ToUpper()): true))
                                  && (true == (!string.IsNullOrEmpty(lastName) ? user.LastName
                                              .Contains(lastName.ToUpper()): true))
                                 && (true == (!string.IsNullOrEmpty(email) ? user.Email.Contains(email) : true))
                                  && requestForms.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value
                                  : requestForms.CreatedOn)
                                  && requestForms.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value
                                  : requestForms.CreatedOn)
                                  select new
                                  {
                                      Customer = customer,
                                      User = user,
                                      UserActivitiy = userActivities,
                                      RequestForm = requestForms,
                                      CardRequest = cardRequests,
                                      CardType = cardType
                                  }).GroupBy(rf => rf.RequestForm.Id)
                                  .OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn).ToList();

                    totalCount = result.Count();
                    var auditCardRequests = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (auditCardRequests != null && auditCardRequests.Count() > 0)
                    {
                        List<CardRequestForm> cardRequestForms = new List<CardRequestForm>();

                        foreach (var grp in auditCardRequests)
                        {
                            var firstRecord = grp.First();
                            CardRequestForm cardRequestForm = new CardRequestForm(firstRecord.RequestForm);
                            cardRequestForm.Customer = firstRecord.Customer;
                            cardRequestForm.Customer.User = firstRecord.User;
                            cardRequestForm.CardRequests = new List<CardRequest>();
                            cardRequestForm.UserActivity = firstRecord.UserActivitiy;

                            foreach (var requestForm in grp.DistinctBy(r => r.CardRequest))
                            {
                                requestForm.CardRequest.CardRequestForm = cardRequestForm;
                                requestForm.CardRequest.CardType = requestForm.CardType;
                                cardRequestForm.CardRequests.Add(requestForm.CardRequest);
                            }

                            cardRequestForms.Add(cardRequestForm);
                        }

                        return cardRequestForms;
                    }

                    return null;
                }
            }

            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }

        }

    }
}
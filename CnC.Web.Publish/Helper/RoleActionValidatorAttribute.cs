using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CnC.Core.Accounts;

namespace CnC.Web.Dev
{
    public class RoleActionValidatorAttribute : FilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values["controller"];
            var actionName = filterContext.RouteData.Values["action"];

            if (HttpContext.Current.Session["UserActions"] != null)
            {
                var userActions = HttpContext.Current.Session["UserActions"] as List<CnCAction>;

                if (userActions != null && actionName != null && controllerName != null)
                {
                    var isPageFound = IsActionFound(userActions, (string)actionName, (string)controllerName);
                    if (isPageFound)
                    {
                        return;
                    }
                    filterContext.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Error" }));
                    filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                }
                else
                {
                    filterContext.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Error" }));
                    filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                }
            }
            else
            {
                filterContext.Result =
                    new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
            }
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            // To do
        }

        private bool IsActionFound(List<CnCAction> userActions, string actionName, string controllerName)
        {
            if (userActions != null && userActions.Count > 0)
            {
                bool isActionFound = userActions.Any(
                    a => (a.ActionName == actionName && a.ControllerName == controllerName));

                if (isActionFound)
                    return true;

                foreach (var userAction in userActions)
                {
                    if (userAction.SubActions != null && userAction.SubActions.Count > 0)
                    {
                        isActionFound = userAction.SubActions.Any(
                            sa => (sa.ActionName == actionName && sa.ControllerName == controllerName));

                        if (isActionFound)
                            return true;

                        foreach (var userSubAction in userAction.SubActions)
                        {
                            if (userSubAction.SubActions != null && userSubAction.SubActions.Count > 0)
                            {
                                isActionFound = userSubAction.SubActions.Any(
                                    sa => (sa.ActionName == actionName && sa.ControllerName == controllerName));

                                if (isActionFound)
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
using CnC.Core.Accounts;
using CnC.Core.Audit;
using CnC.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CnC.Service
{
    public class AuditService
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(AuditService));

        #region User Activity

        public void LogSignIn(int userId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    UserSignIn userSignIn = new UserSignIn();
                    userSignIn.UserId = userId;
                    context.UserSignIns.Add(userSignIn);
                    context.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
            }
        }

        public void LogAction(UserActivity userActivity)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    context.UserActivities.Add(userActivity);
                    context.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                new MessageService().SendExceptionMessage(exception);
            }
        }

        public void LogAction(string action, int? performedForUserId = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(action))
                {
                    var userActivity = new UserActivity();
                    userActivity.Action = action;

                    if (performedForUserId != null)
                    {
                        userActivity.PerformedForUserId = performedForUserId;
                    }

                    var ipAddress = new CommonService().GetIPAddress();
                    if (!string.IsNullOrEmpty(ipAddress))
                        userActivity.IpAddress = ipAddress;

                    var browserName = new CommonService().GetWebBrowserName();
                    if (!string.IsNullOrEmpty(browserName))
                        userActivity.BrowserName = browserName;

                    var machineName = new CommonService().GetMachineName();
                    if (!string.IsNullOrEmpty(machineName))
                        userActivity.MachineName = machineName;

                    var userSession = HttpContext.Current.Session["CurrentSession"] as User;

                    if (userSession != null)
                    {
                        userActivity.UserId = userSession.Id;
                        LogAction(userActivity);
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                new MessageService().SendExceptionMessage(exception);
            }
        }

        #endregion
    }
}

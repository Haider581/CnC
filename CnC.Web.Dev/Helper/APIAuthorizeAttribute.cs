using CnC.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CnC.Web.Helper
{
    public class APIAuthorizeAttribute : AuthorizeAttribute
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(APIAuthorizeAttribute));
        public override void OnAuthorization(HttpActionContext filterContext)
        {
            if (Authorize(filterContext))
            {
                return;
            }
            HandleUnauthorizedRequest(filterContext);
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }

        private bool Authorize(HttpActionContext actionContext)
        {
            try
            {
                var tokenRequest = actionContext.Request.Headers.GetValues("Token").First();
                var secretKey = new SettingService().WebAPISecretKey;

                bool validFlag = false;

                if (!string.IsNullOrEmpty(tokenRequest) && !string.IsNullOrEmpty(secretKey))
                {
                    if (secretKey.Equals(tokenRequest))
                        validFlag = true;
                    else
                        validFlag = false;
                }
                return validFlag;
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return false;
            }
        }

    }
}
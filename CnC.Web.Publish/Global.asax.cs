using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;

namespace CnC.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);//WEB API 1st
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            

            log4net.Config.XmlConfigurator.Configure();
            //CurrentApplication currentApplication = new CurrentApplication();
            //Application["CurrentApplication"] = currentApplication;

        }
        void Application_AcquireRequestState(object sender, EventArgs e)
        {

            //if (Session["UserActions"] != null)
            //{
            //    HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
            //    RouteData rd = RouteTable.Routes.GetRouteData(context);

            //    if (rd != null)
            //    {
            //        string controllerName = rd.GetRequiredString("controller");
            //        string actionName = rd.GetRequiredString("action");
            //        Response.Write("c:" + controllerName + " a:" + actionName);
            //        Response.End();
            //    }
            //}
            
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            //CurrentSession currentSession = new CurrentSession();
            
           // UserService userService = new UserService();
            //User loginUser = userService.GetUser(1);
            //currentSession.LoginUser = loginUser;
           // Session["CurrentSession"] = currentSession;                        
        }

        protected void Session_End(object sender, EventArgs e)
        {
            //Redirect to login page
            //Response.Redirect("/Login");
            //
        }
        

    }
}

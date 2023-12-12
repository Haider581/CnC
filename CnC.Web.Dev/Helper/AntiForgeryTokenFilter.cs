using System.Web.Mvc;
using System.Web.Routing;

namespace CnC.Web.Helper
{
    public class AntiForgeryTokenFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(System.Web.Mvc.ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(HttpAntiForgeryException))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    (new { controller = "Common", action = "Unauthorize" }));

                filterContext.ExceptionHandled = true;
            }
        }
    }
}
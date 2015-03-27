using System.Web.Mvc;
using System.Web.Routing;

namespace ItWebSite.Web.DAL.MySql
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
             {
                 { "controller", "Account" },
                 { "action", "Login" },
                 { "area", "Admin" },
             });
            }
        }
    }
}
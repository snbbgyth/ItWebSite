using System.Web.Mvc;

namespace ItWebSite.Web.Areas.Admin
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_Default",
                "Admin/{controller}/{action}/{id}",
                new {controller = "News", action = "Index", area = "Admin", id = ""},
                new[] { "ItWebSite.Web.Areas.Admin.Controllers" });
        }
    }
}
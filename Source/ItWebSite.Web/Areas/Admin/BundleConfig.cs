using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace ItWebSite.Web.Areas.Admin
{
    internal static class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        internal static void RegisterBundles(BundleCollection bundles, AreaRegistrationContext context, HttpContextBase httpContext)
        {
            ViewBundleRegistrar.RegisterViewBundlesForArea(bundles, context, httpContext);
        }
    }
}
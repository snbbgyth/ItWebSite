using System.Web.Mvc;
using ItWebSite.Web.DAL;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}
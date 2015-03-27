using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;

namespace ItWebSite.Web.Controllers
{
    public class HomeController : Controller
    {
        private IWebContentDal _webContentDal;
        private IWebContentTypeDal _webContentTypeDal;

        public HomeController()
        {
            _webContentDal = DependencyResolver.Current.GetService<IWebContentDal>();
            _webContentTypeDal = DependencyResolver.Current.GetService<IWebContentTypeDal>();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var webContentType =await _webContentTypeDal.FirstOrDefaultAsync(t => t.Name == "关于我们");
            if (webContentType == null) return View(new WebContent());
            var webContentList = await _webContentDal.QueryByFunAsync(t=>t.WebContentTypeId==webContentType.Id);
            if (!webContentList.Any()) return View(new WebContent());
            ViewBag.Title = "关于我们";
            return View(webContentList.First(t=>t.DisplayOrder== webContentList.Max(c => c.DisplayOrder)));
        }

        public ActionResult WebSiteProductIntro()
        {
            ViewBag.Title = "企业简介.";
            return View();
        }

        public ActionResult SoftwareProductIntro()
        {
  
            ViewBag.Title = "企业简介.";
            return View();
        }

        public ActionResult TrainProductIntro()
        {
            ViewBag.Title = "企业简介.";
            return View();
        }

        public async Task<ActionResult> CompanyIntro()
        {
            var webContentType = await _webContentTypeDal.FirstOrDefaultAsync(t => t.Name == "企业简介");
            if (webContentType == null) return View(new WebContent());
            var webContentList = await _webContentDal.QueryByFunAsync(t => t.WebContentTypeId == webContentType.Id);
            if (!webContentList.Any()) return View(new WebContent());
            ViewBag.Title = "企业简介";
            return View(webContentList.First(t => t.DisplayOrder == webContentList.Max(c => c.DisplayOrder)));
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "联系我们.";
            return View();
        }
    }
}
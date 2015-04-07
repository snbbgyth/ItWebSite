using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using NHibernate.Criterion;
using PagedList;

namespace ItWebSite.Web.Controllers
{
    public class HomeController : Controller
    {
        private static IWebContentDal _webContentDal;
        private static IWebContentTypeDal _webContentTypeDal;
        private static IBlogContentDal _blogContentDal;
        private static IBlogContentTypeDal _blogContentTypeDal;
        static HomeController()
        {
            _webContentDal = DependencyResolver.Current.GetService<IWebContentDal>();
            _webContentTypeDal = DependencyResolver.Current.GetService<IWebContentTypeDal>();
            _blogContentDal = DependencyResolver.Current.GetService<IBlogContentDal>();
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
        }

        public async Task<ActionResult> Index(string currentFilter, string searchString, int? page)
        {
            int pageSize = 20;
            if (searchString != null)
            {
                //page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            Expression<Func<BlogContent, bool>> wherExpression =null;
            if (!String.IsNullOrEmpty(searchString))
            {
                wherExpression = s => s.Content.IsLike(searchString) || s.Title.IsLike(searchString) || s.Creater.IsLike(searchString) || s.LastModifier.IsLike(searchString);
            }
            int pageNumber = (page ?? 1);
            ViewBag.CurrentPageIndex = pageNumber;

            ViewBag.LastPageIndex = (await _blogContentDal.QueryCountAsync()) / pageSize;
            ViewBag.CurrentFilter = searchString;
            var entityList = await _blogContentDal.QueryPageAsync(wherExpression, t => t.Id, false, pageNumber, pageSize);
            return View(entityList);
        }


        public async Task<ActionResult> About()
        {
            var webContentType = await _webContentTypeDal.FirstOrDefaultAsync(t => t.Name == "关于我们");
            if (webContentType == null) return View(new WebContent());
            var webContentList = await _webContentDal.QueryByFunAsync(t => t.WebContentTypeId == webContentType.Id);
            if (!webContentList.Any()) return View(new WebContent());
            ViewBag.Title = "关于我们";
            return View(webContentList.First(t => t.DisplayOrder == webContentList.Max(c => c.DisplayOrder)));
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
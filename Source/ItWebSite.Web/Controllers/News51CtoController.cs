using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.Areas.Admin.Models;
using NHibernate.Criterion;

namespace ItWebSite.Web.Controllers
{
    public class News51CtoController : Controller
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        private static  INews51CtoDal _news51CtoDal;

        static News51CtoController()
        {
            _news51CtoDal = DependencyResolver.Current.GetService<INews51CtoDal>();
        }

 
        // GET: News51Cto
        public async Task<ActionResult> Index(string currentFilter, string searchString, int? page)
        {
            int pageSize = 20;
            if (searchString == null)
            {
                searchString = currentFilter;
            }
            Expression<Func<News51Cto, bool>> wherExpression = null;
            if (!String.IsNullOrEmpty(searchString))
            {
                wherExpression = s => s.Content.IsLike(searchString) || s.Title.IsLike(searchString) || s.Creater.IsLike(searchString) || s.LastModifier.IsLike(searchString);
            }
            int pageNumber = (page ?? 1);
            ViewBag.CurrentPageIndex = pageNumber;

            ViewBag.LastPageIndex = (await _news51CtoDal.QueryCountAsync()) / pageSize;
            ViewBag.CurrentFilter = searchString;
            var entityList = await _news51CtoDal.QueryPageAsync(wherExpression, t => t.LastModifyDate, false, pageNumber, pageSize);

            return View(entityList);
        }

        // GET: /News/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var news = await _news51CtoDal.QueryByIdAsync(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            var nextEntity = await _news51CtoDal.QueryByIdAsync(id + 1);
            if (nextEntity != null)
            {
                ViewBag.NextId = nextEntity.Id;
                ViewBag.NextTitle = nextEntity.Title;
            }

            var previousEntity = await _news51CtoDal.QueryByIdAsync(id - 1);
            if (previousEntity != null)
            {
                ViewBag.PriviousId = previousEntity.Id;
                ViewBag.PriviousTitle = previousEntity.Title;
            }
            return View(news);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            
            }
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.DAL.Manage;
using NHibernate.Criterion;
using PagedList;

namespace ItWebSite.Web.Controllers
{
    public class NewsCsdnController : Controller
    {
        private INewsCsdnDal _newsDal;

        public NewsCsdnController()
        {
            _newsDal = DependencyResolver.Current.GetService<INewsCsdnDal>();
        }

        public async Task<ActionResult> ShowIndex(int id, string currentFilter, string searchString, int? page)
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

            Expression<Func<NewsCsdn, bool>> wherExpression = t => t.NewsTypeId==id;
            if (!String.IsNullOrEmpty(searchString))
            {
                wherExpression = s => s.Content.IsLike(searchString) || s.Title.IsLike(searchString) || s.Creater.IsLike(searchString) || s.LastModifier.IsLike(searchString);
            }
            int pageNumber = (page ?? 1);
            ViewBag.CurrentPageIndex = pageNumber;

            ViewBag.LastPageIndex = (await _newsDal.QueryCountAsync()) / pageSize;
            ViewBag.CurrentFilter = searchString;
            var entityList = await _newsDal.QueryPageAsync(wherExpression, t => t.LastModifyDate, false, pageNumber, pageSize);
            ViewBag.Title = NewsManage.QueryNewsTypeNameById(id);
            ViewBag.NewsTypeId = id;
            return  View(entityList);
        }


        public async Task<ActionResult> Index( string currentFilter, string searchString, int? page)
        {
            int pageSize = 20;
            if (searchString == null)
            {
                searchString = currentFilter;
            }
            Expression<Func<NewsCsdn, bool>> wherExpression = null;
            if (!String.IsNullOrEmpty(searchString))
            {
                wherExpression = s => s.Content.IsLike(searchString) || s.Title.IsLike(searchString) || s.Creater.IsLike(searchString) || s.LastModifier.IsLike(searchString);
            }
            int pageNumber = (page ?? 1);
            ViewBag.CurrentPageIndex = pageNumber;

            ViewBag.LastPageIndex = (await _newsDal.QueryCountAsync()) / pageSize;
            ViewBag.CurrentFilter = searchString;
            var entityList = await _newsDal.QueryPageAsync(wherExpression, t => t.LastModifyDate, false, pageNumber, pageSize);
 
            return View(entityList);
        }

        // GET: /News/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var news = await _newsDal.QueryByIdAsync(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            var nextEntity = await _newsDal.QueryByIdAsync(id + 1);
            if (nextEntity != null)
            {
                ViewBag.NextId = nextEntity.Id;
                ViewBag.NextTitle = nextEntity.Title;
            }

            var previousEntity = await _newsDal.QueryByIdAsync(id - 1);
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

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
    public class NewsController : Controller
    {
        private INewsDal _newsDal;

        public NewsController()
        {
            _newsDal = DependencyResolver.Current.GetService<INewsDal>();
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

            Expression<Func<News, bool>> wherExpression = t => t.NewsTypeId==id;
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

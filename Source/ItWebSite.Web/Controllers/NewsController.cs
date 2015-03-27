using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.DAL.Manage;
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
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            IEnumerable<News> entityList = await _newsDal.QueryByFunAsync(t => t.NewsType.Id == id);
            if (entityList.Any())
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    entityList = entityList.Where(s => (s.Content != null && s.Content.Contains(searchString))
                                                                                          || (s.Title != null && s.Title.Contains(searchString))
                                                                                          || (s.Creater != null && s.Creater.Contains(searchString))
                                                                                          || (s.LastModifier != null && s.LastModifier.Contains(searchString)));

                }
                entityList = entityList.OrderByDescending(s => s.LastModifyDate);
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            ViewBag.Title = NewsManage.QueryNewsTypeNameById(id);
            ViewBag.NewsTypeId = id;
            return View(entityList.ToPagedList(pageNumber, pageSize));
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

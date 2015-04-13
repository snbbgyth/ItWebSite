using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.DAL.Manage;
using ItWebSite.Web.DAL.MySql;
using PagedList;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "Admin")]
    public class NewsTypesController : Controller
    {
        private INewsTypeDal _newsTypeDal;

        public NewsTypesController()
        {
            _newsTypeDal = DependencyResolver.Current.GetService<INewsTypeDal>();
        }

        // GET: NewsTypes
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            IEnumerable<NewsType> entityList = await _newsTypeDal.QueryAllAsync();
            if (entityList.Any())
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    entityList = entityList.Where(s => s.Name.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        entityList = entityList.OrderByDescending(s => s.Name);
                        break;
                    default: // Name ascending 
                        entityList = entityList.OrderBy(s => s.Name);
                        break;
                }
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(entityList.ToPagedList(pageNumber, pageSize));
        }

        // GET: NewsTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsType newsType = await _newsTypeDal.QueryByIdAsync(id);
            if (newsType == null)
            {
                return HttpNotFound();
            }
            return View(newsType);
        }

        // GET: NewsTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NewsTypes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewsType newsType)
        {
            //if (ModelState.IsValid)
            //{
            newsType.Creater = User.Identity.Name;
            newsType.LastModifier = User.Identity.Name;
            await _newsTypeDal.InsertAsync(newsType);
            NewsManage.RefreshNewsType();
            return RedirectToAction("Index");
            //}
            //return View(newsType);
        }

        // GET: NewsTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsType newsType = await _newsTypeDal.QueryByIdAsync(id);
            if (newsType == null)
            {
                return HttpNotFound();
            }
            return View(newsType);
        }

        // POST: NewsTypes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] NewsType newsType)
        {
            newsType.LastModifier = User.Identity.Name;
            await _newsTypeDal.ModifyAsync(newsType);
            NewsManage.RefreshNewsType();
            return RedirectToAction("Index");
        }

        // GET: NewsTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var result = await _newsTypeDal.DeleteByIdAsync(id);
            if (result == 0)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index");
        }

        // POST: NewsTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _newsTypeDal.DeleteByIdAsync(id);
            NewsManage.RefreshNewsType();
            return RedirectToAction("Index");
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

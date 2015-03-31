using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.Areas.Admin.Models;
using ItWebSite.Web.DAL;
using ItWebSite.Web.DAL.MySql;
using PagedList;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "Admin,Edit")]
    public class BlogContentTypesController : BaseController
    {
        private static IBlogContentTypeDal _blogContentTypeDal;

        static BlogContentTypesController()
        {
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var entityList = await _blogContentTypeDal.QueryAllAsync();
                       if (entityList.Any())
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    entityList = entityList.Where(s => (s.Name != null && s.Name.Contains(searchString))
                                                                      || (s.Creater != null && s.Creater.Contains(searchString))
                                                                      || (s.LastModifier != null && s.LastModifier.Contains(searchString)));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        entityList = entityList.OrderByDescending(s => s.Name);
                        break;
                    case "Date":
                        entityList = entityList.OrderBy(s => s.LastModifyDate);
                        break;
                    case "date_desc":
                        entityList = entityList.OrderByDescending(s => s.LastModifyDate);
                        break;
                    default:
                        entityList = entityList.OrderBy(s => s.Name);
                        break;
                }
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(entityList.ToPagedList(pageNumber, pageSize));
        }

       

        // GET: Admin/BlogContentTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogContentType blogContentType = await _blogContentTypeDal.QueryByIdAsync(id);
            if (blogContentType == null)
            {
                return HttpNotFound();
            }
            return View(blogContentType);
        }

        // GET: Admin/BlogContentTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/BlogContentTypes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name")] BlogContentType blogContentType)
        {
            if (ModelState.IsValid)
            {
                InitInsert(blogContentType);
                await _blogContentTypeDal.InsertAsync(blogContentType);
                return RedirectToAction("Index");
            }

            return View(blogContentType);
        }

        // GET: Admin/BlogContentTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogContentType blogContentType = await _blogContentTypeDal.QueryByIdAsync(id);
            if (blogContentType == null)
            {
                return HttpNotFound();
            }
            return View(blogContentType);
        }

        // POST: Admin/BlogContentTypes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BlogContentType blogContentType)
        {
            if (!string.IsNullOrEmpty(blogContentType.Name))
            {
                await _blogContentTypeDal.ModifyAsync(blogContentType);
                return RedirectToAction("Index");
            }
            return View(blogContentType);
        }

        // GET: Admin/BlogContentTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogContentType blogContentType = await _blogContentTypeDal.QueryByIdAsync(id);
            if (blogContentType == null)
            {
                return HttpNotFound();
            }
            return View(blogContentType);
        }

        // POST: Admin/BlogContentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _blogContentTypeDal.DeleteByIdAsync(id);
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

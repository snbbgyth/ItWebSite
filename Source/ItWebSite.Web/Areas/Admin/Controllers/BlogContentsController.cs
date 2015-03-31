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
using PagedList;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    public class BlogContentsController : BaseController
    {
        private static IBlogContentDal _blogContentDal;
        private static IBlogContentTypeDal _blogContentTypeDal;

        static BlogContentsController()
        {
            _blogContentDal = DependencyResolver.Current.GetService<IBlogContentDal>();
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
        }

        public async Task<ActionResult> Index(string currentFilter, string searchString, int? page)
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
            var entityList = await _blogContentDal.QueryAllAsync();
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
            return View(entityList.ToPagedList(pageNumber, pageSize));
        }
 

        // GET: Admin/BlogContents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogContent blogContent = await _blogContentDal.QueryByIdAsync(id);
            if (blogContent == null)
            {
                return HttpNotFound();
            }
            return View(blogContent);
        }

        // GET: Admin/BlogContents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/BlogContents/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,WebContentTypeId,DisplayOrder,Title,BlogFrom,BlogFromUrl,Content,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] BlogContent blogContent)
        {
            if (ModelState.IsValid)
            {
                InitInsert(blogContent);
                await _blogContentDal.InsertAsync(blogContent);
                return RedirectToAction("Index");
            }

            return View(blogContent);
        }

        // GET: Admin/BlogContents/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogContent blogContent = await _blogContentDal.QueryByIdAsync(id);
            if (blogContent == null)
            {
                return HttpNotFound();
            }
            return View(blogContent);
        }

        // POST: Admin/BlogContents/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,WebContentTypeId,DisplayOrder,Title,BlogFrom,BlogFromUrl,Content,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] BlogContent blogContent)
        {
            if (ModelState.IsValid)
            {
                InitModify(blogContent);
                await _blogContentDal.ModifyAsync(blogContent);
                return RedirectToAction("Index");
            }
            return View(blogContent);
        }

        // GET: Admin/BlogContents/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogContent blogContent = await _blogContentDal.QueryByIdAsync(id);
            if (blogContent == null)
            {
                return HttpNotFound();
            }
            return View(blogContent);
        }

        // POST: Admin/BlogContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
 
            await _blogContentDal.DeleteByIdAsync(id);
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

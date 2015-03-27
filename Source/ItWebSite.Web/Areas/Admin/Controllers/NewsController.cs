using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.DAL;
using ItWebSite.Web.DAL.MySql;
using PagedList;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "Admin,Edit")]
    public class NewsController : BaseController
    {
        private INewsDal _newsDal;
        private INewsTypeDal _newsTypeDal;

        public NewsController()
        {
            _newsDal = DependencyResolver.Current.GetService<INewsDal>();
            _newsTypeDal = DependencyResolver.Current.GetService<INewsTypeDal>();
        }
        // GET: /News/
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
            IEnumerable<News> entityList = await _newsDal.QueryAllAsync();
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

        // GET: /News/Create
        public async Task<ActionResult> Create()
        {
            var news = new News();
            news.NewsTypeList = await _newsTypeDal.QueryAllAsync();
            return View(news);
        }

        // POST: /News/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Content,IsPublish,NewsType,NewsTypeList,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] News news)
        {
            if (ModelState.IsValid)
            {
                InitInsert(news);
                await _newsDal.InsertAsync(news);
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: /News/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            news.NewsTypeList = await _newsTypeDal.QueryAllAsync();
            return View(news);
        }

        // POST: /News/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Content,IsPublish,NewsType,NewsTypeList,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] News news)
        {
            if (ModelState.IsValid)
            {
                news.LastModifier = User.Identity.Name;
                await _newsDal.ModifyAsync(news);
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: /News/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = await _newsDal.QueryByIdAsync(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: /News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _newsDal.DeleteByIdAsync(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

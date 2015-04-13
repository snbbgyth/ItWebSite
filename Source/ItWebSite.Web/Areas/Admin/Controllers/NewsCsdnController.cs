using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.DAL;
using ItWebSite.Web.DAL.MySql;
using NHibernate.Criterion;
using PagedList;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "Admin,Edit")]
    public class NewsCsdnController : BaseController
    {
        private INewsCsdnDal _newsDal;
        private INewsTypeDal _newsTypeDal;

        public NewsCsdnController()
        {
            _newsDal = DependencyResolver.Current.GetService<INewsCsdnDal>();
            _newsTypeDal = DependencyResolver.Current.GetService<INewsTypeDal>();
        }
        // GET: /News/
        public async Task<ActionResult> Index(string currentFilter, string searchString, int? page)
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
            return View(news);
        }

        // GET: /News/Create
        public async Task<ActionResult> Create()
        {
            var news = new NewsCsdn();
            news.NewsTypeList = await _newsTypeDal.QueryAllAsync();
            return View(news);
        }

        // POST: /News/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Content,IsPublish,NewsType,NewsTypeList,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] NewsCsdn news)
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Content,IsPublish,NewsType,NewsTypeList,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] NewsCsdn news)
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
            NewsCsdn news = await _newsDal.QueryByIdAsync(id);
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

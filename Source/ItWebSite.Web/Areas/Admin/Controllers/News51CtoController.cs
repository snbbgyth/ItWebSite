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
using ItWebSite.Web.DAL;
using NHibernate.Criterion;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    public class News51CtoController : BaseController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        private static INews51CtoDal _news51CtoDal;

        private static INewsTypeDal _newsTypeDal;

        static News51CtoController()
        {
            _news51CtoDal = DependencyResolver.Current.GetService<INews51CtoDal>();
            _newsTypeDal = DependencyResolver.Current.GetService<INewsTypeDal>();
        }

        // GET: Admin/News51Cto
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

        // GET: Admin/News51Cto/Details/5
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

        // GET: Admin/News51Cto/Create
        public async Task<ActionResult> Create()
        {
            var news51Cto = new News51Cto();
            news51Cto.NewsTypeList = await _newsTypeDal.QueryAllAsync();
            return View(news51Cto);
        }

        // POST: /News/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(  News51Cto news51Cto)
        {
            if (ModelState.IsValid)
            {
                InitInsert(news51Cto);
                await _news51CtoDal.InsertAsync(news51Cto);
                return RedirectToAction("Index");
            }
            return View(news51Cto);
        }

        // GET: /News/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            news.NewsTypeList = await _newsTypeDal.QueryAllAsync();
            return View(news);
        }

        // POST: /News/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(  News51Cto news51Cto)
        {
            if (ModelState.IsValid)
            {
                news51Cto.LastModifier = User.Identity.Name;
                await _news51CtoDal.ModifyAsync(news51Cto);
                return RedirectToAction("Index");
            }
            return View(news51Cto);
        }

        // GET: /News/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var news51Cto = await _news51CtoDal.QueryByIdAsync(id);
            if (news51Cto == null)
            {
                return HttpNotFound();
            }
            return View(news51Cto);
        }

        // POST: /News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _news51CtoDal.DeleteByIdAsync(id);
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

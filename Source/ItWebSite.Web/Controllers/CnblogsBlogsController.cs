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
using ItWebSite.Core.Model;
using ItWebSite.Web.Areas.Admin.Models;
using ItWebSite.Web.DAL.Manage;
using NHibernate.Criterion;
using PagedList;

namespace ItWebSite.Web.Controllers
{
    public class CnblogsBlogsController : Controller
    {
        private static ICnblogsBlogDal _blogContentDal;
        private static IBlogContentTypeDal _blogContentTypeDal;
        private static ICnblogsCommentDal _cnblogsCommentDal;

        static CnblogsBlogsController()
        {
            _cnblogsCommentDal = DependencyResolver.Current.GetService<ICnblogsCommentDal>();
            _blogContentDal = DependencyResolver.Current.GetService<ICnblogsBlogDal>();
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
        }

        public async Task<ActionResult> Index(string currentFilter, string searchString, int? page)
        {
            int pageSize = 20;
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            Expression<Func<CnblogsBlog, bool>> wherExpression = t => t.Id > 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                wherExpression = s => s.Content.IsLike(searchString) || s.Title.IsLike(searchString) || s.Creater.IsLike(searchString) || s.LastModifier.IsLike(searchString);
            }
            int pageNumber = (page ?? 1);
            ViewBag.CurrentPageIndex = pageNumber;
            ViewBag.LastPageIndex = (await _blogContentDal.QueryCountAsync()) / pageSize;
            ViewBag.CurrentFilter = searchString;
            var entityList = await _blogContentDal.QueryPageAsync(wherExpression, t => t.LastModifyDate, false, pageNumber, pageSize);
            return View(entityList);
        }

        // GET: Admin/BlogContents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var  entity = await BlogManage.QueryCnBlogViewByIdAsync((int)id);
            var nextEntity = await _blogContentDal.QueryByIdAsync(id + 1);
            if (nextEntity != null)
            {
                ViewBag.NextId = nextEntity.Id;
                ViewBag.NextTitle = nextEntity.Title;
            }
            var previousEntity = await _blogContentDal.QueryByIdAsync(id - 1);
            if (previousEntity != null)
            {
                ViewBag.PriviousId = previousEntity.Id;
                ViewBag.PriviousTitle = previousEntity.Title;
            }
            return View(entity);
        }

        [HttpPost]
        public async Task<ActionResult> CreateComment(CnblogsComment cnblogsComment)
        {
            if (!string.IsNullOrEmpty(cnblogsComment.Content))
            {
                InitInsert(cnblogsComment);
                await _cnblogsCommentDal.InsertAsync(cnblogsComment);
            }
            return RedirectToAction("Details", new { id = cnblogsComment.CnBlogsId });
        }

        public void InitInsert(CnblogsComment entity) 
        {
            entity.Creater = User.Identity.Name;
            entity.LastModifier = User.Identity.Name;
            entity.CreateDate = DateTime.Now;
            entity.LastModifyDate = DateTime.Now;
        }

    }
}

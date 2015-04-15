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
using ItWebSite.Web.DAL.Manage;
using NHibernate.Criterion;

namespace ItWebSite.Web.Controllers
{
    public class CsdnBlogsController: Controller
    {
        private static ICsdnBlogDal _csdnBlogDal;
        private static IBlogContentTypeDal _blogContentTypeDal;
        private static ICsdnBlogCommentDal _blogCommentDal;

        static CsdnBlogsController()
        {
            _csdnBlogDal = DependencyResolver.Current.GetService<ICsdnBlogDal>();
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
            _blogCommentDal = DependencyResolver.Current.GetService<ICsdnBlogCommentDal>();
        }

        public async Task<ActionResult> Index(string currentFilter, string searchString, int? page)
        {
            int pageSize = 20;
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            Expression<Func<CsdnBlog, bool>> wherExpression = t => t.Id > 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                wherExpression = s => s.Content.IsLike(searchString) || s.Title.IsLike(searchString) || s.Creater.IsLike(searchString) || s.LastModifier.IsLike(searchString);
            }
            int pageNumber = (page ?? 1);
            ViewBag.CurrentPageIndex = pageNumber;
            ViewBag.LastPageIndex =(await  _csdnBlogDal.QueryCountAsync())/pageSize ;
            ViewBag.CurrentFilter = searchString;
            var entityList = await _csdnBlogDal.QueryPageAsync(wherExpression, t => t.LastModifyDate, false, pageNumber, pageSize);
            return View(entityList);
        }

 

        // GET: Admin/BlogContents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entity = await BlogManage.QueryCsdnBlogViewByIdAsync((int)id);
            var nextEntity = await _csdnBlogDal.QueryByIdAsync(id+1);
            if (nextEntity != null)
            {
                ViewBag.NextId = nextEntity.Id;
                ViewBag.NextTitle = nextEntity.Title;
            }
            var previousEntity = await _csdnBlogDal.QueryByIdAsync(id-1);
            if (previousEntity != null)
            {
                ViewBag.PriviousId = previousEntity.Id;
                ViewBag.PriviousTitle = previousEntity.Title;
            }
            return View(entity);
        }

        [HttpPost]
        public async Task<ActionResult> CreateComment(CsdnBlogComment csdnblogComment)
        {
            if (!string.IsNullOrEmpty(csdnblogComment.Content))
            {
                InitInsert(csdnblogComment);
                await _blogCommentDal.InsertAsync(csdnblogComment);
            }
            return RedirectToAction("Details", new { id = csdnblogComment.CsdnBlogId });
        }

        public void InitInsert(CsdnBlogComment entity)
        {
            entity.Creater = User.Identity.Name;
            entity.LastModifier = User.Identity.Name;
            entity.CreateDate = DateTime.Now;
            entity.LastModifyDate = DateTime.Now;
        }
    }
}

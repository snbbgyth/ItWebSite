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
using NHibernate.Criterion;

namespace ItWebSite.Web.Controllers
{
    public class CsdnBlogsController: Controller
    {
        private static ICsdnBlogDal _blogContentDal;
        private static IBlogContentTypeDal _blogContentTypeDal;

        static CsdnBlogsController()
        {
            _blogContentDal = DependencyResolver.Current.GetService<ICsdnBlogDal>();
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
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
            ViewBag.LastPageIndex =(await  _blogContentDal.QueryCountAsync())/pageSize ;
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
            CsdnBlog blogContent = await _blogContentDal.QueryByIdAsync(id);
            if (blogContent == null)
            {
                return HttpNotFound();
            }
            
            var nextEntity = await _blogContentDal.QueryByIdAsync(id+1);
            if (nextEntity != null)
            {
                ViewBag.NextId = nextEntity.Id;
                ViewBag.NextTitle = nextEntity.Title;
            }

            var previousEntity = await _blogContentDal.QueryByIdAsync(id-1);
            if (previousEntity != null)
            {
                ViewBag.PriviousId = previousEntity.Id;
                ViewBag.PriviousTitle = previousEntity.Title;
            }
            blogContent.BlogContentType = await _blogContentTypeDal.QueryByIdAsync(id);
            return View(blogContent);
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

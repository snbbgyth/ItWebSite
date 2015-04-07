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
using PagedList;

namespace ItWebSite.Web.Controllers
{
    public class BlogContentsController : Controller
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
            int pageSize = 20;
            if (searchString != null)
            {
                //page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            Expression<Func<BlogContent, bool>> wherExpression = t => t.Id > 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                wherExpression = s => s.Content.IsLike(searchString) || s.Title.IsLike(searchString) || s.Creater.IsLike(searchString) || s.LastModifier.IsLike(searchString);
            }
            int pageNumber = (page ?? 1);
            ViewBag.CurrentPageIndex = pageNumber;
          
            ViewBag.LastPageIndex =   _blogContentDal.QueryCount()/pageSize ;
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
            BlogContent blogContent = await _blogContentDal.QueryByIdAsync(id);
            if (blogContent == null)
            {
                return HttpNotFound();
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

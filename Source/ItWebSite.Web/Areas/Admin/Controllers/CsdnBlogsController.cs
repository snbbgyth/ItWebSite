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
using ItWebSite.Web.DAL.Manage;
using NHibernate.Criterion;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    public class CsdnBlogsController : BaseController
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
            ViewBag.LastPageIndex = (await _csdnBlogDal.QueryCountAsync()) / pageSize;
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
            var  entity = await BlogManage.QueryCsdnBlogViewByIdAsync((int)id);
            return View(entity);
        }

        // GET: Admin/BlogContents/Create
        public async Task<ActionResult> Create()
        {
            var blogContent = new CnblogsBlog();
            blogContent.BlogContentTypeList = await _blogContentTypeDal.QueryAllAsync();
            return View(blogContent);
        }

        // POST: Admin/BlogContents/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CsdnBlog blogContent)
        {
            try
            {
                InitInsert(blogContent);
                await _csdnBlogDal.InsertAsync(blogContent);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(blogContent);
            }
        }

        // GET: Admin/BlogContents/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CsdnBlog blogContent = await _csdnBlogDal.QueryByIdAsync(id);
            if (blogContent == null)
            {
                return HttpNotFound();
            }
            blogContent.BlogContentType = await _blogContentTypeDal.QueryByIdAsync(blogContent.BlogContentTypeId);
            blogContent.BlogContentTypeList = await _blogContentTypeDal.QueryAllAsync();
            return View(blogContent);
        }

        // POST: Admin/BlogContents/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CsdnBlog blogContent)
        {
            try
            {
                InitModify(blogContent);
                await _csdnBlogDal.ModifyAsync(blogContent);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(blogContent);
            }
        }

        // GET: Admin/BlogContents/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CsdnBlog blogContent = await _csdnBlogDal.QueryByIdAsync(id);
            if (blogContent == null)
            {
                return HttpNotFound();
            }
            blogContent.BlogContentType = await _blogContentTypeDal.QueryByIdAsync(blogContent.BlogContentTypeId);
            return View(blogContent);
        }

        // POST: Admin/BlogContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            await _csdnBlogDal.DeleteByIdAsync(id);
            return RedirectToAction("Index");
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

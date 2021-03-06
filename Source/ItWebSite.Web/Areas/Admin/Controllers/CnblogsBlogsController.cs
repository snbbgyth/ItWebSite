﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.Areas.Admin.Models;
using ItWebSite.Web.DAL;
using ItWebSite.Web.DAL.Manage;
using PagedList;
using WebGrease.Css.Extensions;
using NHibernate.Criterion;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    public class CnblogsBlogsController : BaseController
    {
        private static ICnblogsBlogDal _blogContentDal;
        private static IBlogContentTypeDal _blogContentTypeDal;
        private static ICnblogsCommentDal _cnblogsCommentDal;

        static CnblogsBlogsController()
        {
            _blogContentDal = DependencyResolver.Current.GetService<ICnblogsBlogDal>();
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
            _cnblogsCommentDal = DependencyResolver.Current.GetService<ICnblogsCommentDal>();
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
            var blogContent = await BlogManage.QueryCnBlogViewByIdAsync((int)id);
            return View(blogContent);
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
        public async Task<ActionResult> Create(CnblogsBlog blogContent)
        {
            try
            {
                InitInsert(blogContent);
                await _blogContentDal.InsertAsync(blogContent);
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
            CnblogsBlog blogContent = await _blogContentDal.QueryByIdAsync(id);
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
        public async Task<ActionResult> Edit(CnblogsBlog blogContent)
        {
            try
            {
                InitModify(blogContent);
                await _blogContentDal.ModifyAsync(blogContent);
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
            CnblogsBlog blogContent = await _blogContentDal.QueryByIdAsync(id);
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
            await _blogContentDal.DeleteByIdAsync(id);
            return RedirectToAction("Index");
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

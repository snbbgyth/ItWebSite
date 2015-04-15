using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.Models;

namespace ItWebSite.Web.DAL.Manage
{
    public class BlogManage
    {
        private static ICnblogsBlogDal _cnblogsBlogDal;

        private static ICnblogsCommentDal _cnblogsCommentDal;

        private static IBlogContentTypeDal _blogContentTypeDal;

        private static ICsdnBlogDal _csdnBlogDal;

        private static ICsdnBlogCommentDal _csdnBlogCommentDal;

        static BlogManage()
        {
            _cnblogsBlogDal = DependencyResolver.Current.GetService<ICnblogsBlogDal>();
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
            _csdnBlogDal = DependencyResolver.Current.GetService<ICsdnBlogDal>();
            _cnblogsCommentDal = DependencyResolver.Current.GetService<ICnblogsCommentDal>();
            _csdnBlogCommentDal = DependencyResolver.Current.GetService<ICsdnBlogCommentDal>();
        }

        public static IEnumerable<CnblogsBlog> QueryTopCnBlogs(int count)
        {
            return _cnblogsBlogDal.QueryLast(count);
        }

        public static IEnumerable<CsdnBlog> QueryTopCsdnBlogs(int count)
        {
            return _csdnBlogDal.QueryLast(count);
        }

        public static async Task<CnblogsBlogViewModel> QueryCnBlogViewByIdAsync(int id)
        {
            var entity = new CnblogsBlogViewModel();
            entity.CnblogsBlog = await _cnblogsBlogDal.QueryByIdAsync(id);
            entity.CnblogsBlog.BlogContentType = await _blogContentTypeDal.QueryByIdAsync(id);
            entity.CnblogsComments = await _cnblogsCommentDal.QueryByFunAsync(t => t.CnBlogsId == id);
            return entity;
        }

        public static async Task<CsdnBlogViewModel> QueryCsdnBlogViewByIdAsync(int id)
        {
            var entity = new CsdnBlogViewModel();
            entity.CsdnBlog = await _csdnBlogDal.QueryByIdAsync(id);
            entity.CsdnBlog.BlogContentType = await _blogContentTypeDal.QueryByIdAsync(id);
            entity.CsdnblogComments = await _csdnBlogCommentDal.QueryByFunAsync(t => t.CsdnBlogId == id);
            return entity;
        }
    }
}
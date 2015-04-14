using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;

namespace ItWebSite.Web.DAL.Manage
{
    public class BlogContentManage
    {
        private static ICnblogsBlogDal _cnblogsBlogDal;

        private static IBlogContentTypeDal _blogContentTypeDal;

        private static ICsdnBlogDal _csdnBlogDal;

        static BlogContentManage()
        {
            _cnblogsBlogDal = DependencyResolver.Current.GetService<ICnblogsBlogDal>();
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
            _csdnBlogDal = DependencyResolver.Current.GetService<ICsdnBlogDal>();
        }

        public static IEnumerable<CnblogsBlog> QueryTopCnBlogs(int count)
        {
            return _cnblogsBlogDal.QueryLast(count);
        }

        public static IEnumerable<CsdnBlog> QueryTopCsdnBlogs(int count)
        {
            return _csdnBlogDal.QueryLast(count);
        }

    }
}
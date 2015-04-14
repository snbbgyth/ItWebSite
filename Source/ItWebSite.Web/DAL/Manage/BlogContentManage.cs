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
        private static IBlogContentDal _blogContentDal;

        private static IBlogContentTypeDal _blogContentTypeDal;

        static BlogContentManage()
        {
            _blogContentDal = DependencyResolver.Current.GetService<IBlogContentDal>();
            _blogContentTypeDal = DependencyResolver.Current.GetService<IBlogContentTypeDal>();
        }

        public static IEnumerable<BlogContent> QueryTopCsdnBlogs(int count)
        {
            return _blogContentDal.QueryLast(count);
        }

    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;

namespace ItWebSite.Web.DAL.Manage
{
    public static  class NewsManage
    {
        private static INewsCsdnDal _newsCsdnDal;
        private static INews51CtoDal _news51CtoDal;
        private static INewsTypeDal _newsTypeDal;

        static NewsManage()
        {
            _newsCsdnDal = DependencyResolver.Current.GetService<INewsCsdnDal>();
            _newsTypeDal = DependencyResolver.Current.GetService<INewsTypeDal>();
            _news51CtoDal = DependencyResolver.Current.GetService<INews51CtoDal>();
            _newsTypeList = _newsTypeDal.QueryAll();
        }

        private static IEnumerable<NewsType> _newsTypeList;

        public static  IEnumerable<NewsType> QueryAllNewsTypes()
        {
            return _newsTypeList;
        }

        public static NewsType QueryNewsTypeById(int id)
        {
           return  _newsTypeDal.FirstOrDefault(t => t.Id == id);
        }

        public static string QueryNewsTypeNameById(int id)
        {
            var entity= _newsTypeDal.FirstOrDefault(t => t.Id == id);
            if (entity == null) return string.Empty;
            return entity.Name;
        }

        public static void RefreshNewsType()
        {
            Task.Factory.StartNew(NewTaskRefreshNewsType);
        }

        public static void NewTaskRefreshNewsType()
        {
            _newsTypeList = _newsTypeDal.QueryAll();
        }

        public static IEnumerable<News51Cto> QueryTop51CtoNews(int count)
        {
            return _news51CtoDal.QueryLast(count);
        }

        public static IEnumerable<NewsCsdn> QueryTopCsdnNews(int count)
        {
            return _newsCsdnDal.QueryLast(count);
        }
    }
}
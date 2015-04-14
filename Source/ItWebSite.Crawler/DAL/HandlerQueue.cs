using System;
using System.Reflection;
using System.Threading.Tasks;
using ItWebSite.Core.DAL;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.QueueDAL;
using ItWebSite.Crawler.Help;

namespace ItWebSite.Crawler.DAL
{
    public class HandlerQueue : BaseQueueDal<dynamic>
    {
        private static object _syncObj = new object();

        private static HandlerQueue _instance;

        private static ICnblogsBlogDal _blogContentDal;
        private static INewsCsdnDal _newsDal;
        private static INews51CtoDal _news51CtoDal;
        private static ICsdnBlogDal _iteyeBlogDal;

        public static HandlerQueue Instance
        {
            get
            {
                lock (_syncObj)
                {
                    if (_instance == null)
                        _instance = new HandlerQueue();
                }
                return _instance;
            }
        }

        private HandlerQueue()
        {
            _blogContentDal = Helper.Resolve<ICnblogsBlogDal>();
            _newsDal = Helper.Resolve<INewsCsdnDal>();
            _news51CtoDal = Helper.Resolve<INews51CtoDal>();
            _iteyeBlogDal = Helper.Resolve<ICsdnBlogDal>();
        }

        public override void OnNotify(dynamic entity)
        {
            if (entity is CnblogsBlog)
            {
                var blogContent = entity as CnblogsBlog;
                Task.Factory.StartNew(() => HandleBlogContent(blogContent));
            }
            if (entity is NewsCsdn)
            {
                var news = entity as NewsCsdn;
                Task.Factory.StartNew(() => HandleNews(news));
            }
            if (entity is News51Cto)
            {
                var news = entity as News51Cto;
                Task.Factory.StartNew(() => HandleNews51Cto(news));
            }
            if(entity is CsdnBlog)
            {
                var blog = entity as CsdnBlog;
                Task.Factory.StartNew(() => HandleCsdnBlog(blog));
            }
        }

        private void HandleNews(NewsCsdn entity)
        {
            try
            {
                if(!string.IsNullOrEmpty(entity.NewsFromUrl))
                _newsDal.DeleteByNewsFromUrl(entity.NewsFromUrl);
                _newsDal.Insert(entity);
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void HandleNews51Cto(News51Cto entity)
        {
            try
            {
                if (!string.IsNullOrEmpty(entity.NewsFromUrl))
                    _news51CtoDal.DeleteByNewsFromUrl(entity.NewsFromUrl);
                _news51CtoDal.Insert(entity);
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void HandleBlogContent(CnblogsBlog entity)
        {
            try
            {
                _blogContentDal.DeleteByBlogFromUrl(entity.BlogFromUrl);
                _blogContentDal.Insert(entity);
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void HandleCsdnBlog(CsdnBlog entity)
        {
            try
            {
                _iteyeBlogDal.DeleteByBlogFromUrl(entity.BlogFromUrl);
                _iteyeBlogDal.Insert(entity);
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
        }
    }
}

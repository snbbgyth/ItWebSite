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

        private static IBlogContentDal _blogContentDal;
        private static INewsCsdnDal _newsDal;
        private static INews51CtoDal _news51CtoDal;

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
            _blogContentDal = Helper.Resolve<IBlogContentDal>();
            _newsDal = Helper.Resolve<INewsCsdnDal>();
            _news51CtoDal = Helper.Resolve<INews51CtoDal>();
        }

        public override void OnNotify(dynamic entity)
        {
            if (entity is BlogContent)
            {
                var blogContent = entity as BlogContent;
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

        private void HandleBlogContent(BlogContent entity)
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
    }
}

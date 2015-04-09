using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ItWebSite.Core.DAL;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.QueueDAL;

namespace ItWebSite.Crawler
{
    public class HandlerQueue : BaseQueueDal<dynamic>
    {
        private static object _syncObj = new object();

        private static HandlerQueue _instance;

        private static IBlogContentDal _blogContentDal;
        private static INewsDal _newsDal;
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
            _blogContentDal = HandlerBlog.Resolve<IBlogContentDal>();
            _newsDal = HandlerBlog.Resolve<INewsDal>();
        }

        public override void OnNotify(dynamic entity)
        {
            if (entity is BlogContent)
            {
                var blogContent = entity as BlogContent;
                Task.Factory.StartNew(() => HandleBlogContent(blogContent));
            }
            if (entity is News)
            {
                var news = entity as News;
                Task.Factory.StartNew(() => HandleNews(news));
            }
        }

        private void HandleNews(News entity)
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

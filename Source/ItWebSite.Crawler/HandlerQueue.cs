﻿using System;
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
            _blogContentDal = Handler.Resolve<IBlogContentDal>();
        }

        public override void OnNotify(dynamic entity)
        {
            if (entity is BlogContent)
            {
             //Task.Factory.StartNew(()=>  HandleBlogContent(entity));
                HandleBlogContent(entity);
            }
        }

        private void HandleBlogContent(dynamic entity)
        {
            try
            {
                var blogContent = entity as BlogContent;
                _blogContentDal.DeleteByBlogFromUrl(blogContent.BlogFromUrl);
                _blogContentDal.Insert(blogContent);
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }

        }
    }
}

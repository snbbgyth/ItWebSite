using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItWebSite.Core.DAL;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;

namespace ItWebSite.Crawler
{
   public  class HandlerQueue:BaseQueueDal<dynamic>
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
               
           }
       }
   }
}

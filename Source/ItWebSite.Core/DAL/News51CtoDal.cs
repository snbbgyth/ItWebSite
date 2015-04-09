using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.QueueDAL;

namespace ItWebSite.Core.DAL
{
    public class News51CtoDal : DataOperationActivityBase<News51Cto>, INews51CtoDal
    {
        public virtual int DeleteByNewsFromUrl(string url)
        {
            int reslut = 0;

            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    session.Delete(string.Format("from {0} where NewsFromUrl = '{1}'", typeof(News51Cto).Name, url));
                    session.Flush();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return reslut;
        }

    }
}

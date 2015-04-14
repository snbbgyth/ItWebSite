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
    public class CnblogsBlogDal:DataOperationActivityBase<CnblogsBlog>,ICnblogsBlogDal
    {
        public virtual int DeleteByBlogFromUrl(string  url)
        {
            int reslut = 0;

            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    session.Delete(string.Format("from {0} where BlogFromUrl = '{1}'", typeof(CnblogsBlog).Name, url));
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

using System;
using System.Reflection;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.QueueDAL;

namespace ItWebSite.Core.DAL
{
    public class OrderItemDal : DataOperationActivityBase<OrderItem>, IOrderItemDal
    {
        public virtual int DeleteByOrderId(dynamic id)
        {
            int reslut = 0;

            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {

                    var queryString = string.Format(" delete {0} where OrderId = :id ", typeof(OrderItem).Name);
                    reslut = session.CreateQuery(queryString)
                                    .SetParameter("id", id)
                                    .ExecuteUpdate();
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

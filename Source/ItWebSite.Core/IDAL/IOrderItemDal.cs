using ItWebSite.Core.DbModel;

namespace ItWebSite.Core.IDAL
{
    public interface IOrderItemDal : IDataOperationActivity<OrderItem>
    {
        int DeleteByOrderId(dynamic id);
    }
}

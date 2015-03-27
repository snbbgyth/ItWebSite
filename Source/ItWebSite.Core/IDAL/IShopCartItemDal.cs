using ItWebSite.Core.DbModel;

namespace ItWebSite.Core.IDAL
{
    public interface IShopCartItemDal : IDataOperationActivity<ShopCartItem>
    {
        int SubmitById(int id);
    }
}

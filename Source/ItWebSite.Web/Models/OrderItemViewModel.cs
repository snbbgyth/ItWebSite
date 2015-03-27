using ItWebSite.Core.DbModel;

namespace ItWebSite.Web.Models
{
    public class OrderItemViewModel
    {
        public OrderItem OrderItem { get; set; }

        public ShopCartItem ShopCartItem { get; set; }
    }
}
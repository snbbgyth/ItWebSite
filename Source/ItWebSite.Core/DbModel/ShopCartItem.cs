using System.ComponentModel;
using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class ShopCartItem : BaseTable
    {
        public ShopCartItem()
        {
            Count = 1;
        }
        [DisplayName("用户名")]
        public virtual string CustomerName { get; set; }

        [DisplayName("数量")]
        public virtual int Count { get; set; }

        public virtual Product Product { get; set; }

        public virtual int ProductId { get; set; }

        [DisplayName("选择")]
        public virtual bool IsSubmit { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class Order : BaseTable
    {
        [DisplayName("用户名")]
        public virtual string CustomerName { get; set; }

        [Required]
        [DisplayName("联系电话")]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 11)]
        public virtual string CustomerPhone { get; set; }

        [Required]
        [DisplayName("联系地址")]
        [StringLength(1000, ErrorMessage = "请输入祥细的联系地址。", MinimumLength = 5)]
        public virtual string ReceiveAddress { get; set; }

        [DisplayName("总金额")]
        public virtual double TotalPrice { get; set; }

        [DisplayName("是否付款")]
        public virtual bool IsPay { get; set; }

        [DisplayName("订单编号")]
        public virtual string OrderNumber { get; set; }
    }
}

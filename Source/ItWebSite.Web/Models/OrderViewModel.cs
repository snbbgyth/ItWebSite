using System.Collections.Generic;
using ItWebSite.Core.DbModel;

namespace ItWebSite.Web.Models
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            Order=new Order();
            OrderItemViewList = new List<OrderItemViewModel>();
        }

        public Order Order { get; set; }

        public IList<OrderItemViewModel> OrderItemViewList { get; set; }
    }
}
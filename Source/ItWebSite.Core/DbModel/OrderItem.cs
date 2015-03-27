using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
   public  class OrderItem:BaseTable
    {
       
       public virtual Product Product { get; set; }

       public virtual int ProductId { get; set; }

       public virtual int Count { get; set; }

       public virtual double Price { get; set; }

       public virtual int OrderId { get; set; }

       public virtual double Total { get; set; }


    }
}

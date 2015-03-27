using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
    public class OrderMapping : ClassMap<Order>
    {
        public OrderMapping()
        {
            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate);
            Map(x => x.CustomerName);
            Map(x => x.CustomerPhone);
            Map(x => x.ReceiveAddress);
            Map(x => x.TotalPrice);
            Map(x => x.IsPay);
            Map(x => x.OrderNumber);

        }
    }
}

using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
    public class OrderItemMapping : ClassMap<OrderItem>
    {
        public OrderItemMapping()
        {
            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate);

            Map(x => x.Count);
            Map(x => x.Price);
            Map(x => x.OrderId);
            Map(x => x.Total);
            Map(x => x.ProductId);

        }
    }
}

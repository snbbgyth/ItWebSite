using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
    public class ShopCartItemMapping : ClassMap<ShopCartItem>
    {
        public ShopCartItemMapping()
        {
            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate);

            Map(x => x.CustomerName);
            Map(x => x.Count);
            Map(x => x.ProductId);
            Map(x => x.IsSubmit);
        }
    }
}

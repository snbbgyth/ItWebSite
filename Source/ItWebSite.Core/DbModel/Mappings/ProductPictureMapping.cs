using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
    public class ProductPictureMapping : ClassMap<ProductPicture>
    {
        public ProductPictureMapping()
        {
            Table("ProductPictureMap");

            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate);

            Map(x => x.DisplayOrder);
            Map(x => x.PictureId);
            Map(x => x.ProductId);

        }
    }
}

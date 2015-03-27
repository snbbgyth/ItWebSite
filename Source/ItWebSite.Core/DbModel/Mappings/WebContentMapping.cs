using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
    public class WebContentMapping : ClassMap<WebContent>
    {
        public WebContentMapping()
        {
            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate);

            Map(x => x.Content).Length(5000);
            Map(x => x.WebContentTypeId);
            Map(x => x.DisplayOrder);
        }
    }
}

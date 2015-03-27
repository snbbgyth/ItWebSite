using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
    public class PictureMapping : ClassMap<Picture>
    {
        public PictureMapping()
        {
            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate);

            Map(x => x.MimeType);
            Map(x => x.PictureBinary).Length(int.MaxValue);
            Map(x => x.FileName);

        }
    }
}

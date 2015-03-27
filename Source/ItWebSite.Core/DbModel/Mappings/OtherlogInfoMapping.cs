using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
    public class OtherlogInfoMapping : ClassMap<OtherLogInfo>
    {
        public OtherlogInfoMapping()
        {
            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate);

            Map(x => x.ClientName);
            Map(x => x.UniqueName);
            Map(x => x.TableName);
            Map(x => x.ClassName);
            Map(x => x.MethodName);
            Map(x => x.LogType);
            Map(x => x.Message);
            Map(x => x.StackTrace);
        }
    }
}

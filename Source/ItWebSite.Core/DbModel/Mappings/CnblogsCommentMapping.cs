using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
    public class CnblogsCommentMapping : ClassMap<CnblogsComment>
    {
        public CnblogsCommentMapping()
        {
            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate);

            Map(x => x.Content).Length(50000);
            Map(x => x.CnBlogsId).Index("CnBlogsId");
        }
    }
}

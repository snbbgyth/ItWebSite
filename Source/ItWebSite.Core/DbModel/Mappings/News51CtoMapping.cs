using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
   public class News51CtoMapping: ClassMap<News51Cto>
    {
       public News51CtoMapping()
        {
            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate).Index("LastModifyDate");

            Map(x => x.IsPublish);
            Map(x => x.Content).Length(int.MaxValue);
            Map(x => x.Title);
            Map(x => x.NewsTypeId);
            Map(x => x.NewsFrom);
            Map(x => x.NewsFromUrl).Index("NewsFromUrl");
            Map(x => x.DisplayOrder);
            Map(x => x.Summary).Length(1000);
        }
    }
}

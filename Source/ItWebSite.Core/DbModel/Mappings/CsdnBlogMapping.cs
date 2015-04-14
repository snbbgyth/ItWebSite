using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace ItWebSite.Core.DbModel.Mappings
{
   public class CsdnBlogMapping: ClassMap<CsdnBlog>
    {
       public CsdnBlogMapping()
        {
            Id(x => x.Id).UniqueKey("Id").GeneratedBy.Identity();
            Map(x => x.CreateDate);
            Map(x => x.Creater);
            Map(x => x.IsDelete);
            Map(x => x.LastModifier);
            Map(x => x.LastModifyDate).Index("LastModifyDate");

            Map(x => x.Content).Length(50000);
            Map(x => x.BlogContentTypeId);
            Map(x => x.DisplayOrder);
            Map(x => x.Title).Length(100);
            Map(x => x.BlogFrom).Length(200);
            Map(x => x.BlogFromUrl).Length(200).Index("UrlIndex");
        }
    }
}

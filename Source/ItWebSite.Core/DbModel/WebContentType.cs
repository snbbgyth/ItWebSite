using System.ComponentModel;
using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
   public class WebContentType:BaseTable
   {
       [DisplayName("类型名称")]
       public virtual string Name { get; set; }
    }
}

using System.ComponentModel;
using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class NewsType : BaseTable
    {
        [DisplayName("新闻类型")]
        public virtual string Name { get; set; }
    }
}

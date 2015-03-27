using System.ComponentModel;
using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class ProductType:BaseTable
    {
        [DisplayName("产品类型")]
        public virtual string Name { get; set; }
    }
}

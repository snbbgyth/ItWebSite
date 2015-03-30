using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public  class BlogContentType:BaseTable
    {
        [DisplayName("类型名称")]
        public virtual string Name { get; set; }
    }
}

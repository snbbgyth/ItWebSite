using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class CsdnBlogComment : BaseTable
    {

        public virtual int CsdnBlogId { get; set; }

        [AllowHtml]
        [DisplayName("回复内容")]
        public virtual string Content { get; set; }
    }
}

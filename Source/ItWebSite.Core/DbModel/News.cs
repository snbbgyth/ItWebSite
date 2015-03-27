using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class News : BaseTable
    {
        [DisplayName("标题")]
        public virtual string Title { get; set; }

        [AllowHtml]
        [DisplayName("正文")]
        public virtual string Content { get; set; }

        [DisplayName("是否发布")]
        public virtual bool IsPublish { get; set; }

        public virtual NewsType NewsType { get; set; }

        public virtual IEnumerable<NewsType> NewsTypeList { get; set; }

        public News()
        {

        }
    }
}

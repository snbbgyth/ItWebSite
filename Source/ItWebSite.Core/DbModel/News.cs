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

        [DisplayName("来源")]
        public virtual string NewsFrom { get; set; }

        [DisplayName("来源URL")]
        public virtual string NewsFromUrl { get; set; }

        /// <summary>
        /// Each content type just display max display order
        /// </summary>
        [DisplayName("显示顺序")]
        public virtual int DisplayOrder { get; set; }

        [DisplayName("是否发布")]
        public virtual bool IsPublish { get; set; }

        public virtual int NewsTypeId { get; set; }

        public virtual NewsType NewsType { get; set; }

        public virtual IEnumerable<NewsType> NewsTypeList { get; set; }

        public News()
        {

        }
    }
}

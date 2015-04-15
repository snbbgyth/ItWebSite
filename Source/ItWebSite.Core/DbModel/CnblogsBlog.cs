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
    public class CnblogsBlog : BaseTable
    {

        public CnblogsBlog()
        {
            BlogContentTypeList = new List<BlogContentType>();
        }

        public virtual int BlogContentTypeId { get; set; }

        /// <summary>
        /// Each content type just display max display order
        /// </summary>
        [DisplayName("显示顺序")]
        public virtual int DisplayOrder { get; set; }

        [DisplayName("标题")]
        public virtual string Title { get; set; }

        [DisplayName("来源")]
        public virtual string BlogFrom { get; set; }

        [DisplayName("来源URL")]
        public virtual string BlogFromUrl { get; set; }

        [AllowHtml]
        [DisplayName("正文")]
        public virtual string Content { get; set; }

        public virtual BlogContentType BlogContentType { get; set; }

        public virtual IEnumerable<BlogContentType> BlogContentTypeList { get; set; }

    }
}

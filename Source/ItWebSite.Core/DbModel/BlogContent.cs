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
    public class BlogContent : BaseTable
    {

        public BlogContent()
        {
            BlogContentTypeList = new List<BlogContentType>();
        }

        public virtual int WebContentTypeId { get; set; }

        /// <summary>
        /// Each content type just display max display order
        /// </summary>
        [DisplayName("显示顺序")]
        public virtual int DisplayOrder { get; set; }


        [AllowHtml]
        [DisplayName("正文")]
        public virtual string Content { get; set; }

        public virtual BlogContentType BlogContentType { get; set; }


        public virtual IEnumerable<BlogContentType> BlogContentTypeList { get; set; }

    }
}

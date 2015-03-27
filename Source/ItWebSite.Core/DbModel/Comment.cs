using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class Comment : BaseTable
    {
        [DisplayName("手机号码")]
        [Required]
        [Phone]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 11)]
        public virtual string Phone { get; set; }

        [DisplayName("姓名")]
        [Required]
        public virtual string UserName { get; set; }

        [DisplayName("留言")]
        [Required]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public virtual string Content { get; set; }
    }
}

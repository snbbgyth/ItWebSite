using System.ComponentModel;
using System.Web;

namespace ItWebSite.Web.Areas.Admin.Models
{
    public class UploadFileViewModel
    {
        [DisplayName("选择上传文件")]
        public HttpPostedFileBase File { get; set; }
    }
}
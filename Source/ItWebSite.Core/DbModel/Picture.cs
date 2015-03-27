using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class Picture : BaseTable
    {
        public virtual string MimeType { get; set; }

        public virtual byte[] PictureBinary { get; set; }

        public virtual string FileName { get; set; }

        //public virtual string FileExtension { get; set; }
    }
}

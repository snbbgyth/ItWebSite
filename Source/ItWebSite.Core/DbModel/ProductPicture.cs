using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class ProductPicture : BaseTable
    {

        public virtual int DisplayOrder { get; set; }

        public virtual int PictureId { get; set; }

        public virtual int ProductId { get; set; }

    }
}

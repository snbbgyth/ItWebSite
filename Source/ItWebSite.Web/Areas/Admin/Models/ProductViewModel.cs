using System.Collections.Generic;
using ItWebSite.Core.DbModel;

namespace ItWebSite.Web.Areas.Admin.Models
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            Product = new Product();
            PictureList = new List<Picture>();
            ProductPictureList = new List<ProductPicture>();
            UploadFile = new UploadFileViewModel();
        }

        public Product Product { get; set; }

        public List<Picture> PictureList { get; set; }


        public List<ProductPicture> ProductPictureList { get; set; }

        public UploadFileViewModel UploadFile { get; set; }


    }
}
using System.Collections.Generic;
using ItWebSite.Core.DbModel;

namespace ItWebSite.Web.Models
{
    public class ProductContentViewModel
    {
        public ProductContentViewModel()
        {
            Product = new Product();
            PictureList = new List<Picture>();
            ProductPictureList = new List<ProductPicture>();
            ShopCartItem=new ShopCartItem();
        }

        public Product Product { get; set; }

        public List<Picture> PictureList { get; set; }

        public List<ProductPicture> ProductPictureList { get; set; }

        public ShopCartItem ShopCartItem { get; set; }

    }
}
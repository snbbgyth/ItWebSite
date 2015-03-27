using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.Areas.Admin.Models;
using ItWebSite.Web.Models;

namespace ItWebSite.Web.DAL.Manage
{
    public class ProductManage
    {
        private static IProductPictureDal _productPictureDal;
        private static IPictureDal _pictureDal;
        private static IProductDal _productDal;
        private static IProductTypeDal _productTypeDal;

        private static IEnumerable<ProductType> allProductTypes; 

        static ProductManage()
        {
            _pictureDal = DependencyResolver.Current.GetService<IPictureDal>();
            _productDal = DependencyResolver.Current.GetService<IProductDal>();
            _productPictureDal = DependencyResolver.Current.GetService<IProductPictureDal>();
            _productTypeDal = DependencyResolver.Current.GetService<IProductTypeDal>();
            NewTaskProductType();
        }


        public static void RefreshProductType()
        {
            Task.Factory.StartNew(NewTaskProductType);
        }

        public static void NewTaskProductType()
        {
            allProductTypes = _productTypeDal.QueryAll();
        }
        

        public static   ProductType QueryProductTypeById(int id)
        {
            return allProductTypes.FirstOrDefault(t => t.Id == id);
            //return await _productTypeDal.QueryByIdAsync(id);
        }

        public static    IEnumerable<ProductType> QueryAllProductType()
        {
            return  allProductTypes;
        }

        public IEnumerable<Picture> GetPicturesById(int productId)
        {
            var mapList = _productPictureDal.QueryByFun(t => t.ProductId == productId);
            return _pictureDal.QueryByIds(mapList.Select(x => x.ProductId as dynamic));
        }

        public static Picture GetFirstPictureById(int productId)
        {
            var mapList = _productPictureDal.QueryByFun(t => t.ProductId == productId);
            if (mapList.Any())
                return _pictureDal.QueryById(mapList.FirstOrDefault(t => t.DisplayOrder == mapList.Min(x => x.DisplayOrder)).PictureId);
            return null;
        }

        public static async Task<ProductViewModel> GetProductViewById(int? id)
        {
            var productView = new ProductViewModel();
            if (id == null)
            {
                return productView;
            }
            productView.Product = await _productDal.QueryByIdAsync(id);
            if (productView.Product == null)
            {
                return productView;
            }
            productView.Product.ProductType = await _productTypeDal.QueryByIdAsync(productView.Product.ProductTypeId);
            productView.Product.ProductTypeList = await _productTypeDal.QueryAllAsync();
            productView.ProductPictureList = new List<ProductPicture>(await _productPictureDal.QueryByFunAsync(t => t.ProductId == id));
            if (productView.ProductPictureList.Any())
            {
                foreach (var productPicture in productView.ProductPictureList)
                {
                    var picture = await _pictureDal.QueryByIdAsync(productPicture.PictureId);
                    if (picture != null)
                        productView.PictureList.Add(picture);
                }
            }
            return productView;
        }

        public static async Task<ProductContentViewModel> QueryProductContentById(int? id)
        {
            var productContent = new ProductContentViewModel();
            if (id == null)
            {
                return productContent;
            }
            productContent.Product = await _productDal.QueryByIdAsync(id);
            if (productContent.Product == null)
            {
                return productContent;
            }
            productContent.Product.ProductType = await _productTypeDal.QueryByIdAsync(productContent.Product.ProductTypeId);
            productContent.Product.ProductTypeList = await _productTypeDal.QueryAllAsync();
            productContent.ProductPictureList = new List<ProductPicture>(await _productPictureDal.QueryByFunAsync(t => t.ProductId == id));
            if (productContent.ProductPictureList.Any())
            {
                foreach (var productPicture in productContent.ProductPictureList)
                {
                    var picture = await _pictureDal.QueryByIdAsync(productPicture.PictureId);
                    if (picture != null)
                        productContent.PictureList.Add(picture);
                }
            }
            productContent.ShopCartItem.Product = productContent.Product;
            productContent.ShopCartItem.ProductId = productContent.Product.Id;

            return productContent;
        }
    }
}
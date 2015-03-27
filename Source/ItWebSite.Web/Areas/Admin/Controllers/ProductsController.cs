using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.Areas.Admin.Models;
using ItWebSite.Web.DAL;
using ItWebSite.Web.DAL.Manage;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    public class ProductsController : BaseController
    {
        private IProductDal _productDal;
        private IPictureDal _pictureDal;
        private IProductPictureDal _productPictureDal;
        private IProductTypeDal _productTypeDal;

        public ProductsController()
        {
            _productDal = DependencyResolver.Current.GetService<IProductDal>();
            _pictureDal = DependencyResolver.Current.GetService<IPictureDal>();
            _productPictureDal = DependencyResolver.Current.GetService<IProductPictureDal>();
            _productTypeDal = DependencyResolver.Current.GetService<IProductTypeDal>();
        }

        // GET: Products
        public async Task<ActionResult> Index()
        {
            return View(await _productDal.QueryAllAsync());
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(await ProductManage.GetProductViewById(id));
        }

        // GET: Products/Create
        public async Task<ActionResult> Create()
        {
            var model = new ProductViewModel();
            model.Product.ProductTypeList = await _productTypeDal.QueryAllAsync();
            return View(model);
        }

        // POST: Products/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductViewModel productView)
        {
            InitInsert(productView.Product);
            productView.Product.Id = await _productDal.InsertAsync(productView.Product);
            if (productView.UploadFile.File != null)
            {
                await AddPictureToProduct(productView);
            }
            return RedirectToAction("Edit", new { id = productView.Product.Id });
            //return await Edit(productView.Product.Id);
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase fileName)
        {
            if (fileName != null)
            {
                var picture = await AddUploadFile(fileName);
                return Json(new { Result = true, PitureId = picture.Id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
        }

        private async Task<int> AddPictureToProduct(ProductViewModel productView)
        {
            var picture = await AddUploadFile(productView.UploadFile.File);
            var productPicture = new ProductPicture();
            productPicture.PictureId = picture.Id;
            productPicture.ProductId = productView.Product.Id;
            InitInsert(productPicture);
            await _productPictureDal.InsertAsync(productPicture);
            productView.PictureList.Add(picture);
            productView.ProductPictureList.Add(productPicture);
            return 1;
        }

        private async Task<Picture> AddUploadFileByPath(string filePath)
        {
            var picture = new Picture();
            picture.FileName = Path.GetFileName(filePath);
            picture.MimeType = ImageManage.GetContentType(filePath);
            InitInsert(picture);
            picture.Id = await _pictureDal.InsertAsync(picture);
            System.IO.File.Copy(filePath, ImageManage.GetOriginalImagePath(picture));
            HandleQueue.Instance.Add(picture);
            return picture;
        }

        private async Task<Picture> AddUploadFile(HttpPostedFileBase file)
        {
            var picture = new Picture();
            picture.FileName = file.FileName;
            picture.MimeType = ImageManage.GetContentType(file);
            InitInsert(picture);
            picture.Id = _pictureDal.Insert(picture);
            var path = ImageManage.GetOriginalImagePath(picture);
            try
            {
                file.SaveAs(path);
            }
            catch (Exception ex)
            {
            }
            HandleQueue.Instance.Add(picture);
            return picture;
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(await ProductManage.GetProductViewById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Product,PictureList,ProductPictureList,UploadFile")] ProductViewModel productView)
        {
            InitModify(productView.Product);
            await _productDal.ModifyAsync(productView.Product);
            if (productView.UploadFile.File != null)
            {
                await AddPictureToProduct(productView);
            }
            productView.Product.ProductTypeList = await _productTypeDal.QueryAllAsync();
          return   RedirectToAction("Edit", new {id = productView.Product.Id});
        }

        [HttpPost]
        public async Task<ActionResult> EditPicture(int? id, int? displayOrder)
        {
            if (id == null || displayOrder == null) return Json(new { Result = true }, JsonRequestBehavior.AllowGet); ;
            try
            {
                var entity = _productPictureDal.QueryById(id);
                entity.DisplayOrder = displayOrder ?? 0;
                InitModify(entity);
                _productPictureDal.Modify(entity);
            }
            catch (Exception ex)
            {
            }
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeletePicture(int? id)
        {
            try
            {
                var productPricture = await _productPictureDal.QueryByIdAsync(id);
                await _productPictureDal.DeleteByIdAsync(id);
                await ImageManage.DeleteImage(productPricture.PictureId);
            }
            catch (Exception ex)
            {
            }
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await _productDal.QueryByIdAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _productDal.DeleteByIdAsync(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

    }
}

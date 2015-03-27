using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.DAL;

namespace ItWebSite.Web.Controllers
{
    public class ShopCartItemsController : BaseController
    {
        private IShopCartItemDal _shopCartItemDal;
        private IProductDal _productDal;
        private IOrderDal _orderDal;
        private IOrderItemDal _orderItemDal;

        public ShopCartItemsController()
        {
            _shopCartItemDal = DependencyResolver.Current.GetService<IShopCartItemDal>();
            _productDal = DependencyResolver.Current.GetService<IProductDal>();
            _orderDal = DependencyResolver.Current.GetService<IOrderDal>();
            _orderItemDal = DependencyResolver.Current.GetService<IOrderItemDal>();
        }

        // GET: ShopCartItems
        public async Task<ActionResult> Index()
        {
            var entityList = await _shopCartItemDal.QueryByFunAsync(t => t.CustomerName == User.Identity.Name && !t.IsSubmit);
            foreach (var entity in entityList)
            {
                entity.Product = await _productDal.QueryByIdAsync(entity.ProductId);
            }
            return View(new List<ShopCartItem>(entityList));
        }

        [HttpPost]
        public async Task<ActionResult> Index(IList<ShopCartItem> entityList)
        {
            if (entityList == null || entityList.Count==0) return RedirectToAction("Index");
            if (entityList.Count > 0)
            {
                var submitList = entityList.Where(t => t.IsSubmit);
                if (submitList.Any())
                {
                    foreach (var shopCartItem in submitList)
                    {
                        _shopCartItemDal.SubmitById(shopCartItem.Id);
                    }
                }
            }

            return RedirectToAction("Submit","Orders");
        }

        // GET: ShopCartItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopCartItem shopCartItem = await _shopCartItemDal.QueryByIdAsync(id);
            if (shopCartItem == null)
            {
                return HttpNotFound();
            }
            return View(shopCartItem);
        }

        [HttpPost]
        public async Task<ActionResult> AddShopCart(int productId, int count)
        {
            if (count <= 0)
                return Json(new { Result = false, Message = "请选择数商品数量。" }, JsonRequestBehavior.AllowGet);
            try
            {
                var entity = new ShopCartItem
                {
                    Count = count,
                    ProductId = productId,
                    CustomerName = User.Identity.Name
                };
                InitInsert(entity);
                await _shopCartItemDal.InsertAsync(entity);
            }
            catch (Exception ex)
            {
            }
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        // GET: ShopCartItems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopCartItem shopCartItem = await _shopCartItemDal.QueryByIdAsync(id);
            if (shopCartItem == null)
            {
                return HttpNotFound();
            }
            shopCartItem.Product = await _productDal.QueryByIdAsync(shopCartItem.ProductId);
            return View(shopCartItem);
        }

        // POST: ShopCartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _shopCartItemDal.DeleteByIdAsync(id);
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

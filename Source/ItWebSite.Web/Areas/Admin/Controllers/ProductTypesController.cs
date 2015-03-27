using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.DAL;
using ItWebSite.Web.DAL.Manage;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    public class ProductTypesController : BaseController
    {
        private IProductTypeDal _productTypeDal;

        public ProductTypesController()
        {
            _productTypeDal = DependencyResolver.Current.GetService<IProductTypeDal>();
        }

        // GET: Admin/ProductTypes
        public async Task<ActionResult> Index()
        {
            return View(await _productTypeDal.QueryAllAsync());
        }

        // GET: Admin/ProductTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductType productType = await _productTypeDal.QueryByIdAsync(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
            return View(productType);
        }

        // GET: Admin/ProductTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ProductTypes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] ProductType productType)
        {
            try
            {
                InitInsert(productType);
                await _productTypeDal.InsertAsync(productType);
                ProductManage.RefreshProductType();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(productType);
            }
        }

        // GET: Admin/ProductTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductType productType = await _productTypeDal.QueryByIdAsync(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
 
            return View(productType);
        }

        // POST: Admin/ProductTypes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CreateDate,LastModifyDate,IsDelete,Creater,LastModifier")] ProductType productType)
        {
            try
            {
                InitModify(productType);
                await _productTypeDal.ModifyAsync(productType);
                ProductManage.RefreshProductType();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(productType);
            }
        }

        // GET: Admin/ProductTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductType productType = await _productTypeDal.QueryByIdAsync(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
            return View(productType);
        }

        // POST: Admin/ProductTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _productTypeDal.DeleteByIdAsync(id);
            ProductManage.RefreshProductType();
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

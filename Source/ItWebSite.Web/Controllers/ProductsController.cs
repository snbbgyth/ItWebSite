using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.DAL.Manage;

namespace ItWebSite.Web.Controllers
{
    public class ProductsController : Controller
    {
        private IProductDal _productDal;

        public ProductsController()
        {
            _productDal = DependencyResolver.Current.GetService<IProductDal>();
        }

        // GET: Products
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "所有产品";
            return View(await _productDal.QueryAllAsync());
        }

        public async Task<ActionResult> ShowIndex(int id)
        {
            var productType = ProductManage.QueryProductTypeById(id);
            ViewBag.Title = productType.Name;

            return View(await _productDal.QueryByFunAsync(t => t.ProductTypeId == id));
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(await ProductManage.QueryProductContentById(id));
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

using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.Areas.Admin.Models;
using ItWebSite.Web.DAL.Manage;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ItWebSite.Web.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private IOrderDal _orderDal;
        public UserManager<ApplicationUser> _userManager { get; private set; }
        private ApplicationDbContext context { get; set; }

        private IOrderItemDal _orderItemDal;

        private static IShopCartItemDal _shopCartItemDal;

        public OrdersController()
        {
            context = new ApplicationDbContext();
            _orderDal = DependencyResolver.Current.GetService<IOrderDal>();
            _orderItemDal = DependencyResolver.Current.GetService<IOrderItemDal>();
            _shopCartItemDal = DependencyResolver.Current.GetService<IShopCartItemDal>();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }


        /// <summary>
        /// 类型：1代表己支付，0代表未支付，2代表全部。默认是1
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string type)
        {
            if (type == "2")
            {
                ViewBag.Title = "全部订单";
                return View(await _orderDal.QueryAllAsync());
            }
            var isPay = type == "1";
            if (isPay)
            {
                ViewBag.Title = "己支付订单";
            }
            else
            {
                ViewBag.Title = "未支付订单";
            }
            return View(await _orderDal.QueryByFunAsync(t => t.IsPay == isPay));
        }

        // GET: Admin/Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderView = await OrderManage.GetOrderView(id.Value);
            if (orderView == null)
            {
                return HttpNotFound();
            }
            return View(orderView);
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

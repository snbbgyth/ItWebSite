using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.Areas.Admin.Models;
using ItWebSite.Web.DAL;
using ItWebSite.Web.DAL.Manage;
using ItWebSite.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ItWebSite.Web.Controllers
{
    public class OrdersController : BaseController
    {
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

        public async Task<ActionResult> NoPay()
        {
            return View(await _orderDal.QueryByFunAsync(t => !t.IsPay));
        }

        public async Task<ActionResult> Complete()
        {
            return View(await _orderDal.QueryByFunAsync(t => t.IsPay));
        }

        public async Task<ActionResult> Submit()
        {
            var user = _userManager.FindByName(User.Identity.Name);
            var orderView = new OrderViewModel();
            orderView.OrderItemViewList = await OrderManage.GetOrderItemsByUserName(User.Identity.Name);
            if (!orderView.OrderItemViewList.Any())
                return RedirectToAction("NoSubmit");
            orderView.Order.CustomerName = User.Identity.Name;
            orderView.Order.CustomerPhone = user.PhoneNumber;
            orderView.Order.TotalPrice = orderView.OrderItemViewList.Sum(t => t.OrderItem.Total);
            return View(orderView);

        }

        [HttpPost]
        public async Task<ActionResult> Submit(OrderViewModel orderView)
        {
            if (ModelState.IsValid)
            {
                var order = orderView.Order;
                InitInsert(order);
                OrderManage.GenerateOrderNumber(order);
                var orderId = await _orderDal.InsertAsync(order);
                foreach (var orderItemView in orderView.OrderItemViewList)
                {
                    orderItemView.OrderItem.OrderId = orderId;
                    InitInsert(orderItemView.OrderItem);
                    await _orderItemDal.InsertAsync(orderItemView.OrderItem);
                    await _shopCartItemDal.DeleteByIdAsync(orderItemView.ShopCartItem.Id);
                }
                return RedirectToAction("NoPay");
            }
            return View(orderView);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteShopCartItem(int id)
        {
            await _shopCartItemDal.DeleteByIdAsync(id);
            return RedirectToAction("Submit");
        }

        // GET: Orders/Details/5
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

        // GET: Orders/Create
        public ActionResult NoSubmit()
        {
            return View();
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
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

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await OrderManage.DeleteOrder(id);
            return RedirectToAction("Complete");
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

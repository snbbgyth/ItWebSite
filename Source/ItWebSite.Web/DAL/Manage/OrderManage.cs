using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Web.Models;

namespace ItWebSite.Web.DAL.Manage
{
    public class OrderManage
    {
        private static IShopCartItemDal _shopCartItemDal;
        private static IProductDal _productDal;
        private static IOrderDal _orderDal;
        private static IOrderItemDal _orderItemDal;

        static OrderManage()
        {
            _shopCartItemDal = DependencyResolver.Current.GetService<IShopCartItemDal>();
            _productDal = DependencyResolver.Current.GetService<IProductDal>();
            _orderDal = DependencyResolver.Current.GetService<IOrderDal>();
            _orderItemDal = DependencyResolver.Current.GetService<IOrderItemDal>();
        }

        public static async Task<OrderViewModel> GetOrderView(int orderId)
        {
            var orderView = new OrderViewModel();
            orderView.Order = await _orderDal.QueryByIdAsync(orderId);
            orderView.OrderItemViewList = await GetOrderItemViewList(orderId);
            foreach (var orderItem in orderView.OrderItemViewList)
            {
                orderItem.OrderItem.Product = await _productDal.QueryByIdAsync(orderItem.OrderItem.ProductId);
            }
            return orderView;
        }

        public static IEnumerable<Order> LastOrders(int count)
        {
            return _orderDal.QueryLastByFun(t=>t.IsPay,count);
        }


        private static async Task<List<OrderItemViewModel>> GetOrderItemViewList(int orderId)
        {
            var orderItemList = await _orderItemDal.QueryByFunAsync(t => t.OrderId == orderId);
            return orderItemList.Select(orderItem => new OrderItemViewModel
            {
                OrderItem = orderItem,
            }).ToList();
        }

        public static async Task<IList<OrderItemViewModel>> GetOrderItemsByUserName(string userName)
        {
            var shopCartItems = await _shopCartItemDal.QueryByFunAsync(t => t.CustomerName == userName && t.IsSubmit == true);
            if (shopCartItems.Any())
            {
                return shopCartItems.Select(t => ConvertToOrderItem(t)).ToList();
            }
            return new List<OrderItemViewModel>();
        }

        private static OrderItemViewModel ConvertToOrderItem(ShopCartItem shopCartItem)
        {
            shopCartItem.Product = _productDal.QueryById(shopCartItem.ProductId);
            return new OrderItemViewModel
            {
                OrderItem = new OrderItem
              {
                  ProductId = shopCartItem.ProductId,
                  Count = shopCartItem.Count,
                  Price = shopCartItem.Product.Price,
                  Product = shopCartItem.Product,
                  Total = shopCartItem.Count * shopCartItem.Product.Price
              },
                ShopCartItem = shopCartItem
            };
        }

        public static void GenerateOrderNumber(Order order)
        {
            var timeTick = DateTime.Now.ToString("yyyyMMddhhmmss");
            var random = new Random().Next(100, 10000).ToString("0000");
            order.OrderNumber = timeTick + random;
        }

        public static async Task<int> DeleteOrder(int orderId)
        {
            await _orderDal.DeleteByIdAsync(orderId);
            _orderItemDal.DeleteByOrderId(orderId);
            return 1;
        }
    }
}
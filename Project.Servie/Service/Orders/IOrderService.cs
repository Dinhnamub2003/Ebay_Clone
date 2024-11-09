using Project.Data.Models;
using Project.Model.OrderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Service.Orders
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(int userId, decimal totalAmount, string status);
        Task AddOrderDetailsAsync(int orderId, List<OrderDetail> orderDetails);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task<Data.Models.Order> GetOrderByIdAsync(int orderId);
        Task<List<ViewOrderModel>> GetOrderByUserIdAsync(int userId);
        Task UpdateOrderTotalAmountAsync(int orderId, decimal totalAmount);

        Task UpdatePendingOrdersToConfirmedAsync();
    }
}

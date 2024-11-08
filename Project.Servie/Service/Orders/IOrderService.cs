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
        Task<int> CreateOrderAsync(int userId, decimal totalAmount, string status = "Confirmed");
        Task AddOrderDetailsAsync(int orderId, List<OrderDetail> orderDetails);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task<List<ViewOrderModel>> GetOrderByIdAsync(int orderId);
        Task UpdateOrderTotalAmountAsync(int orderId, decimal totalAmount);
    }
}

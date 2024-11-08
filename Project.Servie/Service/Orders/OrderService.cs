using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Project.Bussiness.Infrastructure;
using Project.Data.Models;
using Project.Model.OrderModel;
using Project.Service.Service.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Service.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<int> CreateOrderAsync(int userId, decimal totalAmount, string status = "Confirmed")
        {
            var order = new Data.Models.Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = status,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _unitOfWork.OrderRepository.Add(order);
            await _unitOfWork.SaveChangesAsync();

            return order.OrderId;
        }


        public async Task AddOrderDetailsAsync(int orderId, List<OrderDetail> orderDetails)
        {
            foreach (var detail in orderDetails)
            {
                detail.OrderId = orderId;
                detail.CreatedAt = DateTime.UtcNow;
                _unitOfWork.OrderDetailRepository.Add(detail);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            order.OrderStatus = status;
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ViewOrderModel>> GetOrderByIdAsync(int orderId)
        {
            return await _unitOfWork.OrderDetailRepository.GetQuery()
               .Where(od => od.OrderId == orderId)
               .Select(od => new ViewOrderModel
               {
                   OrderDetailId = od.OrderDetailId,
                   ProductId = od.Product.ProductId,
                   Quantity = od.Quantity,
                   Price = od.Price,
                   OrderId = od.Order.OrderId,
                   CreatedAt = od.CreatedAt
               })
               .ToListAsync();
        }

        public async Task UpdateOrderTotalAmountAsync(int orderId, decimal totalAmount)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                order.TotalAmount = totalAmount;
                _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Order not found");
            }
        }
    }
}


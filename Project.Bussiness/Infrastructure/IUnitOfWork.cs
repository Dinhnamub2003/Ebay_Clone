using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories for each model
        IBaseRepository<User> UserRepository { get; }
        IBaseRepository<Role> RoleRepository { get; }
        IBaseRepository<Category> CategoryRepository { get; }
        IBaseRepository<Product> ProductRepository { get; }
        IBaseRepository<ProductImage> ProductImageRepository { get; }
        IBaseRepository<Cart> CartRepository { get; }
        IBaseRepository<Review> ReviewRepository { get; }
        IBaseRepository<Order> OrderRepository { get; }
        IBaseRepository<OrderDetail> OrderDetailRepository { get; }
        IBaseRepository<Payment> PaymentRepository { get; }

        // Generic repository method
        IBaseRepository<T> GenericRepository<T>() where T : class;

        // Save changes methods
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync();
    }
}

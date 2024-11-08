using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EbayClone1Context _appDbContext;

        public UnitOfWork(EbayClone1Context appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // User Repository
        private IBaseRepository<User>? _userRepository;
        public IBaseRepository<User> UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new BaseRepository<User, EbayClone1Context>(_appDbContext);
                }
                return _userRepository;
            }
        }

        // Role Repository
        private IBaseRepository<Role>? _roleRepository;
        public IBaseRepository<Role> RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                {
                    _roleRepository = new BaseRepository<Role, EbayClone1Context>(_appDbContext);
                }
                return _roleRepository;
            }
        }

        // Category Repository
        private IBaseRepository<Category>? _categoryRepository;
        public IBaseRepository<Category> CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new BaseRepository<Category, EbayClone1Context>(_appDbContext);
                }
                return _categoryRepository;
            }
        }

        // Product Repository
        private IBaseRepository<Product>? _productRepository;
        public IBaseRepository<Product> ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new BaseRepository<Product, EbayClone1Context>(_appDbContext);
                }
                return _productRepository;
            }
        }

        // Product Image Repository
        private IBaseRepository<ProductImage>? _productImageRepository;
        public IBaseRepository<ProductImage> ProductImageRepository
        {
            get
            {
                if (_productImageRepository == null)
                {
                    _productImageRepository = new BaseRepository<ProductImage, EbayClone1Context>(_appDbContext);
                }
                return _productImageRepository;
            }
        }

        // Cart Repository
        private IBaseRepository<Cart>? _cartRepository;
        public IBaseRepository<Cart> CartRepository
        {
            get
            {
                if (_cartRepository == null)
                {
                    _cartRepository = new BaseRepository<Cart, EbayClone1Context>(_appDbContext);
                }
                return _cartRepository;
            }
        }

        // Review Repository
        private IBaseRepository<Review>? _reviewRepository;
        public IBaseRepository<Review> ReviewRepository
        {
            get
            {
                if (_reviewRepository == null)
                {
                    _reviewRepository = new BaseRepository<Review, EbayClone1Context>(_appDbContext);
                }
                return _reviewRepository;
            }
        }

        // Order Repository
        private IBaseRepository<Order>? _orderRepository;
        public IBaseRepository<Order> OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new BaseRepository<Order, EbayClone1Context>(_appDbContext);
                }
                return _orderRepository;
            }
        }

        // Order Detail Repository
        private IBaseRepository<OrderDetail>? _orderDetailRepository;
        public IBaseRepository<OrderDetail> OrderDetailRepository
        {
            get
            {
                if (_orderDetailRepository == null)
                {
                    _orderDetailRepository = new BaseRepository<OrderDetail, EbayClone1Context>(_appDbContext);
                }
                return _orderDetailRepository;
            }
        }

        // Payment Repository
        private IBaseRepository<Payment>? _paymentRepository;
        public IBaseRepository<Payment> PaymentRepository
        {
            get
            {
                if (_paymentRepository == null)
                {
                    _paymentRepository = new BaseRepository<Payment, EbayClone1Context>(_appDbContext);
                }
                return _paymentRepository;
            }
        }


        // Notification Repository
        private IBaseRepository<Notification>? _notificationRepository;
        public IBaseRepository<Notification> NotificationRepository
        {
            get
            {
                if (_notificationRepository == null)
                {
                    _notificationRepository = new BaseRepository<Notification, EbayClone1Context>(_appDbContext);
                }
                return _notificationRepository;
            }
        }

      

        // Transaction Repository
        private IBaseRepository<Transaction>? _transactionRepository;
        public IBaseRepository<Transaction> TransactionRepository
        {
            get
            {
                if (_transactionRepository == null)
                {
                    _transactionRepository = new BaseRepository<Transaction, EbayClone1Context>(_appDbContext);
                }
                return _transactionRepository;
            }
        }

        // Wallet Repository
        private IBaseRepository<Wallet>? _walletRepository;
        public IBaseRepository<Wallet> WalletRepository
        {
            get
            {
                if (_walletRepository == null)
                {
                    _walletRepository = new BaseRepository<Wallet, EbayClone1Context>(_appDbContext);
                }
                return _walletRepository;
            }
        }


        private IBaseRepository<TransactionType>? _transactionTypeRepository;
        public IBaseRepository<TransactionType> TransactionTypeRepository
        {
            get
            {
                if (_transactionTypeRepository == null)
                {
                    _transactionTypeRepository = new BaseRepository<TransactionType, EbayClone1Context>(_appDbContext);
                }
                return _transactionTypeRepository;
            }
        }


        


        // Generic repository method
        public IBaseRepository<T> GenericRepository<T>() where T : class
        {
            return new BaseRepository<T, EbayClone1Context>(_appDbContext);
        }

        // Save changes
        public int SaveChanges()
        {
            return _appDbContext.SaveChanges();
        }

        // Save changes asynchronously with cancellation token
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.SaveChangesAsync(cancellationToken);
        }

        // Save changes asynchronously
        public async Task<int> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }

        // Dispose the DbContext
        public void Dispose()
        {
            _appDbContext.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Project.Data.Models
{
    public partial class EbayClone1Context : DbContext
    {
        public EbayClone1Context()
        {
        }

        public EbayClone1Context(DbContextOptions<EbayClone1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<TransactionType> TransactionTypes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Wallet> Wallets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =(local); database = EbayClone1 ; uid=sa;pwd=123;Trusted_Connection=True;Encrypt=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("cart");

                entity.Property(e => e.CartId).HasColumnName("cart_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__cart__product_id__74AE54BC");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__cart__user_id__73BA3083");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("category_name");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");

                entity.Property(e => e.NotificationId).HasColumnName("notification_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsRead)
                    .HasColumnName("is_read")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Message)
                    .HasColumnType("text")
                    .HasColumnName("message");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__notificat__user___0E6E26BF");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("order_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("order_status");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_amount");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__orders__user_id__7B5B524B");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("order_details");

                entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__order_det__order__01142BA1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__order_det__produ__02084FDA");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.PaymentAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("payment_amount");

                entity.Property(e => e.PaymentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("payment_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("payment_status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__payments__order___09A971A2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__payments__user_i__08B54D69");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.BrandName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("brand_name");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("product_name");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("User_id");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__products__catego__5DCAEF64");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__products__User_i__5EBF139D");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__product___DC9AC955AB5C0DEF");

                entity.ToTable("product_images");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.ImageUrl)
                    .IsUnicode(false)
                    .HasColumnName("image_url");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__product_i__produ__6477ECF3");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");

                entity.Property(e => e.ReviewId).HasColumnName("review_id");

                entity.Property(e => e.Comment)
                    .HasColumnType("text")
                    .HasColumnName("comment");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.ReviewDate)
                    .HasColumnType("datetime")
                    .HasColumnName("review_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ReviewerId).HasColumnName("reviewer_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__reviews__product__6C190EBB");

                entity.HasOne(d => d.Reviewer)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ReviewerId)
                    .HasConstraintName("FK__reviews__reviewe__6D0D32F4");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("role_name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionsId)
                    .HasName("PK__transact__235186093C2D6C26");

                entity.ToTable("transactions");

                entity.Property(e => e.TransactionsId).HasColumnName("transactions_id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.BalanceAfter)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("balance_after");

                entity.Property(e => e.BalanceBefore)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("balance_before");

                entity.Property(e => e.Bank)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImgBank)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");

                entity.HasOne(d => d.TransactionType)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.TransactionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__transacti__trans__1CBC4616");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__transacti__walle__1BC821DD");
            });

            modelBuilder.Entity<TransactionType>(entity =>
            {
                entity.ToTable("transaction_type");

                entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");

                entity.Property(e => e.TypeName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.Avatar)
                    .IsUnicode(false)
                    .HasColumnName("avatar");

                entity.Property(e => e.Code)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("fullname");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsVerification)
                    .HasColumnName("isVerification")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__users__role_id__5812160E");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("wallet");

                entity.HasIndex(e => e.UserId, "UQ__wallet__B9BE370E58CE839C")
                    .IsUnique();

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");

                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("balance")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.LastUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Wallet)
                    .HasForeignKey<Wallet>(d => d.UserId)
                    .HasConstraintName("FK__wallet__user_id__14270015");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

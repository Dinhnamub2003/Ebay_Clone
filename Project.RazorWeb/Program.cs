using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Project.Bussiness.Infrastructure.Repository;
using Project.Bussiness.Infrastructure;
using Project.Data.Models;
using Project.Service.Service.Captcha;
using Project.Service.Service.Email;
using Project.Servie.Service.Auth;
using System.Text;
using Project.Servie.Service.Products;
using Project.Servie.Service.Categories;
using Project.Service.Service.Carts;
using Project.Service.Service.Orders;
using Project.Service.Service.Account;
using Project.Servie.Service.VnPay;
using Project.Service.Service.Wallets;
using Project.Service.Service.Order;
using Project.Service.Service.VnPay;
using Project.EventRazor.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// Register DbContext
builder.Services.AddDbContext<EbayClone1Context>();

// Register UnitOfWork and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBaseRepository<User>, UserRepository>();

// Register Auth, Email, and Captcha Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICaptchaService, CaptchaService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHostedService<BackgoundService>();






// Add session and cache

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<VnPayLibrary>();

// Register HttpClient if needed for external API calls
builder.Services.AddHttpClient();

// Register JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT"); // Load JWT settings from appsettings.json
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]); // Convert secret to byte array




// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;  // Chỉ dùng nếu phát triển cục bộ không có HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,  // Đảm bảo token còn hạn
            ValidateIssuerSigningKey = true,  // Kiểm tra khóa ký JWT
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });



builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();
app.UseSession();  // Enable session
app.UseMiddleware<JwtMiddleware>();  // Apply JWT Middleware
app.UseAuthentication();  // Handle authentication
app.UseAuthorization();  // Handle authorization
app.MapRazorPages();


app.MapGet("/", context =>
{
    context.Response.Redirect("/home");
    return Task.CompletedTask;
});


app.MapRazorPages();
app.MapHub<DocumentHub>("/documentHub");
app.Run();

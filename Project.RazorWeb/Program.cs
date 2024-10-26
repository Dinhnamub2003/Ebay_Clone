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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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

builder.Services.AddScoped<ICategoryService, CategoryService>();





// Add session and cache

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register HttpClient if needed for external API calls
builder.Services.AddHttpClient();

// Register JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT"); // Load JWT settings from appsettings.json
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]); // Convert secret to byte array

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Validate the Issuer
        ValidateAudience = true, // Validate the Audience
        ValidateIssuerSigningKey = true, // Validate the Signing Key
        ValidateLifetime = true, // Validate token expiration
        ValidIssuer = jwtSettings["ValidIssuer"], // Issuer
        ValidAudience = jwtSettings["ValidAudience"], // Audience
        IssuerSigningKey = new SymmetricSecurityKey(secretKey) // Secret key for signing the token
    };
});

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

// Enable session
app.UseSession();

// Enable authentication and authorization middleware
app.UseAuthentication(); // This is needed to ensure JWT authentication works
app.UseAuthorization();


app.MapGet("/", context =>
{
    context.Response.Redirect("/home");
    return Task.CompletedTask;
});


app.MapRazorPages();

app.Run();

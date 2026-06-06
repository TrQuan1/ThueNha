using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalHouse.Application.Interfaces;
using RentalHouse.Infrastructure.Data;
using RentalHouse.Infrastructure.Repositories;
using RentalHouse.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Connection String kết nối Database MySQL (Pomelo)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Đăng ký các dịch vụ Repository Pattern & Unit of Work
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// 3. Đăng ký bộ mã hóa mật khẩu và dịch vụ sinh mã thông báo JWT
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();

// 4. Cấu hình MediatR quét qua tầng Application để liên kết CQRS Handler
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(IUnitOfWork).Assembly));

// 5. Cấu hình xác thực hộ chiếu điện tử bằng JwtBearer Middleware
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey chưa được cấu hình.");

builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();

// Đăng ký Repository xử lý Database cho Ảnh
builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();

// Đăng ký Service xử lý lưu file vật lý vào thư mục wwwroot
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Tạm thời tắt do chạy Localhost Development
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Triệt tiêu độ trễ 5 phút mặc định của Server
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RentalHouse API",
        Version = "v1"
    });

    // Tích hợp nút nhập khóa Access Token (JWT) lên giao diện Swagger UI
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Nhập chuỗi Token của bạn theo định dạng: Bearer {chuỗi_token_của_bạn}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    // ĐÃ SỬA CHÍNH XÁC Ở DÒNG NÀY: Dùng ReferenceType
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles(); // Dòng này cực kỳ quan trọng để Frontend xem được ảnh

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Kích hoạt Middleware Xác thực (Phải đặt TRƯỚC UseAuthorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
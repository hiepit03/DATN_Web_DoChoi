using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models.Momo;
using Web_DoChoi.Repository;
using Web_DoChoi.Services.Momo;
using Web_DoChoi.Services.Vnpay;
using Web_DoChoi.Services;

var builder = WebApplication.CreateBuilder(args);

//connect momoapi
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

//connect db
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

// Cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Đường dẫn trang đăng nhập
        options.LogoutPath = "/Account/Logout"; // Đường dẫn trang đăng xuất
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian hết hạn của cookie
        options.SlidingExpiration = true;  // Gia hạn cookie khi người dùng đang hoạt động
    });

// Đăng ký dịch vụ EmailSender
builder.Services.AddSingleton<EmailSender>(); // Hoặc AddScoped<EmailSender>()

// Trước khi build
builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews();

//connect vnpay
builder.Services.AddScoped<IVnPayService, VnPayService>();

var app = builder.Build();

Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // ✅ Session được dùng trước Authentication

app.UseAuthentication(); // Đảm bảo authentication middleware được sử dụng

app.UseAuthorization();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=ThongKe}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TrangChu}/{action=Index}/{id?}");

// Seeding data
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedingData(context);

app.Run();

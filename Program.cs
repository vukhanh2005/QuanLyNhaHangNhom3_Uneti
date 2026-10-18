using Azure;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repository;
using QuanLyNhaHang.Utils;

namespace QuanLyNhaHang
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddScoped<AccountRepository>();
            builder.Services.AddScoped<LoaiBanRepository>();
            builder.Services.AddScoped<MonAnRepository>();
            builder.Services.AddScoped<IMonAnService, QuanLyNhaHang.Service.Implement.MonAnService>();
            builder.Services.AddScoped<ILoaiBanService, QuanLyNhaHang.Service.Implement.LoaiBanService>();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );
            });
            
            var app = builder.Build();
            // Chỉ seed khi chủ động chạy với --seed-demo-accounts.
            if (args.Contains("--seed-demo-accounts"))
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                AccountSeeder.CreateAsync(context, "admin", "Admin@123456",
                    "Quản trị viên", "admin@example.com", VaiTro.Admin).GetAwaiter().GetResult();
                AccountSeeder.CreateAsync(context, "user", "User@123456",
                    "Khách hàng mẫu", "user@example.com", VaiTro.User,
                    new KhachHang
                    {
                        HoTen = "Khách hàng mẫu",
                        Email = "user@example.com",
                        NgaySinh = new DateTime(2000, 1, 1),
                        GioiTinh = true,
                        SoDienThoai = "0900000000",
                        DiaChi = "Hà Nội",
                        NgayDangKy = DateTime.Now,
                        TrangThai = true
                    }).GetAwaiter().GetResult();

                var accounts = context.TaiKhoans.Include(x => x.KhachHang)
                    .Where(x => x.TenDangNhap == "admin" || x.TenDangNhap == "user").ToList();
                foreach (var account in accounts)
                {
                    var password = account.VaiTro == VaiTro.Admin ? "Admin@123456" : "User@123456";
                    if (!QuanLyNhaHang.Helpers.PasswordHelper.VerifyPassword(account, account.MatKhau!, password))
                        throw new InvalidOperationException("Mật khẩu tài khoản seed không khớp.");
                    if (account.VaiTro == VaiTro.User && account.KhachHang == null)
                        throw new InvalidOperationException("Tài khoản user chưa có hồ sơ khách hàng.");
                    Console.WriteLine($"Seed verified: {account.TenDangNhap}, role={account.VaiTro}, customer={account.KhachHang?.MaKhachHang}");
                }
                return;
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();
            app.UseSession();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=TrangChu}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}

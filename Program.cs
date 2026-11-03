using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repository;
using QuanLyNhaHang.Service.Interface;
using QuanLyNhaHang.Service.Implement;
using QuanLyNhaHang.Utils;

namespace QuanLyNhaHang
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();

            // Repository
            builder.Services.AddScoped<AccountRepository>();
            builder.Services.AddScoped<LoaiBanRepository>();
            builder.Services.AddScoped<MonAnRepository>();

            // Module 1
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Module 2
            builder.Services.AddScoped<IMonAnService, MonAnService>();
            builder.Services.AddScoped<ILoaiBanService, LoaiBanService>();

            // Module 4
            builder.Services.AddScoped<IPhieuDatBanService, PhieuDatBanService>();
            builder.Services.AddScoped<IClientDatBanService, ClientDatBanService>();

            // Session
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

            // Chỉ seed bàn khi chạy với tham số --seed-ban-an.
            if (args.Contains("--seed-ban-an"))
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var count = await QuanLyNhaHang.Data.BanAnSeeder.SeedAsync(context);
                Console.WriteLine($"Đã thêm {count} bàn mẫu.");
                return;
            }

            // Chỉ seed tài khoản mẫu khi chạy với tham số --seed-demo-accounts.
            if (args.Contains("--seed-demo-accounts"))
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                await AccountSeeder.CreateAsync(
                    context,
                    "admin",
                    "Admin@123456",
                    "Quản trị viên",
                    "admin@example.com",
                    VaiTro.Admin
                );

                await AccountSeeder.CreateAsync(
                    context,
                    "user",
                    "User@123456",
                    "Khách hàng mẫu",
                    "user@example.com",
                    VaiTro.User,
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
                    }
                );

                var accounts = context.TaiKhoans
                    .Include(x => x.KhachHang)
                    .Where(x => x.TenDangNhap == "admin" ||
                                x.TenDangNhap == "user")
                    .ToList();

                foreach (var account in accounts)
                {
                    var password = account.VaiTro == VaiTro.Admin
                        ? "Admin@123456"
                        : "User@123456";

                    if (!QuanLyNhaHang.Helpers.PasswordHelper.VerifyPassword(
                        account, account.MatKhau!, password))
                    {
                        throw new InvalidOperationException(
                            "Mật khẩu tài khoản seed không khớp.");
                    }

                    if (account.VaiTro == VaiTro.User &&
                        account.KhachHang == null)
                    {
                        throw new InvalidOperationException(
                            "Tài khoản user chưa có hồ sơ khách hàng.");
                    }

                    Console.WriteLine(
                        $"Seed verified: {account.TenDangNhap}, " +
                        $"role={account.VaiTro}, " +
                        $"customer={account.KhachHang?.MaKhachHang}");
                }

                return;
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();

            // Session phải được bật trước khi xử lý Controller.
            app.UseSession();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=TrangChu}/{action=Index}/{id?}"
            ).WithStaticAssets();

            await app.RunAsync();
        }
    }
}

using Azure;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repository;
using QuanLyNhaHang.Service.Implement;
using QuanLyNhaHang.Utils;

namespace QuanLyNhaHang
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Tải biến môi trường từ file .env nếu có
            LoadDotEnv();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddScoped<AccountRepository>();
            builder.Services.AddScoped<KhachHangRepository>();
            builder.Services.AddScoped<IKhachHangService, QuanLyNhaHang.Service.Implement.KhachHangService>();
            builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
            builder.Services.AddScoped<PhieuDatBanRepository>();
            builder.Services.AddScoped<IPhieuDatBanService, PhieuDatBanService>();
            builder.Services.AddScoped<LoaiBanRepository>();
            builder.Services.AddScoped<MonAnRepository>();
            builder.Services.AddScoped<IMonAnService, QuanLyNhaHang.Service.Implement.MonAnService>();
            builder.Services.AddScoped<ILoaiBanService, QuanLyNhaHang.Service.Implement.LoaiBanService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100);
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

            // Tự động kiểm tra và seed tài khoản Admin từ .env nếu chưa có
            await EnsureAdminAccountAsync(app);

            // Vi Thái Học - 23103100054: chỉ seed Module 2 khi được gọi rõ ràng.
            if (args.Contains("--seed-ban-an"))
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var count = await QuanLyNhaHang.Data.BanAnSeeder.SeedAsync(context);
                Console.WriteLine($"Đã thêm {count} bàn mẫu.");
                return;
            }
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
            await app.RunAsync();
        }

        /// <summary>
        /// Đọc các biến cấu hình từ file .env vào Environment Variables
        /// </summary>
        private static void LoadDotEnv()
        {
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            if (!File.Exists(envPath)) return;

            foreach (var line in File.ReadAllLines(envPath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var parts = trimmed.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim().Trim('"', '\'');
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        /// <summary>
        /// Kiểm tra database xem đã có tài khoản Admin nào chưa.
        /// Nếu chưa, tự động seed tài khoản Admin theo cấu hình trong .env qua AccountSeeder.
        /// </summary>
        private static async Task EnsureAdminAccountAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Kiểm tra đã có tài khoản có role Admin chưa
                var hasAdmin = await context.TaiKhoans.AnyAsync(t => t.VaiTro == VaiTro.Admin);
                if (!hasAdmin)
                {
                    var adminUsername = Environment.GetEnvironmentVariable("AdminUsername");
                    if (string.IsNullOrWhiteSpace(adminUsername))
                        adminUsername = "admin";

                    var adminPassword = Environment.GetEnvironmentVariable("AdminPassword");
                    if (string.IsNullOrWhiteSpace(adminPassword))
                        adminPassword = "123456";

                    var adminFullName = Environment.GetEnvironmentVariable("AdminFullName");
                    if (string.IsNullOrWhiteSpace(adminFullName))
                        adminFullName = "Quản trị viên";

                    var adminEmail = Environment.GetEnvironmentVariable("AdminEmail");
                    if (string.IsNullOrWhiteSpace(adminEmail))
                        adminEmail = "admin@nhahang.local";

                    var success = await AccountSeeder.CreateAsync(
                        context,
                        adminUsername,
                        adminPassword,
                        adminFullName,
                        adminEmail,
                        VaiTro.Admin
                    );

                    if (success)
                    {
                        Console.WriteLine($"[Seed] Chưa có Admin trong hệ thống. Đã tự động tạo tài khoản Admin: {adminUsername}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Tránh throw exception làm dừng ứng dụng khi chạy các công cụ CLI chưa kết nối DB
                Console.WriteLine($"[Seed Notice] Kiểm tra seed Admin: {ex.Message}");
            }
        }
    }
}

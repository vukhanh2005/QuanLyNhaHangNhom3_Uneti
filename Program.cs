using Azure;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repository;

namespace QuanLyNhaHang
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddScoped<AccountRepository>();
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
            // Vi Thái Học - 23103100054: chỉ seed Module 2 khi được gọi rõ ràng.
            if (args.Contains("--seed-ban-an"))
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var count = await QuanLyNhaHang.Data.BanAnSeeder.SeedAsync(context);
                Console.WriteLine($"Đã thêm {count} bàn mẫu.");
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
    }
}

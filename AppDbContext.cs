using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

public class AppDbContext : DbContext
{
    public DbSet<TaiKhoan> TaiKhoans { get; set; }

    public DbSet<BanAn> BanAns { get; set; }

    public DbSet<MonAn> MonAns { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
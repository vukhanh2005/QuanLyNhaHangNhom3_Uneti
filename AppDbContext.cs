using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangNhom3_Uneti.Models;
using QuanLyNhaHang.Models;

public class AppDbContext : DbContext{
    public DbSet<TaiKhoan> TaiKhoans {get; set;}
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<BanAn> BanAns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}

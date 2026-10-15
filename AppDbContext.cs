using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

public class AppDbContext : DbContext{
    //______________________________NVK EDIT______________________________
    /*|*/public DbSet<TaiKhoan> TaiKhoans {get; set;}
    /*|*/public DbSet<LoaiBan> LoaiBans {get; set;}
    /*|*/
    //|--------------------------------------------------------------------
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<TaiKhoan> TaiKhoans { get; set; }

    public DbSet<MonAn> MonAn { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
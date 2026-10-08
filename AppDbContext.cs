using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangNhom3_Uneti.Models;
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

    public DbSet<BanAn> BanAns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}

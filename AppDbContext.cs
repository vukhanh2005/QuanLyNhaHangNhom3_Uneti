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
    public DbSet<MonAn> MonAn { get; set; }

    public DbSet<BanAn> BanAns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<BanAn>().HasOne(x => x.LoaiBan).WithMany()
            .HasForeignKey(x => x.MaLoaiBan).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BanAn>().HasIndex(x => x.TenBan).IsUnique();
        modelBuilder.Entity<BanAn>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_BanAns_SoChoNgoi", "[SoChoNgoi] > 0");
            t.HasCheckConstraint("CK_BanAns_TrangThai",
                "[TrangThai] IN (N'Sẵn sàng', N'Đang sử dụng', N'Tạm ngừng phục vụ')");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}

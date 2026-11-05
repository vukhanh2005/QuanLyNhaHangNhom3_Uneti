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

    public DbSet<KhachHang> KhachHangs { get; set; }

    public DbSet<BanAn> BanAns { get; set; }

    public DbSet<PhieuDatBan> PhieuDatBans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PhieuDatBan>().HasOne(x => x.KhachHang).WithMany(x => x.PhieuDatBans)
            .HasForeignKey(x => x.MaKhachHang).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PhieuDatBan>().HasOne(x => x.BanAn).WithMany(x => x.PhieuDatBans)
            .HasForeignKey(x => x.MaBan).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PhieuDatBan>().Property(x => x.NgaySuDung).HasColumnType("date");
        modelBuilder.Entity<PhieuDatBan>().HasIndex(x => new { x.MaBan, x.NgaySuDung });
        modelBuilder.Entity<PhieuDatBan>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_PhieuDatBans_SoNguoi", "[SoNguoi] > 0");
            t.HasCheckConstraint("CK_PhieuDatBans_ThoiGian", "[GioKetThuc] > [GioBatDau]");
            t.HasCheckConstraint("CK_PhieuDatBans_TrangThai", "[TrangThai] IN (0, 1, 2, 3)");
        });
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

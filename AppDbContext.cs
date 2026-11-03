using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

public class AppDbContext : DbContext
{
    public DbSet<TaiKhoan> TaiKhoans { get; set; }
    public DbSet<BanAn> BanAns { get; set; }
    public DbSet<MonAn> MonAns { get; set; }
    public DbSet<MonAn> MonAn { get; set; }
    public DbSet<LoaiBan> LoaiBans { get; set; }
    public DbSet<KhachHang> KhachHangs { get; set; }
    public DbSet<PhieuDatBan> PhieuDatBans { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MonAn>().ToTable("MonAn");

        modelBuilder.Entity<BanAn>()
            .HasOne(x => x.LoaiBan)
            .WithMany(x => x.BanAns)
            .HasForeignKey(x => x.MaLoaiBan)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BanAn>()
            .HasIndex(x => x.TenBan)
            .IsUnique();

        modelBuilder.Entity<BanAn>().ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_BanAns_SoChoNgoi",
                "[SoChoNgoi] > 0");

            t.HasCheckConstraint(
                "CK_BanAns_TrangThai",
                "[TrangThai] IN (N'Sẵn sàng', N'Đang sử dụng', N'Tạm ngừng phục vụ')");
        });

        modelBuilder.Entity<KhachHang>()
            .HasOne(x => x.TaiKhoan)
            .WithOne(x => x.KhachHang)
            .HasForeignKey<KhachHang>(x => x.MaTaiKhoan)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PhieuDatBan>()
            .HasOne(x => x.KhachHang)
            .WithMany(x => x.PhieuDatBans)
            .HasForeignKey(x => x.MaKhachHang)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PhieuDatBan>()
            .HasOne(x => x.BanAn)
            .WithMany(x => x.PhieuDatBans)
            .HasForeignKey(x => x.MaBan)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

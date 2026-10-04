using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangNhom3_Uneti.Models;

public class AppDbContext : DbContext{
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

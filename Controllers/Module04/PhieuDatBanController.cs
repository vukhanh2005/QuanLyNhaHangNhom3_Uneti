using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.ViewModels.Module04;

namespace QuanLyNhaHang.Controllers.Module04
{
public class PhieuDatBanController : Controller
{
private readonly AppDbContext _context;


    public PhieuDatBanController(AppDbContext context)
    {
        _context = context;
    }

    
    public async Task<IActionResult> Index(
        string? tenKhachHang,
        string? tenBan,
        string? trangThai,
        DateTime? ngaySuDung)
    {
        var query = _context.PhieuDatBans
            .Include(p => p.KhachHang)
            .Include(p => p.BanAn)
            .AsQueryable();

        
        if (!string.IsNullOrWhiteSpace(tenKhachHang))
        {
            query = query.Where(p =>
                p.KhachHang != null &&
                p.KhachHang.HoTen.Contains(tenKhachHang));
        }

       
        if (!string.IsNullOrWhiteSpace(tenBan))
        {
            query = query.Where(p =>
                p.BanAn != null &&
                p.BanAn.TenBan.Contains(tenBan));
        }

     
        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            query = query.Where(p => p.TrangThai == trangThai);
        }

        
        if (ngaySuDung.HasValue)
        {
            var ngay = ngaySuDung.Value.Date;
            var ngayTiepTheo = ngay.AddDays(1);

            query = query.Where(p =>
                p.NgayGioDat >= ngay &&
                p.NgayGioDat < ngayTiepTheo);
        }

        var model = new PhieuDatBanViewModel
        {
            DanhSachPhieuDatBan = await query
                .OrderByDescending(p => p.NgayGioDat)
                .ToListAsync(),

            TenKhachHang = tenKhachHang,
            TenBan = tenBan,
            TrangThai = trangThai,
            NgaySuDung = ngaySuDung
        };

        return View(
            "~/Views/Module04/PhieuDatBan/Index.cshtml",
            model);
    }
}


}

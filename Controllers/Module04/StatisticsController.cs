using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.ViewModels.Module04;

namespace QuanLyNhaHang.Controllers.Module04
{
    public class StatisticsController : Controller
    {
        private readonly AppDbContext _context;

        private bool DaDangNhap =>
            HttpContext.Session.GetInt32("MaTaiKhoan").HasValue;

        private bool LaAdmin =>
            string.Equals(
                HttpContext.Session.GetString("VaiTro"),
                "Admin",
                StringComparison.OrdinalIgnoreCase);

        public StatisticsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Chưa đăng nhập
            if (!DaDangNhap)
                return RedirectToAction("Login", "Account");

            // Không phải Admin
            if (!LaAdmin)
                return RedirectToAction("Index", "TrangChu");

            int tongSoBan = await _context.BanAns.CountAsync();

            int tongSoMonAn = await _context.MonAns.CountAsync();

            int tongSoPhieuDatBan = await _context.PhieuDatBans.CountAsync();

            var soBanTheoLoai = await _context.BanAns
                .Include(b => b.LoaiBan)
                .GroupBy(b => b.LoaiBan != null
                    ? b.LoaiBan.TenLoaiBan
                    : "Chưa phân loại")
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            var soBanTheoTrangThai = await _context.BanAns
                .GroupBy(b => b.TrangThai)
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            var phieuDatBanTheoTrangThai = await _context.PhieuDatBans
                .GroupBy(p => p.TrangThai)
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            var model = new StatisticsViewModel
            {
                TongSoBan = tongSoBan,
                TongSoMonAn = tongSoMonAn,
                TongSoPhieuDatBan = tongSoPhieuDatBan,
                SoBanTheoLoai = soBanTheoLoai,
                SoBanTheoTrangThai = soBanTheoTrangThai,
                PhieuDatBanTheoTrangThai = phieuDatBanTheoTrangThai
            };

            return View(
                "~/Views/Module04/Statistics/Index.cshtml",
                model
            );
        }
    }
}

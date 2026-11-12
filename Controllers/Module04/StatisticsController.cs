using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.ViewModels.Module04;

namespace QuanLyNhaHang.Controllers.Module04
{
    public class StatisticsController : Controller
    {
        private readonly AppDbContext _context;

        public StatisticsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!HttpContext.Session.GetInt32("MaTaiKhoan").HasValue)
                return RedirectToAction("Login", "Account");

            if (!string.Equals(
                HttpContext.Session.GetString("VaiTro"),
                "Admin",
                StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "TrangChu");

            var model = new StatisticsViewModel
            {
                TongSoBan = await _context.BanAns.CountAsync(),
                TongSoMonAn = await _context.MonAns.CountAsync(),
                TongSoPhieuDatBan = await _context.PhieuDatBans.CountAsync()
            };

            // 1. Số bàn theo loại bàn
            model.SoBanTheoLoai = await _context.BanAns
                .GroupBy(b => b.LoaiBan != null
                    ? b.LoaiBan.TenLoaiBan : "Chưa phân loại")
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // 2. Số bàn theo trạng thái
            model.SoBanTheoTrangThai = await _context.BanAns
                .GroupBy(b => b.TrangThai)
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // 3. Phiếu đặt bàn theo trạng thái
            model.PhieuDatBanTheoTrangThai = await _context.PhieuDatBans
                .GroupBy(p => p.TrangThai)
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // 4. Số phiếu theo từng bàn
            model.PhieuTheoBan = await _context.PhieuDatBans
                .GroupBy(p => p.BanAn != null
                    ? p.BanAn.TenBan : "Không xác định")
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // 5. Số phiếu theo loại bàn
            model.PhieuTheoLoaiBan = await _context.PhieuDatBans
                .GroupBy(p => p.BanAn != null && p.BanAn.LoaiBan != null
                    ? p.BanAn.LoaiBan.TenLoaiBan : "Chưa phân loại")
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // 6. Bàn được đặt nhiều nhất (bao gồm trường hợp đồng hạng)
            var soPhieuTheoBan = await _context.PhieuDatBans
                .GroupBy(p => p.BanAn != null
                    ? p.BanAn.TenBan : "Không xác định")
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .ToListAsync();

            int soLuongDatCaoNhat = soPhieuTheoBan
                .Select(x => x.SoLuong)
                .DefaultIfEmpty(0)
                .Max();

            model.BanDuocDatNhieuNhat = soPhieuTheoBan
                .Where(x => x.SoLuong == soLuongDatCaoNhat && soLuongDatCaoNhat > 0)
                .OrderBy(x => x.Nhom)
                .ToList();

            // 7. Số phiếu theo khách hàng
            model.PhieuTheoKhachHang = await _context.PhieuDatBans
                .GroupBy(p => p.KhachHang != null
                    ? p.KhachHang.HoTen : "Không xác định")
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .Take(20)
                .ToListAsync();

            // 8. Số phiếu theo tháng đặt
            var duLieuTheoThang = await _context.PhieuDatBans
                .GroupBy(p => new
                {
                    p.NgayGioDat.Year,
                    p.NgayGioDat.Month
                })
                .Select(g => new
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    SoLuong = g.Count()
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .ToListAsync();

            model.PhieuTheoThang = duLieuTheoThang
                .Select(x => new ThongKeViewModel
                {
                    Nhom = $"Tháng {x.Thang:D2}/{x.Nam}",
                    SoLuong = x.SoLuong
                })
                .ToList();

            return View("~/Views/Module04/Statistics/Index.cshtml", model);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.ViewModels.Module04;

namespace QuanLyNhaHang.Controllers.Module04
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
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

            var model = new DashboardViewModel
            {
                TongSoLoaiBan = await _context.LoaiBans.CountAsync(),
                TongSoBan = await _context.BanAns.CountAsync(),
                TongSoMonAn = await _context.MonAns.CountAsync(),
                TongSoKhachHang = await _context.KhachHangs.CountAsync(),
                TongSoPhieuDatBan = await _context.PhieuDatBans.CountAsync(),

                SoPhieuChoXacNhan = await _context.PhieuDatBans
                    .CountAsync(p => p.TrangThai == "Chờ xác nhận"),

                SoPhieuDaXacNhan = await _context.PhieuDatBans
                    .CountAsync(p => p.TrangThai == "Đã xác nhận"),

                SoBanDangSuDung = await _context.BanAns
                    .CountAsync(b => b.TrangThai == "Đang sử dụng"),

                SoBanSanSang = await _context.BanAns
                    .CountAsync(b => b.TrangThai == "Sẵn sàng"),

                SoBanTheoLoai = await _context.BanAns
                    .GroupBy(b => b.LoaiBan != null
                        ? b.LoaiBan.TenLoaiBan
                        : "Chưa phân loại")
                    .Select(g => new ThongKeBanViewModel
                    {
                        Nhom = g.Key,
                        SoLuong = g.Count()
                    })
                    .OrderBy(x => x.Nhom)
                    .ToListAsync(),

                SoBanTheoTrangThai = await _context.BanAns
                    .GroupBy(b => b.TrangThai)
                    .Select(g => new ThongKeBanViewModel
                    {
                        Nhom = g.Key,
                        SoLuong = g.Count()
                    })
                    .OrderBy(x => x.Nhom)
                    .ToListAsync()
            };

            return View("~/Views/Module04/Dashboard/Index.cshtml", model);
        }
    }
}

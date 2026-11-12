using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Service.Interface;
using QuanLyNhaHang.ViewModels.Module04;

namespace QuanLyNhaHang.Controllers.Module04
{
    public class SuDungBanController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPhieuDatBanService _phieuDatBanService;

        public SuDungBanController(
            AppDbContext context,
            IPhieuDatBanService phieuDatBanService)
        {
            _context = context;
            _phieuDatBanService = phieuDatBanService;
        }

        private bool LaAdmin =>
            HttpContext.Session.GetInt32("MaTaiKhoan").HasValue &&
            string.Equals(
                HttpContext.Session.GetString("VaiTro"),
                "Admin",
                StringComparison.OrdinalIgnoreCase);

        public async Task<IActionResult> Index()
        {
            if (!HttpContext.Session.GetInt32("MaTaiKhoan").HasValue)
                return RedirectToAction("Login", "Account");

            if (!LaAdmin)
                return RedirectToAction("Index", "TrangChu");

            var danhSach = await _context.PhieuDatBans
                .Include(p => p.KhachHang)
                .Include(p => p.BanAn)
                    .ThenInclude(b => b!.LoaiBan)
                .Where(p => p.TrangThai == "Đã xác nhận"
                         || p.TrangThai == "Đang sử dụng")
                .OrderBy(p => p.NgayGioDat)
                .Select(p => new SuDungBanViewModel
                {
                    MaPhieuDatBan = p.MaPhieuDatBan,
                    TenKhachHang = p.KhachHang != null
                        ? p.KhachHang.HoTen : "Không xác định",
                    TenBan = p.BanAn != null
                        ? p.BanAn.TenBan : "Không xác định",
                    TenLoaiBan = p.BanAn != null && p.BanAn.LoaiBan != null
                        ? p.BanAn.LoaiBan.TenLoaiBan : "Chưa phân loại",
                    NgayGioDat = p.NgayGioDat,
                    SoNguoi = p.SoNguoi,
                    TrangThai = p.TrangThai,
                    GhiChu = p.GhiChu
                })
                .ToListAsync();

            ViewData["Title"] = "Sử dụng bàn";

            return View("~/Views/Module04/SuDungBan/Index.cshtml", danhSach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatDauSuDung(int id)
        {
            if (!LaAdmin)
                return RedirectToAction("Login", "Account");

            var ketQua = await _phieuDatBanService
                .CapNhatTrangThaiAsync(id, "Đang sử dụng");

            TempData[ketQua.ThanhCong ? "Success" : "Error"] =
                ketQua.ThongBao;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KetThucSuDung(int id)
        {
            if (!LaAdmin)
                return RedirectToAction("Login", "Account");

            var ketQua = await _phieuDatBanService
                .CapNhatTrangThaiAsync(id, "Đã sử dụng");

            TempData[ketQua.ThanhCong ? "Success" : "Error"] =
                ketQua.ThongBao;

            return RedirectToAction(nameof(Index));
        }
    }
}

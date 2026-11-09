using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Service.Interface;

namespace QuanLyNhaHang.Controllers.Module04
{
    public class PhieuDatBanController : Controller
    {
        private readonly IPhieuDatBanService _phieuDatBanService;

        public PhieuDatBanController(
            IPhieuDatBanService phieuDatBanService)
        {
            _phieuDatBanService = phieuDatBanService;
        }

        private int? MaTaiKhoan =>
            HttpContext.Session.GetInt32("MaTaiKhoan");

        private string? VaiTro =>
            HttpContext.Session.GetString("VaiTro");

        private bool DaDangNhap => MaTaiKhoan.HasValue;

        private bool LaAdmin =>
            string.Equals(
                VaiTro,
                "Admin",
                StringComparison.OrdinalIgnoreCase);

        private IActionResult VeTrangDangNhap()
        {
            return RedirectToAction("Login", "Account");
        }

        private IActionResult KhongCoQuyen()
        {
            return RedirectToAction("Index", "TrangChu");
        }

        // Danh sách phiếu đặt bàn
        public async Task<IActionResult> Index(
            string? tenKhachHang,
            string? tenBan,
            string? trangThai,
            DateTime? ngaySuDung)
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            if (LaAdmin)
            {
                var model = await _phieuDatBanService.GetDanhSachAsync(
                    tenKhachHang,
                    tenBan,
                    trangThai,
                    ngaySuDung);

                return View(
                    "~/Views/PhieuDatBan/Admin/Index.cshtml",
                    model);
            }

            if (!string.Equals(
                VaiTro,
                "User",
                StringComparison.OrdinalIgnoreCase))
            {
                return KhongCoQuyen();
            }

            var danhSachCuaToi =
                await _phieuDatBanService.GetDanhSachTheoTaiKhoanAsync(
                    MaTaiKhoan!.Value,
                    tenKhachHang,
                    tenBan,
                    trangThai,
                    ngaySuDung);

            return View(
                "~/Views/PhieuDatBan/Client/Index.cshtml",
                danhSachCuaToi);
        }

        // Xem chi tiết phiếu
        public async Task<IActionResult> Details(int id)
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            if (!LaAdmin &&
                !string.Equals(
                    VaiTro,
                    "User",
                    StringComparison.OrdinalIgnoreCase))
            {
                return KhongCoQuyen();
            }

            var phieu =
                await _phieuDatBanService.GetChiTietAsync(id);

            if (phieu == null)
                return NotFound();

            // Client chỉ được xem phiếu của mình
            if (!LaAdmin &&
                phieu.KhachHang?.MaTaiKhoan != MaTaiKhoan)
            {
                return Forbid();
            }

            if (LaAdmin)
            {
                return View(
                    "~/Views/PhieuDatBan/Admin/Details.cshtml",
                    phieu);
            }

            return View(
                "~/Views/PhieuDatBan/Client/Details.cshtml",
                phieu);
        }

        // Xác nhận, hủy, bắt đầu hoặc kết thúc sử dụng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            int id,
            string trangThaiMoi)
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            if (!LaAdmin)
                return KhongCoQuyen();

            var ketQua =
                await _phieuDatBanService.CapNhatTrangThaiAsync(
                    id,
                    trangThaiMoi);

            TempData[ketQua.ThanhCong
                ? "SuccessMessage"
                : "ErrorMessage"] = ketQua.ThongBao;

            return RedirectToAction(nameof(Details), new { id });
        }

        // Xác nhận phiếu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(int id)
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            if (!LaAdmin)
                return KhongCoQuyen();

            var ketQua =
                await _phieuDatBanService.CapNhatTrangThaiAsync(
                    id,
                    "Đã xác nhận");

            TempData[ketQua.ThanhCong
                ? "SuccessMessage"
                : "ErrorMessage"] = ketQua.ThongBao;

            return RedirectToAction(nameof(Details), new { id });
        }

        // Hủy phiếu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            if (!LaAdmin)
                return KhongCoQuyen();

            var ketQua =
                await _phieuDatBanService.CapNhatTrangThaiAsync(
                    id,
                    "Đã hủy");

            TempData[ketQua.ThanhCong
                ? "SuccessMessage"
                : "ErrorMessage"] = ketQua.ThongBao;

            return RedirectToAction(nameof(Details), new { id });
        }

        // Bắt đầu sử dụng bàn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatDauSuDung(int id)
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            if (!LaAdmin)
                return KhongCoQuyen();

            var ketQua =
                await _phieuDatBanService.CapNhatTrangThaiAsync(
                    id,
                    "Đang sử dụng");

            TempData[ketQua.ThanhCong
                ? "SuccessMessage"
                : "ErrorMessage"] = ketQua.ThongBao;

            return RedirectToAction(nameof(Details), new { id });
        }

        // Kết thúc sử dụng bàn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KetThucSuDung(int id)
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            if (!LaAdmin)
                return KhongCoQuyen();

            var ketQua =
                await _phieuDatBanService.CapNhatTrangThaiAsync(
                    id,
                    "Đã sử dụng");

            TempData[ketQua.ThanhCong
                ? "SuccessMessage"
                : "ErrorMessage"] = ketQua.ThongBao;

            return RedirectToAction(nameof(Details), new { id });
        }

        // Xóa phiếu đặt bàn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            if (!LaAdmin)
                return KhongCoQuyen();

            var ketQua =
                await _phieuDatBanService.XoaAsync(id);

            TempData[ketQua.ThanhCong
                ? "SuccessMessage"
                : "ErrorMessage"] = ketQua.ThongBao;

            return RedirectToAction(nameof(Index));
        }
    }
}

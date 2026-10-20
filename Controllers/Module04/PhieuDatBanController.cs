
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

        public async Task<IActionResult> Index(
            string? tenKhachHang,
            string? tenBan,
            string? trangThai,
            DateTime? ngaySuDung)
        {
            var model = await _phieuDatBanService.GetDanhSachAsync(
                tenKhachHang,
                tenBan,
                trangThai,
                ngaySuDung);

            return View(
                "~/Views/Module04/PhieuDatBan/Index.cshtml",
                model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var phieu = await _phieuDatBanService.GetChiTietAsync(id);

            if (phieu == null)
            {
                return NotFound();
            }

            return View(
                "~/Views/Module04/PhieuDatBan/Details.cshtml",
                phieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            int id,
            string trangThaiMoi)
        {
            var ketQua =
                await _phieuDatBanService.CapNhatTrangThaiAsync(
                    id,
                    trangThaiMoi);

            if (ketQua.ThanhCong)
            {
                TempData["SuccessMessage"] = ketQua.ThongBao;
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.ThongBao;
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}

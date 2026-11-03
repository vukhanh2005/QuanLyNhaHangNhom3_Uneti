using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Service.Interface;
using QuanLyNhaHang.ViewModels.Client;

namespace QuanLyNhaHang.Controllers.Client
{
    public class DatBanController : Controller
    {
        private readonly IClientDatBanService _service;

        public DatBanController(IClientDatBanService service)
        {
            _service = service;
        }

        private int? MaTaiKhoan =>
            HttpContext.Session.GetInt32("MaTaiKhoan");

        private bool DaDangNhap => MaTaiKhoan.HasValue;

        private IActionResult VeTrangDangNhap()
        {
            return RedirectToAction("Login", "Account");
        }

        // Hiển thị trang đặt bàn.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            var model = new TaoPhieuDatBanViewModel
            {
                DanhSachBan = await _service.GetDanhSachBanAsync(),
                NgayGioDat = DateTime.Now.AddDays(1)
            };

            return View("~/Views/Client/DatBan/Index.cshtml", model);
        }

        // Xử lý đặt bàn.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TaoPhieuDatBanViewModel model)
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            if (!ModelState.IsValid)
            {
                model.DanhSachBan =
                    await _service.GetDanhSachBanAsync();

                return View(
                    "~/Views/Client/DatBan/Index.cshtml",
                    model);
            }

            var ketQua = await _service.TaoPhieuDatBanAsync(
                model,
                MaTaiKhoan!.Value);

            if (!ketQua.ThanhCong)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ketQua.ThongBao);

                model.DanhSachBan =
                    await _service.GetDanhSachBanAsync();

                return View(
                    "~/Views/Client/DatBan/Index.cshtml",
                    model);
            }

            TempData["SuccessMessage"] = ketQua.ThongBao;

            return RedirectToAction(nameof(Index));
        }

        // Lịch sử đặt bàn của tài khoản hiện tại.
        [HttpGet]
        public async Task<IActionResult> LichSu()
        {
            if (!DaDangNhap)
                return VeTrangDangNhap();

            var danhSach = await _service.GetLichSuAsync(
                MaTaiKhoan!.Value);

            return View(
                "~/Views/Client/DatBan/LichSu.cshtml",
                danhSach);
        }
    }
}

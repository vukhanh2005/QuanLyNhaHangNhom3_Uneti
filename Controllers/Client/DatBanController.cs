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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new TaoPhieuDatBanViewModel
            {
                DanhSachBan = await _service.GetDanhSachBanAsync(),
                NgayGioDat = DateTime.Now.AddDays(1)
            };

            return View("~/Views/Client/DatBan/Index.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaoPhieuDatBanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DanhSachBan = await _service.GetDanhSachBanAsync();
                return View("~/Views/Client/DatBan/Index.cshtml", model);
            }

            var ketQua = await _service.TaoPhieuDatBanAsync(model);

            if (!ketQua.ThanhCong)
            {
                ModelState.AddModelError(string.Empty, ketQua.ThongBao);
                model.DanhSachBan = await _service.GetDanhSachBanAsync();

                return View("~/Views/Client/DatBan/Index.cshtml", model);
            }

            TempData["SuccessMessage"] = ketQua.ThongBao;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> LichSu()
        {
            // Chỉ lấy email từ tài khoản đã đăng nhập
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] =
                    "Vui lòng đăng nhập để xem lịch sử đặt bàn.";

                return RedirectToAction("Login", "Account",
                    new { area = "" });
            }

            var danhSach = await _service.GetLichSuAsync(email);

            return View("~/Views/Client/DatBan/LichSu.cshtml", danhSach);
        }
    }
}

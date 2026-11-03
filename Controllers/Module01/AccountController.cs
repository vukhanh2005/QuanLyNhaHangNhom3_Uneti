using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Service.Interface;

namespace QuanLyNhaHang.Controllers.Module01
{
    public class AccountController : Controller
    {
        private readonly IAuthService service;

        public AccountController(IAuthService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(TaiKhoanRequest request)
        {
            TaiKhoanResponse response = await service.Login(request);

            if (response.message != null &&
                response.message.StartsWith("[SUCCESS]"))
            {
                HttpContext.Session.SetInt32(
                    "MaTaiKhoan", response.maTaiKhoan);

                HttpContext.Session.SetString(
                    "TenDangNhap", response.username ?? "");

                HttpContext.Session.SetString(
                    "HoTen", response.hoTen ?? "");

                HttpContext.Session.SetString(
                    "Email", response.email ?? "");

                HttpContext.Session.SetString(
                    "VaiTro", response.vaiTro ?? "User");

                return RedirectToAction("Index", "TrangChu");
            }

            ViewBag.FailMessage = response.message ?? "Đăng nhập thất bại.";
            return View("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> Register(TaiKhoanRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FailMessage =
                    "Thông tin đăng ký không hợp lệ. Vui lòng kiểm tra lại.";
                return View("Register");
            }

            TaiKhoanResponse response = await service.Register(request);

            if (response.message != null &&
                response.message.StartsWith("[SUCCESS]"))
            {
                ViewBag.SuccessMessage =
                    response.message.Substring("[SUCCESS]".Length).Trim();

                return View("Register");
            }

            ViewBag.FailMessage = response.message ?? "Đăng ký thất bại.";
            return View("Register");
        }

        [HttpGet]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}

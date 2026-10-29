using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Repository;
using QuanLyNhaHang.Service;

namespace QuanLyNhaHang.Controllers.Module01
{
    public class AccountController : Controller
    {
        private readonly LoginService service;

        public AccountController(AccountRepository repository)
        {
            service = new LoginService(repository);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(TaiKhoanRequest request)
        {
            TaiKhoanResponse response = await service.Login(request);

            if (response.message != null &&
                response.message.StartsWith("[SUCCESS]"))
            {
                // Lưu thông tin tài khoản vào Session
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

            ViewData["Message"] =
                response.message != null &&
                response.message.Length >= 7
                    ? response.message.Substring(7)
                    : "Đăng nhập thất bại.";

            return View("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(TaiKhoanRequest request)
        {
            TaiKhoanResponse response = await service.Register(request);

            if (response.message != null &&
                response.message.StartsWith("[SUCCESS]"))
            {
                ViewBag.SuccessMessage = response.message.Substring(6);
                return View("Register");
            }

            ViewBag.FailMessage = response.message;
            return View("Register");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}

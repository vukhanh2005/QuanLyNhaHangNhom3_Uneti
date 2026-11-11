using Azure;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Repository;
using QuanLyNhaHang.Service;
using QuanLyNhaHang.Service.Implement;
using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Controllers.Module01
{
    public class AccountController : Controller
    {
        private readonly IAuthService service;

        public AccountController(IAuthService service)
        {
            this.service =service;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(TaiKhoanRequest request)
        {
            bool isSave = request.RememberMe;
            TaiKhoanResponse response = await service.Login(request);

            if (response.message != null && response.message.StartsWith("[SUCCESS]"))
            {

                if (isSave)
                {
                    // Lưu vào session theo yêu cầu đề tài
                    HttpContext.Session.SetInt32("MaTaiKhoan", response.maTaiKhoan);
                    HttpContext.Session.SetString("HoTen", response.hoTen ?? "");
                    HttpContext.Session.SetString("VaiTro", response.vaiTro ?? "");
                }
                return RedirectToAction("Index", "TrangChu", new {role = response.vaiTro});
            }
            else
            {
                ViewBag.FailMessage = response.message ?? "Đăng nhập thất bại.";
                return View("Login");
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(TaiKhoanRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FailMessage = "Thông tin đăng ký không hợp lệ. Vui lòng kiểm tra lại các trường đã nhập.";
                return View("Register");
            }

            TaiKhoanResponse response = await service.Register(request);

            if (response.message != null && response.message.StartsWith("[SUCCESS]"))
            {
                ViewBag.SuccessMessage = response.message.Substring(9);
                return View("Register");
            }
            else
            {
                ViewBag.FailMessage = response.message;
                return View("Register");
            }
        }
       public IActionResult DangXuat()
        {
            // Xóa session hiện tại
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}

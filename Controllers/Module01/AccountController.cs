using Azure;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Repository;
using QuanLyNhaHang.Service;

namespace QuanLyNhaHang.Controllers.Module01
{
    public class AccountController : Controller
    {
        LoginService service = null;
        public AccountController(AccountRepository repository)
        {
            this.service = new LoginService(repository);
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
    
            if (response.message.StartsWith("[SUCCESS]"))
            {
                //Luu vao session
                HttpContext.Session.SetInt32("MaTaiKhoan", response.maTaiKhoan);
                HttpContext.Session.SetString("HoTen", response.hoTen);
                HttpContext.Session.SetString("VaiTro", response.vaiTro);
                ViewBag.SuccessMessage = response.message.Substring(7);
                return View("Login");
            }
            else
            {
                ViewData["Message"] = response.message.Substring(7);
                ViewBag.FailMessage = response.message;
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
            // System.Console.WriteLine(
            //     $"Username: {request.username}\n Password: {request.password} \n Confirm Password: {request.confirmPassword}"+
            //     $"Name: {request.hoTen}\n Email: {request.email}"
            // );
            TaiKhoanResponse response = await service.Register(request);

            if (response.message.StartsWith("[SUCCESS]"))
            {
                // System.Console.WriteLine($"Dang ki thanh cong tai khaon :{response.username}-{response.password}");
                ViewBag.SuccessMessage = response.message.Substring(9);
                return View("Register");
            }
            else
            {
                //System.Console.WriteLine($"Dang ki that bai: {response.message}");
                ViewBag.FailMessage = response.message;
                return View("Register");
            }
        }
    }
}
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
                return RedirectToAction("Index", "TrangChu");
            }
            else
            {
                ViewData["Message"] = response.message.Substring(7);
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
            System.Console.WriteLine(
                $"Username: {request.username}\n Password: {request.password} \n Confirm Password: {request.confirmPassword}"+
                $"Name: {request.hoTen}\n Email: {request.email}"
            );
            return View("Test");
        }
    }
}
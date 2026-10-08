using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Models;
using System.Diagnostics;

namespace QuanLyNhaHang.Controllers
{
    public class TrangChuController : Controller
    {
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                //Da dang nhap roi
                return View();
            }
            else
            {
                //Direct den trang dang nhap
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

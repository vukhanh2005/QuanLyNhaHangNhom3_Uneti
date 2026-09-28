using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaHang.Controllers.Module01
{
    public class DangNhapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
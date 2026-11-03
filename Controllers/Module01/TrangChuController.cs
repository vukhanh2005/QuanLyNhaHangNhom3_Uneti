using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Models;
using System.Diagnostics;

namespace QuanLyNhaHang.Controllers
{
    public class TrangChuController : Controller
    {
        private readonly ILoaiBanService loaiBanService;
        private readonly IMonAnService monAnService;

        public TrangChuController(ILoaiBanService loaiBanService, IMonAnService monAnService)
        {
            this.loaiBanService = loaiBanService;
            this.monAnService = monAnService;
        }

        public async Task<IActionResult> Index()
        {
            if(HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                //Da dang nhap roi
                if(HttpContext.Session.GetString("VaiTro") == nameof(VaiTro.Admin))
                {
                    // Hai service dùng chung DbContext scoped, nên truy vấn tuần tự.
                    var loaiBans = await loaiBanService.GetAllLoaiBan();
                    var monAns = await monAnService.GetAll();
                    return View("Admin", new DashboardViewModel
                    {
                        TongLoaiBan = loaiBans.Count,
                        LoaiBanHoatDong = loaiBans.Count(x => x.TrangThai),
                        TongMonAn = monAns.Count,
                        MonAnDangPhucVu = monAns.Count(x => x.TrangThai == TrangThaiMonAn.DangPhucVu)
                    });
                }
                else
                {
                    return View("Client");
                }
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

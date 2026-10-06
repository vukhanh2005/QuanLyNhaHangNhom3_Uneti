using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.ViewModels.Module04;

namespace QuanLyNhaHang.Controllers.Module04
{
    public class StatisticsController : Controller
    {
        private readonly AppDbContext _context;

        public StatisticsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Tổng số bàn
            int tongSoBan = await _context.BanAns.CountAsync();

            // Tổng số món ăn
            int tongSoMonAn = await _context.MonAns.CountAsync();

            // Thống kê số bàn theo loại
            var soBanTheoLoai = await _context.BanAns
                .GroupBy(b => b.MaLoaiBan)
                .Select(g => new ThongKeViewModel
                {
                    Nhom = "Loại bàn " + g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // Thống kê số bàn theo trạng thái
            var soBanTheoTrangThai = await _context.BanAns
                .GroupBy(b => b.TrangThai)
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            var model = new StatisticsViewModel
            {
                TongSoBan = tongSoBan,
                TongSoMonAn = tongSoMonAn,
                SoBanTheoLoai = soBanTheoLoai,
                SoBanTheoTrangThai = soBanTheoTrangThai
            };

            return View("~/Views/Module04/Statistics/Index.cshtml", model);
        }
    }
}
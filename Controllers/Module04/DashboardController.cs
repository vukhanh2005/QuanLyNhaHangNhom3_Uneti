using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.ViewModels.Module04;

namespace QuanLyNhaHang.Controllers.Module04
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tongSoBan = await _context.BanAns.CountAsync();

            var tongSoMonAn = await _context.MonAns.CountAsync();

            var tongSoLoaiBan = await _context.BanAns
                .Select(b => b.MaLoaiBan)
                .Distinct()
                .CountAsync();

            var soBanDangSuDung = await _context.BanAns
                .CountAsync(b => b.TrangThai == "Đang sử dụng");

            var soBanSanSang = await _context.BanAns
                .CountAsync(b => b.TrangThai == "Sẵn sàng");

            // Thống kê số bàn theo tên loại bàn
            var soBanTheoLoai = await _context.BanAns
                .Include(b => b.LoaiBan)
                .GroupBy(b => b.LoaiBan != null
                    ? b.LoaiBan.TenLoaiBan
                    : "Chưa phân loại")
                .Select(g => new ThongKeBanViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderBy(x => x.Nhom)
                .ToListAsync();

            // Thống kê số bàn theo trạng thái
            var soBanTheoTrangThai = await _context.BanAns
                .GroupBy(b => b.TrangThai)
                .Select(g => new ThongKeBanViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderBy(x => x.Nhom)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TongSoLoaiBan = tongSoLoaiBan,
                TongSoBan = tongSoBan,
                TongSoMonAn = tongSoMonAn,
                SoBanDangSuDung = soBanDangSuDung,
                SoBanSanSang = soBanSanSang,
                SoBanTheoLoai = soBanTheoLoai,
                SoBanTheoTrangThai = soBanTheoTrangThai
            };

            return View(
                "~/Views/Module04/Dashboard/Index.cshtml",
                model);
        }
    }
}

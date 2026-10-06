using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // Tổng số bàn ăn
            int tongSoBan = await _context.BanAns.CountAsync();

            // Tổng số món ăn
            int tongSoMonAn = await _context.MonAns.CountAsync();

            // Số bàn đang sử dụng
            int soBanDangSuDung = await _context.BanAns
                .CountAsync(b => b.TrangThai == "Đang sử dụng");

            // Số bàn sẵn sàng
            int soBanSanSang = await _context.BanAns
                .CountAsync(b => b.TrangThai == "Sẵn sàng");

            // Tạm thời thống kê số loại bàn dựa trên MaLoaiBan
            int tongSoLoaiBan = await _context.BanAns
                .Select(b => b.MaLoaiBan)
                .Distinct()
                .CountAsync();

            // Số bàn theo loại
            var soBanTheoLoai = await _context.BanAns
                .GroupBy(b => b.MaLoaiBan)
                .Select(g => new ThongKeBanViewModel
                {
                    Nhom = "Loại bàn " + g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // Số bàn theo trạng thái
            var soBanTheoTrangThai = await _context.BanAns
                .GroupBy(b => b.TrangThai)
                .Select(g => new ThongKeBanViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
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

            return View(model);
        }
    }
}
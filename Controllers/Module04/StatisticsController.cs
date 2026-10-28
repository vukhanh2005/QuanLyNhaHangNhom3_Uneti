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

            // Tổng số phiếu đặt bàn
            int tongSoPhieuDatBan = await _context.PhieuDatBans.CountAsync();

            // Thống kê số bàn theo tên loại bàn
            var soBanTheoLoai = await _context.BanAns
                .Include(b => b.LoaiBan)
                .GroupBy(b => b.LoaiBan != null
                    ? b.LoaiBan.TenLoaiBan
                    : "Chưa phân loại")
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
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

            // Thống kê phiếu đặt bàn theo trạng thái
            var phieuDatBanTheoTrangThai = await _context.PhieuDatBans
                .GroupBy(p => p.TrangThai)
                .Select(g => new ThongKeViewModel
                {
                    Nhom = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // Tạo ViewModel truyền dữ liệu sang giao diện
            var model = new StatisticsViewModel
            {
                TongSoBan = tongSoBan,
                TongSoMonAn = tongSoMonAn,
                TongSoPhieuDatBan = tongSoPhieuDatBan,
                SoBanTheoLoai = soBanTheoLoai,
                SoBanTheoTrangThai = soBanTheoTrangThai,
                PhieuDatBanTheoTrangThai = phieuDatBanTheoTrangThai
            };

            return View(
                "~/Views/Module04/Statistics/Index.cshtml",
                model
            );
        }
    }
}

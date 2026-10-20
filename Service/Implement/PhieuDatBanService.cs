
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Service.Interface;
using QuanLyNhaHang.ViewModels.Module04;

namespace QuanLyNhaHang.Service.Implement
{
    public class PhieuDatBanService : IPhieuDatBanService
    {
        private readonly AppDbContext _context;

        public PhieuDatBanService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách phiếu đặt bàn và lọc dữ liệu
        public async Task<PhieuDatBanViewModel> GetDanhSachAsync(
            string? tenKhachHang,
            string? tenBan,
            string? trangThai,
            DateTime? ngaySuDung)
        {
            var query = _context.PhieuDatBans
                .AsNoTracking()
                .Include(p => p.KhachHang)
                .Include(p => p.BanAn)
                .AsQueryable();

            // Tìm theo tên khách hàng
            if (!string.IsNullOrWhiteSpace(tenKhachHang))
            {
                var ten = tenKhachHang.Trim();

                query = query.Where(p =>
                    p.KhachHang != null &&
                    p.KhachHang.HoTen.Contains(ten));
            }

            // Tìm theo tên bàn
            if (!string.IsNullOrWhiteSpace(tenBan))
            {
                var ten = tenBan.Trim();

                query = query.Where(p =>
                    p.BanAn != null &&
                    p.BanAn.TenBan.Contains(ten));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                var status = trangThai.Trim();

                query = query.Where(p => p.TrangThai == status);
            }

            // Lọc theo ngày sử dụng
            if (ngaySuDung.HasValue)
            {
                var ngay = ngaySuDung.Value.Date;
                var ngayTiepTheo = ngay.AddDays(1);

                query = query.Where(p =>
                    p.NgayGioDat >= ngay &&
                    p.NgayGioDat < ngayTiepTheo);
            }

            var danhSach = await query
                .OrderByDescending(p => p.NgayGioDat)
                .ToListAsync();

            return new PhieuDatBanViewModel
            {
                DanhSachPhieuDatBan = danhSach,
                TenKhachHang = tenKhachHang,
                TenBan = tenBan,
                TrangThai = trangThai,
                NgaySuDung = ngaySuDung
            };
        }

        // Lấy chi tiết một phiếu đặt bàn
        public async Task<PhieuDatBan?> GetChiTietAsync(int id)
        {
            return await _context.PhieuDatBans
                .AsNoTracking()
                .Include(p => p.KhachHang)
                .Include(p => p.BanAn)
                    .ThenInclude(b => b!.LoaiBan)
                .FirstOrDefaultAsync(
                    p => p.MaPhieuDatBan == id);
        }

        // Cập nhật trạng thái phiếu đặt bàn
        public async Task<(bool ThanhCong, string ThongBao)>
            CapNhatTrangThaiAsync(int id, string trangThaiMoi)
        {
            var phieu = await _context.PhieuDatBans
                .FirstOrDefaultAsync(
                    p => p.MaPhieuDatBan == id);

            if (phieu == null)
            {
                return (
                    false,
                    "Không tìm thấy phiếu đặt bàn.");
            }

            if (string.IsNullOrWhiteSpace(trangThaiMoi))
            {
                return (
                    false,
                    "Vui lòng chọn trạng thái.");
            }

            var trangThaiHopLe = new[]
            {
                "Chờ xác nhận",
                "Đã xác nhận",
                "Đã hủy",
                "Đã sử dụng"
            };

            if (!trangThaiHopLe.Contains(trangThaiMoi))
            {
                return (
                    false,
                    "Trạng thái được chọn không hợp lệ.");
            }

            // Kiểm tra quy tắc chuyển trạng thái
            var duocChuyen = phieu.TrangThai switch
            {
                "Chờ xác nhận" =>
                    trangThaiMoi == "Đã xác nhận" ||
                    trangThaiMoi == "Đã hủy",

                "Đã xác nhận" =>
                    trangThaiMoi == "Đã sử dụng" ||
                    trangThaiMoi == "Đã hủy",

                _ => false
            };

            if (!duocChuyen)
            {
                return (
                    false,
                    $"Không thể chuyển từ '{phieu.TrangThai}' " +
                    $"sang '{trangThaiMoi}'.");
            }

            phieu.TrangThai = trangThaiMoi;

            await _context.SaveChangesAsync();

            return (
                true,
                "Cập nhật trạng thái phiếu đặt bàn thành công.");
        }
    }
}

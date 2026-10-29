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

            if (!string.IsNullOrWhiteSpace(tenKhachHang))
            {
                var ten = tenKhachHang.Trim();

                query = query.Where(p =>
                    p.KhachHang != null &&
                    p.KhachHang.HoTen.Contains(ten));
            }

            if (!string.IsNullOrWhiteSpace(tenBan))
            {
                var ten = tenBan.Trim();

                query = query.Where(p =>
                    p.BanAn != null &&
                    p.BanAn.TenBan.Contains(ten));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                var status = trangThai.Trim();
                query = query.Where(p => p.TrangThai == status);
            }

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

        // Lấy chi tiết phiếu đặt bàn
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

        // Cập nhật trạng thái phiếu và trạng thái bàn
        public async Task<(bool ThanhCong, string ThongBao)>
            CapNhatTrangThaiAsync(int id, string trangThaiMoi)
        {
            if (string.IsNullOrWhiteSpace(trangThaiMoi))
            {
                return (false, "Vui lòng chọn trạng thái.");
            }

            var trangThaiHopLe = new[]
            {
                "Chờ xác nhận",
                "Đã xác nhận",
                "Đang sử dụng",
                "Đã sử dụng",
                "Đã hủy"
            };

            if (!trangThaiHopLe.Contains(trangThaiMoi))
            {
                return (false, "Trạng thái được chọn không hợp lệ.");
            }

            var phieu = await _context.PhieuDatBans
                .Include(p => p.BanAn)
                .FirstOrDefaultAsync(
                    p => p.MaPhieuDatBan == id);

            if (phieu == null)
            {
                return (false, "Không tìm thấy phiếu đặt bàn.");
            }

            var trangThaiCu = phieu.TrangThai;
            var ban = phieu.BanAn;

            // Kiểm tra quy tắc chuyển trạng thái
            var duocChuyen = trangThaiCu switch
            {
                "Chờ xác nhận" =>
                    trangThaiMoi == "Đã xác nhận" ||
                    trangThaiMoi == "Đã hủy",

                "Đã xác nhận" =>
                    trangThaiMoi == "Đang sử dụng" ||
                    trangThaiMoi == "Đã hủy",

                "Đang sử dụng" =>
                    trangThaiMoi == "Đã sử dụng",

                _ => false
            };

            if (!duocChuyen)
            {
                return (
                    false,
                    $"Không thể chuyển từ '{trangThaiCu}' " +
                    $"sang '{trangThaiMoi}'.");
            }

            // Mỗi lượt đặt bàn kéo dài 2 giờ.
const int thoiLuongDatBanGio = 2;

if (trangThaiMoi == "Đã xác nhận" ||
    trangThaiMoi == "Đang sử dụng")
{
    if (ban == null)
        return (false, "Phiếu chưa được gán bàn.");

    var batDau = phieu.NgayGioDat;
    var ketThuc = batDau.AddHours(thoiLuongDatBanGio);

    var coPhieuTrung = await _context.PhieuDatBans
        .AnyAsync(p =>
            p.MaPhieuDatBan != id &&
            p.MaBan == ban.MaBan &&
            p.TrangThai != "Đã hủy" &&
            p.NgayGioDat < ketThuc &&
            p.NgayGioDat.AddHours(thoiLuongDatBanGio) > batDau);

    if (coPhieuTrung)
    {
        return (
            false,
            "Bàn đã có phiếu đặt trùng khung giờ. Vui lòng kiểm tra lại.");
    }
}
            // Bắt đầu sử dụng bàn
            if (trangThaiMoi == "Đang sử dụng")
            {
                if (ban == null)
                {
                    return (false, "Phiếu chưa được gán bàn.");
                }

                if (ban.TrangThai != "Sẵn sàng")
                {
                    return (false, "Bàn hiện không sẵn sàng.");
                }

                ban.TrangThai = "Đang sử dụng";
            }

            // Kết thúc sử dụng bàn
            if (trangThaiMoi == "Đã sử dụng")
            {
                if (ban == null ||
                    ban.TrangThai != "Đang sử dụng")
                {
                    return (
                        false,
                        "Trạng thái bàn không hợp lệ.");
                }

                ban.TrangThai = "Sẵn sàng";
            }

            phieu.TrangThai = trangThaiMoi;

            await _context.SaveChangesAsync();

            return (
                true,
                "Cập nhật trạng thái phiếu đặt bàn thành công.");
        }
    }
}

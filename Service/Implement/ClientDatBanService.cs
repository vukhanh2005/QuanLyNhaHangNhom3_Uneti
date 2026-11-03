using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Service.Interface;
using QuanLyNhaHang.ViewModels.Client;

namespace QuanLyNhaHang.Service.Implement
{
    public class ClientDatBanService : IClientDatBanService
    {
        private readonly AppDbContext _context;

        // Một lượt đặt bàn kéo dài 2 tiếng.
        private const int ThoiLuongDatBanGio = 2;

        public ClientDatBanService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách bàn còn sẵn sàng.
        public async Task<List<BanAn>> GetDanhSachBanAsync()
        {
            return await _context.BanAns
                .AsNoTracking()
                .Include(b => b.LoaiBan)
                .Where(b => b.TrangThai == "Sẵn sàng")
                .OrderBy(b => b.SoChoNgoi)
                .ThenBy(b => b.TenBan)
                .ToListAsync();
        }

        // Tạo phiếu đặt bàn cho tài khoản đang đăng nhập.
        public async Task<(bool ThanhCong, string ThongBao)>
            TaoPhieuDatBanAsync(
                TaoPhieuDatBanViewModel model,
                int maTaiKhoan)
        {
            if (model.MaBan == null)
                return (false, "Vui lòng chọn bàn.");

            if (model.NgayGioDat < DateTime.Now.AddMinutes(30))
                return (false, "Vui lòng đặt bàn trước ít nhất 30 phút.");

            if (model.SoNguoi < 1)
                return (false, "Số người phải lớn hơn 0.");

            // Kiểm tra tài khoản đang đăng nhập.
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan);

            if (taiKhoan == null)
                return (false, "Không tìm thấy tài khoản đăng nhập.");

            var email = model.Email?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email))
                return (false, "Vui lòng nhập email.");

            // Email phải trùng với email tài khoản đăng nhập.
            if (!string.Equals(
                email,
                taiKhoan.Email?.Trim(),
                StringComparison.OrdinalIgnoreCase))
            {
                return (
                    false,
                    "Email đặt bàn không khớp với tài khoản đăng nhập.");
            }

            // Kiểm tra bàn.
            var ban = await _context.BanAns
                .FirstOrDefaultAsync(b => b.MaBan == model.MaBan.Value);

            if (ban == null || ban.TrangThai != "Sẵn sàng")
                return (
                    false,
                    "Bàn không tồn tại hoặc hiện không sẵn sàng.");

            if (model.SoNguoi > ban.SoChoNgoi)
                return (
                    false,
                    $"Bàn {ban.TenBan} chỉ có {ban.SoChoNgoi} chỗ ngồi.");

            // Kiểm tra trùng lịch đặt bàn.
            var batDauMoi = model.NgayGioDat;
            var ketThucMoi = batDauMoi.AddHours(ThoiLuongDatBanGio);

            var biTrungLich = await _context.PhieuDatBans
                .AnyAsync(p =>
                    p.MaBan == ban.MaBan &&
                    p.TrangThai != "Đã hủy" &&
                    p.NgayGioDat < ketThucMoi &&
                    p.NgayGioDat.AddHours(ThoiLuongDatBanGio)
                        > batDauMoi);

            if (biTrungLich)
            {
                return (
                    false,
                    "Bàn đã có người đặt trong khung giờ này. Vui lòng chọn bàn hoặc giờ khác.");
            }

            // Tìm khách hàng theo tài khoản, không tìm theo email đơn thuần.
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k =>
                    k.MaTaiKhoan == maTaiKhoan);

            if (khachHang == null)
            {
                return (
                    false,
                    "Tài khoản chưa có hồ sơ khách hàng. Vui lòng cập nhật hồ sơ trước khi đặt bàn.");
            }

            // Cập nhật thông tin liên hệ.
            khachHang.HoTen = model.HoTen.Trim();
            khachHang.SoDienThoai = model.SoDienThoai.Trim();
            khachHang.Email = email;
            khachHang.TrangThai = true;

            // Tạo phiếu gắn với khách hàng của tài khoản hiện tại.
            var phieu = new PhieuDatBan
            {
                MaKhachHang = khachHang.MaKhachHang,
                MaBan = ban.MaBan,
                NgayGioDat = batDauMoi,
                SoNguoi = model.SoNguoi,
                TrangThai = "Chờ xác nhận",
                GhiChu = string.IsNullOrWhiteSpace(model.GhiChu)
                    ? null
                    : model.GhiChu.Trim(),
                NgayTao = DateTime.Now
            };

            _context.PhieuDatBans.Add(phieu);

            await _context.SaveChangesAsync();

            return (
                true,
                "Đặt bàn thành công! Phiếu của bạn đang chờ xác nhận.");
        }

        // Chỉ lấy lịch sử đặt bàn của tài khoản hiện tại.
        public async Task<List<PhieuDatBan>> GetLichSuAsync(
            int maTaiKhoan)
        {
            return await _context.PhieuDatBans
                .AsNoTracking()
                .Include(p => p.BanAn)
                .Include(p => p.KhachHang)
                .Where(p =>
                    p.KhachHang != null &&
                    p.KhachHang.MaTaiKhoan == maTaiKhoan)
                .OrderByDescending(p => p.NgayTao)
                .ToListAsync();
        }
    }
}

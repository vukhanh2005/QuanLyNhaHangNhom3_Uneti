using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Service.Interface;
using QuanLyNhaHang.ViewModels.Client;

namespace QuanLyNhaHang.Service.Implement
{
    public class ClientDatBanService : IClientDatBanService
    {
        private readonly AppDbContext _context;

        // Giả định một lượt đặt bàn kéo dài 2 tiếng.
        private const int ThoiLuongDatBanGio = 2;

        public ClientDatBanService(AppDbContext context)
        {
            _context = context;
        }

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

        public async Task<(bool ThanhCong, string ThongBao)>
            TaoPhieuDatBanAsync(TaoPhieuDatBanViewModel model)
        {
            if (model.MaBan == null)
                return (false, "Vui lòng chọn bàn.");

            if (model.NgayGioDat < DateTime.Now.AddMinutes(30))
                return (false, "Vui lòng đặt bàn trước ít nhất 30 phút.");

            if (model.SoNguoi < 1)
                return (false, "Số người phải lớn hơn 0.");

            var email = model.Email.Trim().ToLower();

            var ban = await _context.BanAns
                .FirstOrDefaultAsync(b => b.MaBan == model.MaBan.Value);

            if (ban == null || ban.TrangThai != "Sẵn sàng")
                return (false, "Bàn không tồn tại hoặc hiện không sẵn sàng.");

            if (model.SoNguoi > ban.SoChoNgoi)
                return (
                    false,
                    $"Bàn {ban.TenBan} chỉ có {ban.SoChoNgoi} chỗ ngồi.");

            var batDauMoi = model.NgayGioDat;
            var ketThucMoi = batDauMoi.AddHours(ThoiLuongDatBanGio);

            var biTrungLich = await _context.PhieuDatBans
                .AnyAsync(p =>
                    p.MaBan == ban.MaBan &&
                    p.TrangThai != "Đã hủy" &&
                    p.NgayGioDat < ketThucMoi &&
                    p.NgayGioDat.AddHours(ThoiLuongDatBanGio) > batDauMoi);

            if (biTrungLich)
                return (
                    false,
                    "Bàn đã có người đặt trong khung giờ này. Vui lòng chọn bàn hoặc giờ khác.");

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k =>
                    k.Email != null &&
                    k.Email.ToLower() == email);

            if (khachHang == null)
            {
                khachHang = new KhachHang
                {
                    HoTen = model.HoTen.Trim(),
                    SoDienThoai = model.SoDienThoai.Trim(),
                    Email = email,
                    TrangThai = true
                };

                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();
            }
            else
            {
                khachHang.HoTen = model.HoTen.Trim();
                khachHang.SoDienThoai = model.SoDienThoai.Trim();
                khachHang.TrangThai = true;
            }

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

            return (true, "Đặt bàn thành công! Phiếu của bạn đang chờ xác nhận.");
        }

        public async Task<List<PhieuDatBan>> GetLichSuAsync(string email)
        {
            var emailChuanHoa = email.Trim().ToLower();

            return await _context.PhieuDatBans
                .AsNoTracking()
                .Include(p => p.BanAn)
                .Include(p => p.KhachHang)
                .Where(p =>
                    p.KhachHang != null &&
                    p.KhachHang.Email != null &&
                    p.KhachHang.Email.ToLower() == emailChuanHoa)
                .OrderByDescending(p => p.NgayTao)
                .ToListAsync();
        }
    }
}

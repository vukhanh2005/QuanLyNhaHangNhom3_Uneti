using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repository;

public class KhachHangRepository(AppDbContext context)
{
    public Task<List<KhachHang>> GetAll(string? tuKhoa = null, bool? trangThai = null)
    {
        var query = context.KhachHangs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(x => x.HoTen.Contains(keyword) || x.Email.Contains(keyword)
                || x.SoDienThoai.Contains(keyword));
        }
        if (trangThai.HasValue) query = query.Where(x => x.TrangThai == trangThai.Value);
        return query.OrderBy(x => x.HoTen).ThenBy(x => x.MaKhachHang).ToListAsync();
    }

    public Task<KhachHang?> GetById(int id) => context.KhachHangs.AsNoTracking()
        .FirstOrDefaultAsync(x => x.MaKhachHang == id);

    public Task<KhachHang?> GetByTaiKhoan(int maTaiKhoan) => context.KhachHangs.AsNoTracking()
        .FirstOrDefaultAsync(x => x.MaTaiKhoan == maTaiKhoan && x.TaiKhoan.VaiTro == VaiTro.User);

    public async Task<KhachHang?> CapNhatTrangThai(int id, bool trangThai)
    {
        var item = await context.KhachHangs.FirstOrDefaultAsync(x => x.MaKhachHang == id);
        if (item == null) return null;
        item.TrangThai = trangThai;
        await context.SaveChangesAsync();
        return item;
    }

    // Mã tài khoản do controller lấy từ session, không lấy mã khách hàng từ form.
    public async Task<KhachHang?> CapNhatCaNhan(int maTaiKhoan, KhachHangRequest request)
    {
        var item = await context.KhachHangs.Include(x => x.TaiKhoan)
            .FirstOrDefaultAsync(x => x.MaTaiKhoan == maTaiKhoan && x.TaiKhoan.VaiTro == VaiTro.User);
        if (item == null) return null;
        item.HoTen = request.HoTen.Trim();
        item.NgaySinh = request.NgaySinh!.Value.Date;
        item.GioiTinh = request.GioiTinh!.Value;
        item.SoDienThoai = request.SoDienThoai.Trim();
        item.Email = request.Email.Trim();
        item.DiaChi = request.DiaChi.Trim();
        // Hai model cùng lưu họ tên/email nên cập nhật trong cùng giao dịch.
        item.TaiKhoan.HoTen = item.HoTen;
        item.TaiKhoan.Email = item.Email;
        await context.SaveChangesAsync();
        return item;
    }
}

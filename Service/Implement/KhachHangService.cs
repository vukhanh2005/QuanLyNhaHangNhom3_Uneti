using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Service.Implement
{
    public class KhachHangService
    {
        // Request đã được kiểm tra trong luồng đăng ký; repository lưu cả hai entity.
        public KhachHang TaoKhachHang(TaiKhoanRequest request)
        {
            return new KhachHang
            {
                HoTen = request.hoTen.Trim(),
                Email = request.email.Trim(),
                NgaySinh = request.ngaySinh!.Value.Date,
                GioiTinh = request.gioiTinh!.Value,
                SoDienThoai = request.soDienThoai.Trim(),
                DiaChi = request.diaChi.Trim(),
                NgayDangKy = DateTime.Now,
                TrangThai = true
            };
        }
    }
}

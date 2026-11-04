using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repository;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaHang.Service.Implement
{
    public class KhachHangService(KhachHangRepository repository) : IKhachHangService
    {
        public async Task<List<KhachHangResponse>> GetAll(string? tuKhoa = null, bool? trangThai = null)
            => (await repository.GetAll(tuKhoa, trangThai)).Select(ToResponse).ToList();

        public async Task<KhachHangResult> GetById(int id) => Result(await repository.GetById(id));
        public async Task<KhachHangResult> GetCaNhan(int maTaiKhoan) => Result(await repository.GetByTaiKhoan(maTaiKhoan));

        public async Task<KhachHangResult> CapNhatTrangThai(int id, bool trangThai)
        {
            try { return Result(await repository.CapNhatTrangThai(id, trangThai), "Cập nhật trạng thái thành công."); }
            catch (DbUpdateConcurrencyException) { return new() { Message = "Hồ sơ đã thay đổi hoặc bị xóa. Vui lòng tải lại." }; }
        }

        public async Task<KhachHangResult> CapNhatCaNhan(int maTaiKhoan, KhachHangRequest request)
        {
            var errors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, new ValidationContext(request), errors, true))
                return new() { Message = string.Join(" ", errors.Select(x => x.ErrorMessage)) };
            try { return Result(await repository.CapNhatCaNhan(maTaiKhoan, request), "Cập nhật thông tin cá nhân thành công."); }
            catch (DbUpdateConcurrencyException) { return new() { Message = "Hồ sơ đã thay đổi hoặc bị xóa. Vui lòng tải lại." }; }
        }

        private static KhachHangResponse ToResponse(KhachHang item) => new()
        {
            MaKhachHang = item.MaKhachHang, MaTaiKhoan = item.MaTaiKhoan,
            HoTen = item.HoTen, NgaySinh = item.NgaySinh, GioiTinh = item.GioiTinh,
            SoDienThoai = item.SoDienThoai, Email = item.Email, DiaChi = item.DiaChi,
            NgayDangKy = item.NgayDangKy, TrangThai = item.TrangThai
        };

        private static KhachHangResult Result(KhachHang? item, string? message = null) => new()
        {
            IsSuccess = item != null,
            Message = item == null ? "Không tìm thấy khách hàng." : message,
            Data = item == null ? null : ToResponse(item)
        };

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

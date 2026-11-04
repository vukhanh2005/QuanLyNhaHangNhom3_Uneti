using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;

public interface IKhachHangService
{
    KhachHang TaoKhachHang(TaiKhoanRequest request);
    Task<List<KhachHangResponse>> GetAll(string? tuKhoa = null, bool? trangThai = null);
    Task<KhachHangResult> GetById(int id);
    Task<KhachHangResult> GetCaNhan(int maTaiKhoan);
    Task<KhachHangResult> CapNhatTrangThai(int id, bool trangThai);
    Task<KhachHangResult> CapNhatCaNhan(int maTaiKhoan, KhachHangRequest request);
}

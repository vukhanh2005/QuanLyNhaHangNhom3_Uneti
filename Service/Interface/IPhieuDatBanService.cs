
using System;
using System.Threading.Tasks;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.ViewModels.Module04;

namespace QuanLyNhaHang.Service.Interface
{
    public interface IPhieuDatBanService
    {
        Task<PhieuDatBanViewModel> GetDanhSachAsync(
            string? tenKhachHang,
            string? tenBan,
            string? trangThai,
            DateTime? ngaySuDung);

        Task<PhieuDatBan?> GetChiTietAsync(int id);

        Task<(bool ThanhCong, string ThongBao)> CapNhatTrangThaiAsync(
            int id,
            string trangThaiMoi);
    }
}

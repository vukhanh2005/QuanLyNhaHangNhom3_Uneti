using QuanLyNhaHang.Models;
using QuanLyNhaHang.ViewModels.Client;

namespace QuanLyNhaHang.Service.Interface
{
    public interface IClientDatBanService
    {
        Task<List<BanAn>> GetDanhSachBanAsync();

        Task<(bool ThanhCong, string ThongBao)> TaoPhieuDatBanAsync(
            TaoPhieuDatBanViewModel model);

        Task<List<PhieuDatBan>> GetLichSuAsync(string email);
    }
}

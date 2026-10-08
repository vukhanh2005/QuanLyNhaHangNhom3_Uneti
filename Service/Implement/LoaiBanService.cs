using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Repository;

namespace QuanLyNhaHang.Service.Implement
{
    public class LoaiBanService : ILoaiBanService
    {
        LoaiBanRepository repository;
        public LoaiBanService(LoaiBanRepository repository)
        {
            this.repository = repository;
        }
        Task<TaiKhoanResponse> ILoaiBanService.Them(LoaiBanRequest request)
        {
            return null;
        }
        Task<TaiKhoanResponse> ILoaiBanService.Sua(int MaLoaiBan, LoaiBanRequest data)
        {
            return null;   
        }

        Task<TaiKhoanResponse> ILoaiBanService.Xoa(int MaLoaiBan)
        {
            return null;   
        }

        Task<List<TaiKhoanResponse>> ILoaiBanService.TimKiem(int MaLoaiBan)
        {
            return null;   
        }

    }
}
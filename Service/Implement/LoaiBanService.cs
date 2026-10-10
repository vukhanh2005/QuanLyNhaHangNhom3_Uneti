using Microsoft.IdentityModel.Tokens;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;
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
        async Task<LoaiBanResponse> ILoaiBanService.Them(LoaiBanRequest request)
        {
            //Kiem tra du lieu
            if (request.TenLoaiBan.IsNullOrEmpty() || request.MoTa.IsNullOrEmpty())
            {
                return new LoaiBanResponse("Dữ liệu không được để trống", IsSuccess: false);
            }

            LoaiBan response = await repository.Them(request.ConvertToLoaiBan());
            
            if(response == null)
            {
                return new LoaiBanResponse("Có lỗi xảy ra!!", IsSuccess: false);
            }
            else
            {
                return new LoaiBanResponse(
                    response.MaLoaiBan,
                    response.TenLoaiBan,
                    response.MoTa == null ? "" : response.MoTa,
                    response.TrangThai,
                    isSuccess: true,
                    "Thêm thành công!!!"
                );
            }
        }
        async Task<LoaiBanResponse> ILoaiBanService.Sua(int MaLoaiBan, LoaiBanRequest request)
        {
            //Kiem tra du lieu
            if (request.TenLoaiBan.IsNullOrEmpty() || request.MoTa.IsNullOrEmpty())
            {
                return new LoaiBanResponse("Dữ liệu không được để trống", IsSuccess: false);
            }

            LoaiBan? response = await repository.Sua(MaLoaiBan, request.ConvertToLoaiBan());
            
            if(response == null)
            {
                return new LoaiBanResponse("Có lỗi xảy ra!!", IsSuccess: false);
            }
            else
            {
                return new LoaiBanResponse(
                    response.MaLoaiBan,
                    response.TenLoaiBan,
                    response.MoTa == null ? "" : response.MoTa,
                    response.TrangThai,
                    isSuccess: true,
                    "Sửa thành công!!!"
                );
            }
        }

        async Task<LoaiBanResponse> ILoaiBanService.Xoa(int MaLoaiBan)
        {
            LoaiBan? response = await repository.Xoa(MaLoaiBan);
            
            if(response == null)
            {
                return new LoaiBanResponse("Có lỗi xảy ra!!", IsSuccess: false);
            }
            else
            {
                return new LoaiBanResponse(
                    response.MaLoaiBan,
                    response.TenLoaiBan,
                    response.MoTa == null ? "" : response.MoTa,
                    response.TrangThai,
                    isSuccess: true,
                    "Xóa thành công!!!"
                );
            }
        }

        async Task<LoaiBanResponse> ILoaiBanService.TimKiem(int MaLoaiBan)
        {
            LoaiBan? response = await repository.TimKiem(MaLoaiBan);
            
            if(response == null)
            {
                return new LoaiBanResponse("Có lỗi xảy ra!!", IsSuccess: false);
            }
            else
            {
                return new LoaiBanResponse(
                    response.MaLoaiBan,
                    response.TenLoaiBan,
                    response.MoTa == null ? "" : response.MoTa,
                    response.TrangThai,
                    isSuccess: true,
                    "Tìm kiếm thành công!!!"
                );
            }
        }

    }
}
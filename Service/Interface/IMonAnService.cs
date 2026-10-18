using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;

public interface IMonAnService
{
    Task<List<MonAn>> GetAll(bool chiDangPhucVu = false);
    Task<MonAnResponse> GetById(int id);
    Task<MonAnResponse> Them(MonAnRequest request);
    Task<MonAnResponse> Sua(int id, MonAnRequest request);
    Task<MonAnResponse> Xoa(int id);
    Task<MonAnResponse> CapNhatTrangThai(int id, TrangThaiMonAn trangThai);
}

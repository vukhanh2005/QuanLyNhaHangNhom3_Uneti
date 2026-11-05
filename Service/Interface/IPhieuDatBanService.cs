using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;
using QuanLyNhaHangNhom3_Uneti.Models;

public interface IPhieuDatBanService
{
    Task<PhieuDatBanResult> TaoPhieu(int maTaiKhoan, PhieuDatBanRequest request,
        CancellationToken cancellationToken = default);
    Task<PhieuDatBanResult> Huy(int maPhieu, int? maTaiKhoan = null,
        CancellationToken cancellationToken = default);
    Task<List<PhieuDatBan>> GetAll(int? maTaiKhoan = null,
        CancellationToken cancellationToken = default);
    Task<List<PhieuDatBan>> GetLichSu(int maTaiKhoan,
        CancellationToken cancellationToken = default);
    Task<PhieuDatBan?> GetChiTietCaNhan(int maPhieu, int maTaiKhoan,
        CancellationToken cancellationToken = default);
    Task<List<BanAn>> GetBanDangPhucVu(CancellationToken cancellationToken = default);
}

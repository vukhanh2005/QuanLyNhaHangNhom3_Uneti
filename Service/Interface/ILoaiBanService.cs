using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;

public interface ILoaiBanService
{
    public Task<List<LoaiBan>> GetAllLoaiBan();
    public Task<LoaiBanResponse> CapNhatTrangThai(int MaLoaiBan, bool trangThai);
    public Task<LoaiBanResponse> Them(LoaiBanRequest request);
    public Task<LoaiBanResponse> Sua(int MaLoaiBan, LoaiBanRequest data);
    public Task<LoaiBanResponse> Xoa(int MaLoaiBan);
    public Task<LoaiBanResponse> TimKiem(int MaLoaiBan);
}

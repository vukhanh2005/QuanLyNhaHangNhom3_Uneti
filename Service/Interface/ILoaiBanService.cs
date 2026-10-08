using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.DTO;

public interface ILoaiBanService
{
    public Task<TaiKhoanResponse> Them(LoaiBanRequest request);
    public Task<TaiKhoanResponse> Sua(int MaLoaiBan, LoaiBanRequest data);
    public Task<TaiKhoanResponse> Xoa(int MaLoaiBan);
    public Task<List<TaiKhoanResponse>> TimKiem(int MaLoaiBan);
}
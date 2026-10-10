using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.DTO;

public interface ILoaiBanService
{
    public Task<LoaiBanResponse> Them(LoaiBanRequest request);
    public Task<LoaiBanResponse> Sua(int MaLoaiBan, LoaiBanRequest data);
    public Task<LoaiBanResponse> Xoa(int MaLoaiBan);
    public Task<LoaiBanResponse> TimKiem(int MaLoaiBan);
}
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.DTO;

public interface ILoginService
{
    public Task<TaiKhoanResponse> Login(TaiKhoanRequest request);
    public Task<TaiKhoanResponse> Register(TaiKhoanRequest request);

}
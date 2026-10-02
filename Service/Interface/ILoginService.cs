using QuanLyNhaHang.DTO;

public interface ILoginService
{
    public Task<TaiKhoanResponse> Login(string username, string password);

}
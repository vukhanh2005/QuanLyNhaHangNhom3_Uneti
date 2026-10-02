using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repository;

namespace QuanLyNhaHang.Service
{
    public class LoginService : ILoginService
    {
        AccountRepository repository = null;
        public LoginService(AccountRepository repository)
        {
            this.repository = repository;
        }
    
        public async Task<TaiKhoanResponse> Login(string username, string password)
        {
            TaiKhoan account = await repository.getAccountByUsername(username);

            //Tim thay tai khoan, kiem tra mat khau
            if(account != null)
            {
                if (account.MatKhau.Equals(password))
                {
                    string message = "[SUCCESS] Đăng nhập thành công";
                    //Acc dung
                    return new TaiKhoanResponse(
                        account.MaTaiKhoan,
                        account.TenDangNhap,
                        account.MatKhau,
                        account.HoTen,
                        account.Email,
                        account.VaiTro.ToString(),
                        account.TrangThai,
                        message
                    );
                }
                else
                {
                    string message = "[FAIL] Mật khẩu không chính xác";
                    TaiKhoanResponse response = new TaiKhoanResponse();
                    //Acc sai mat khau
                    response.message = message;
                    return response;
                }
            }
            else
            {
                //Acc khong ton tai
                string message = "[FAIL] Tài khoản không có trong hệ thống";
                TaiKhoanResponse response = new TaiKhoanResponse();
                response.message = message;
                return response;
            }
        }
    }
}
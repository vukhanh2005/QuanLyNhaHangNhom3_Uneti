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
        public async Task<TaiKhoanResponse> Register(TaiKhoanRequest request)
        {
            //Kiem tra du lieu de trong
            if(request.hoTen.Trim().Equals("") || request.username.Trim().Equals("") || request.password.Trim().Equals("")
            || request.confirmPassword.Trim().Equals("") || request.email.Trim().Equals("")
            )
            {
                return new TaiKhoanResponse("Dữ liệu không được để trống");
            }
            //Kiem tra nhap lai mat khau co trung khong
            if (!request.confirmPassword.Equals(request.password))
            {
                return new TaiKhoanResponse("Mật khẩu nhập lại không trùng");
            }
            //Kiem tra username co san trong csdl chua
            TaiKhoan account = await repository.getAccountByUsername(request.username);

            if(account == null) //khong co tai khoan => tien hanh dang ki
            {
                await repository.themTaiKhoan(request.ConvertToTaiKhoan());
            }
            else // da co tai khoan => khong dang ki duoc
            {
                return new TaiKhoanResponse($"Đã có tài khoản {request.username} trong hệ thống!!");
            }
            return null;
        }
        public async Task<TaiKhoanResponse> Login(TaiKhoanRequest request)
        {
            string username = request.username;
            string password = request.password;
            //Kiem tra username, password rong
            if(username.Trim().Equals(String.Empty) || password.Trim().Equals(String.Empty))
            {
                string message = "Tài khoản hoặc mật khẩu không chính xác";
                return new TaiKhoanResponse(
                    message
                );
            }
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
                    TaiKhoanResponse response = new TaiKhoanResponse(message);
                    return response;
                }
            }
            else
            {
                //Acc khong ton tai
                string message = "[FAIL] Tài khoản không có trong hệ thống";
                TaiKhoanResponse response = new TaiKhoanResponse(message);
                return response;
            }
        }
    }
}
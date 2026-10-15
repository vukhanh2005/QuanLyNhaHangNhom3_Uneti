using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repository;
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang.Service.Implement
{
    public class LoginService : ILoginService
    {
        AccountRepository repository;
        private readonly KhachHangService khachHangService = new KhachHangService();
        public LoginService(AccountRepository repository)
        {
            this.repository = repository;
        }

        public async Task<TaiKhoanResponse> Register(TaiKhoanRequest request)
        {
            // Kiểm tra dữ liệu để trống
            if (string.IsNullOrWhiteSpace(request.hoTen) ||
                string.IsNullOrWhiteSpace(request.username) ||
                string.IsNullOrWhiteSpace(request.password) ||
                string.IsNullOrWhiteSpace(request.confirmPassword) ||
                string.IsNullOrWhiteSpace(request.email) ||
                string.IsNullOrWhiteSpace(request.soDienThoai) ||
                string.IsNullOrWhiteSpace(request.diaChi) ||
                !request.ngaySinh.HasValue || !request.gioiTinh.HasValue)
            {
                return new TaiKhoanResponse("Dữ liệu không được để trống");
            }

            request.username = request.username.Trim();
            request.hoTen = request.hoTen.Trim();
            request.email = request.email.Trim();

            if (request.username.Length > 50 || request.hoTen.Length > 250 || request.email.Length > 50)
                return new TaiKhoanResponse("Tên đăng nhập/email tối đa 50 ký tự, họ tên tối đa 250 ký tự.");

            if (!new EmailAddressAttribute().IsValid(request.email))
                return new TaiKhoanResponse("Email không hợp lệ.");

            if (!new PhoneAttribute().IsValid(request.soDienThoai.Trim()))
                return new TaiKhoanResponse("Số điện thoại không hợp lệ.");

            if (request.ngaySinh.Value.Date > DateTime.Today || request.ngaySinh.Value == DateTime.MinValue)
                return new TaiKhoanResponse("Ngày sinh không hợp lệ.");

            // Kiểm tra nhập lại mật khẩu có trùng khớp không
            if (!request.confirmPassword.Equals(request.password))
            {
                return new TaiKhoanResponse("Mật khẩu nhập lại không trùng");
            }

            // Kiểm tra username đã tồn tại trong CSDL chưa
            TaiKhoan account = await repository.getAccountByUsername(request.username);

            if (account == null) // Chưa có tài khoản => tiến hành đăng ký
            {
                // Chuyển đổi DTO sang Entity
                TaiKhoan newAccount = request.ConvertToTaiKhoan();
                newAccount.KhachHang = khachHangService.TaoKhachHang(request);

                // --- MÃ HÓA MẬT KHẨU TRƯỚC KHI LƯU VÀO DATABASE ---
                newAccount.MatKhau = PasswordHelper.HashPassword(newAccount, request.password);

                // EF lưu tài khoản và khách hàng trong cùng một SaveChanges/transaction,
                // tự gán MaTaiKhoan cho khách hàng sau khi tạo tài khoản.
                TaiKhoan responseAccount = await repository.themTaiKhoan(newAccount);

                return new TaiKhoanResponse(
                    responseAccount.MaTaiKhoan,
                    responseAccount.TenDangNhap,
                    responseAccount.MatKhau,
                    responseAccount.HoTen,
                    responseAccount.Email,
                    responseAccount.VaiTro.ToString(),
                    responseAccount.TrangThai,
                    "[SUCCESS] Đăng kí thành công"
                );
            }
            else // Đã có tài khoản
            {
                return new TaiKhoanResponse($"Đã có tài khoản {request.username} trong hệ thống!!");
            }
        }

        public async Task<TaiKhoanResponse> Login(TaiKhoanRequest request)
        {
            string username = request.username ?? string.Empty;
            string password = request.password ?? string.Empty;

            // Kiểm tra username, password rỗng
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new TaiKhoanResponse("Tài khoản hoặc mật khẩu không chính xác");
            }

            TaiKhoan account = await repository.getAccountByUsername(username);

            // Tìm thấy tài khoản trong CSDL
            if (account != null)
            {
                // Kiểm tra tài khoản có bị khóa không (Quy định bắt buộc của đề tài)
                if (!account.TrangThai)
                {
                    return new TaiKhoanResponse("Tài khoản của bạn đã bị khóa");
                }

                // --- KIỂM TRA MẬT KHẨU BẰNG HÀM VERIFY CỦA PASSWORDHELPER ---
                bool isPasswordValid = PasswordHelper.VerifyPassword(account, account.MatKhau, password);

                if (isPasswordValid)
                {
                    string message = "[SUCCESS] Đăng nhập thành công";
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
                    return new TaiKhoanResponse("Mật khẩu không chính xác");
                }
            }
            else
            {
                // Tài khoản không tồn tại
                return new TaiKhoanResponse("Tài khoản không có trong hệ thống");
            }
        }
    }
}

using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.DTO
{
    public class TaiKhoanRequest
    {
        public string username {get; set;}
        public string password {get; set;}
        public string confirmPassword {get; set;}
        public string email { get; set; }
        public string hoTen { get; set; }

        public TaiKhoan ConvertToTaiKhoan()
        {
            TaiKhoan rs = new TaiKhoan();
            rs.TenDangNhap = username;
            rs.MatKhau = password;
            rs.HoTen = hoTen;
            rs.TrangThai = true;
            rs.VaiTro = VaiTro.User;
            rs.Email = email;
            return rs;
        }
    }
}
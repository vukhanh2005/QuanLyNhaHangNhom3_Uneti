using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.DTO
{
    public class TaiKhoanResponse
    {
        public TaiKhoanResponse(int maTaiKhoan, string username, string password, string hoTen, 
        string email, string vaiTro, bool trangThai, string message)
        {
            this.maTaiKhoan = maTaiKhoan;
            this.username = username;
            this.password = password;
            this.hoTen = hoTen;
            this.email = email;
            this.vaiTro = vaiTro;
            this.trangThai = trangThai;
            this.message = message;
        }       
        public TaiKhoanResponse()
        {
            
        }
        public int maTaiKhoan { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string hoTen { get; set; }
        public string email { get; set; }
        public string vaiTro { get; set; }
        public bool trangThai { get; set; }
        public string message { get; set; }
    }
}
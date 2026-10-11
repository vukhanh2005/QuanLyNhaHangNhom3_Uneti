// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Khai báo các trạng thái bàn ăn dùng chung cho Module 2, 3, 4.
namespace QuanLyNhaHangNhom3_Uneti.Models;

// Dùng chung các giá trị này khi tích hợp Module 3/4.
public static class BanAnTrangThai
{
    public const string SanSang = "Sẵn sàng";
    public const string DangSuDung = "Đang sử dụng";
    public const string TamNgung = "Tạm ngừng phục vụ";
    public static readonly string[] TatCa = [SanSang, DangSuDung, TamNgung];
    public const string Pattern = "^(Sẵn sàng|Đang sử dụng|Tạm ngừng phục vụ)$";
}


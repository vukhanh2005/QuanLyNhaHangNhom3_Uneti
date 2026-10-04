// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Quản lý bàn ăn, tìm kiếm, lọc, sắp xếp và phân trang.
namespace QuanLyNhaHangNhom3_Uneti.Models;

// Dùng chung các giá trị này khi tích hợp Module 3/4.
public static class BanAnTrangThai
{
    public const string SanSang = "Sẵn sàng phục vụ";
    public const string DangSuDung = "Đang sử dụng";
    public const string TamNgung = "Tạm ngừng phục vụ";
    public static readonly string[] TatCa = [SanSang, DangSuDung, TamNgung];
    public const string Pattern = "^(Sẵn sàng phục vụ|Đang sử dụng|Tạm ngừng phục vụ)$";
}


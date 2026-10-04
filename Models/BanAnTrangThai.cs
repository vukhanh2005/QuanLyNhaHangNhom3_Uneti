// Họ và tên: Vi Thái Học
// Mã sinh viên: [ĐIỀN MÃ SINH VIÊN]
// Nội dung thực hiện: Quản lý bàn ăn, tìm kiếm, lọc, sắp xếp và phân trang.
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


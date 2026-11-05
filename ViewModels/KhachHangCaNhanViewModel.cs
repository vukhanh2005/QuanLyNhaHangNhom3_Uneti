using QuanLyNhaHang.DTO;

namespace QuanLyNhaHang.ViewModels;

// Thông tin chỉ đọc được lấy lại từ máy chủ, tách khỏi các trường khách hàng được sửa.
public class KhachHangCaNhanViewModel
{
    public KhachHangRequest Request { get; set; } = new();
    public KhachHangResponse? ThongTin { get; set; }
    public string? ThongBao { get; set; }
    public string NgaySinhToiDa => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
        DateTimeOffset.UtcNow, "SE Asia Standard Time").ToString("yyyy-MM-dd");
}

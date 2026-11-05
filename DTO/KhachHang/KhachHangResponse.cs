namespace QuanLyNhaHang.DTO;

public class KhachHangResponse
{
    public int MaKhachHang { get; set; }
    public int MaTaiKhoan { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public DateTime NgaySinh { get; set; }
    public bool GioiTinh { get; set; }
    public string SoDienThoai { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public DateTime NgayDangKy { get; set; }
    public bool TrangThai { get; set; }
}

public class KhachHangResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public KhachHangResponse? Data { get; set; }
}

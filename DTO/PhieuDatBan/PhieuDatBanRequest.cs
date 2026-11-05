using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang.DTO;

// Chỉ nhận dữ liệu khách hàng được phép chọn; tài khoản, ngày đặt và trạng thái do hệ thống xác định.
public class PhieuDatBanRequest
{
    [Display(Name = "Bàn")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn bàn.")]
    public int MaBan { get; set; }

    [Display(Name = "Ngày sử dụng")]
    [Required(ErrorMessage = "Vui lòng chọn ngày sử dụng.")]
    [DataType(DataType.Date)]
    public DateTime? NgaySuDung { get; set; }

    [Display(Name = "Giờ bắt đầu")]
    [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu.")]
    [DataType(DataType.Time)]
    public TimeSpan? GioBatDau { get; set; }

    [Display(Name = "Giờ kết thúc")]
    [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc.")]
    [DataType(DataType.Time)]
    public TimeSpan? GioKetThuc { get; set; }

    [Display(Name = "Số người")]
    [Range(1, int.MaxValue, ErrorMessage = "Số người phải lớn hơn 0.")]
    public int SoNguoi { get; set; }

    [Display(Name = "Ghi chú")]
    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; set; }
}

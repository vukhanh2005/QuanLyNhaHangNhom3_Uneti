using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang.DTO;

// Chỉ chứa các trường khách hàng được phép tự sửa.
public class KhachHangRequest : IValidatableObject
{
    [Display(Name = "Họ tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(250, ErrorMessage = "Họ tên không được vượt quá 250 ký tự.")]
    public string HoTen { get; set; } = string.Empty;
    [Display(Name = "Ngày sinh")]
    [Required(ErrorMessage = "Vui lòng chọn ngày sinh.")]
    [DataType(DataType.Date)]
    public DateTime? NgaySinh { get; set; }
    [Display(Name = "Giới tính")]
    [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
    public bool? GioiTinh { get; set; }
    [Display(Name = "Số điện thoại")]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string SoDienThoai { get; set; } = string.Empty;
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự.")]
    public string Email { get; set; } = string.Empty;
    [Display(Name = "Địa chỉ")]
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    public string DiaChi { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var homNay = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, "SE Asia Standard Time").Date;
        if (NgaySinh.HasValue && (NgaySinh.Value.Date > homNay || NgaySinh.Value.Date == DateTime.MinValue.Date))
            yield return new ValidationResult("Ngày sinh không hợp lệ.", new[] { nameof(NgaySinh) });
    }
}

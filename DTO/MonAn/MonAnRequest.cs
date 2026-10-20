using System.ComponentModel.DataAnnotations;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.DTO;

public class MonAnRequest : IValidatableObject
{
    [Required(ErrorMessage = "Vui lòng nhập tên món.")]
    [StringLength(50)]
    [Display(Name = "Tên món")]
    public string TenMon { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập loại món.")]
    [StringLength(50)]
    [Display(Name = "Loại món")]
    public string LoaiMon { get; set; } = string.Empty;
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }
    [EnumDataType(typeof(TrangThaiMonAn), ErrorMessage = "Trạng thái không hợp lệ.")]
    [Display(Name = "Trạng thái")]
    public TrangThaiMonAn TrangThai { get; set; } = TrangThaiMonAn.DangPhucVu;

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (DonGia <= 0 || DonGia > 9999999999999999.99m || decimal.Round(DonGia, 2) != DonGia)
            yield return new ValidationResult("Đơn giá phải lớn hơn 0, tối đa 16 chữ số phần nguyên và 2 chữ số thập phân.", new[] { nameof(DonGia) });
    }

    public MonAn ConvertToMonAn() => new()
    {
        TenMon = TenMon.Trim(), LoaiMon = LoaiMon.Trim(), DonGia = DonGia,
        MoTa = MoTa?.Trim(), TrangThai = TrangThai
    };
}

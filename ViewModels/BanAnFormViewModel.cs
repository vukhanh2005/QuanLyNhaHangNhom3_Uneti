// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Quản lý bàn ăn, tìm kiếm, lọc, sắp xếp và phân trang.
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.ViewModels;

public class BanAnFormViewModel
{
    [Required(ErrorMessage = "Tên bàn không được để trống.")]
    [StringLength(100, ErrorMessage = "Tên bàn tối đa 100 ký tự.")]
    [Display(Name = "Tên bàn")]
    public string TenBan { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn loại bàn.")]
    [Display(Name = "Loại bàn")]
    public int MaLoaiBan { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số chỗ ngồi phải lớn hơn 0.")]
    [Display(Name = "Số chỗ ngồi")]
    public int SoChoNgoi { get; set; } = 4;

    [Required(ErrorMessage = "Vị trí không được để trống.")]
    [StringLength(100, ErrorMessage = "Vị trí tối đa 100 ký tự.")]
    [Display(Name = "Vị trí")]
    public string ViTri { get; set; } = "";

    [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự.")]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái.")]
    [RegularExpression(BanAnTrangThai.Pattern, ErrorMessage = "Trạng thái không hợp lệ.")]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = BanAnTrangThai.SanSang;

    [ValidateNever]
    public IEnumerable<SelectListItem> LoaiBanOptions { get; set; } = [];
    public IEnumerable<SelectListItem> TrangThaiOptions =>
        BanAnTrangThai.TatCa.Select(x => new SelectListItem(x, x));
}


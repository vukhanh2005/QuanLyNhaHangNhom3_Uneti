using System.ComponentModel.DataAnnotations;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels.Client
{
    public class TaoPhieuDatBanViewModel
    {
        public List<BanAn> DanhSachBan { get; set; } = new();

        [Required(ErrorMessage = "Vui lòng chọn bàn.")]
        public int? MaBan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày giờ.")]
        public DateTime NgayGioDat { get; set; } = DateTime.Now.AddDays(1);

        [Range(1, 100, ErrorMessage = "Số người phải từ 1 đến 100.")]
        public int SoNguoi { get; set; } = 2;

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [StringLength(20)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [StringLength(500)]
        public string? GhiChu { get; set; }
    }
}

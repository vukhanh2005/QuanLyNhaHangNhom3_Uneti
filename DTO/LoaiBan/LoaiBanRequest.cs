using QuanLyNhaHang.Models;
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang.DTO
{
    public class LoaiBanRequest
    {
        [Required(ErrorMessage = "Tên loại bàn không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên loại bàn tối đa 100 ký tự.")]
        [Display(Name = "Tên loại bàn")]
        public string TenLoaiBan { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự.")]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        public LoaiBan ConvertToLoaiBan()
        {
            LoaiBan rs = new LoaiBan();
            rs.TenLoaiBan = TenLoaiBan.Trim();
            rs.MoTa = MoTa?.Trim();
            rs.TrangThai = TrangThai;
            return rs;
        }
    }
}

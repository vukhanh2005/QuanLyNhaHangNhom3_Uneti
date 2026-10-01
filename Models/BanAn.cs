using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangNhom3_Uneti.Models
{
    public class BanAn
    {
        [Key]
        public int MaBan { get; set; }

        [Required(ErrorMessage = "Tên bàn không được để trống")]
        [StringLength(100)]
        public string TenBan { get; set; } = string.Empty;

        [Required]
        public int MaLoaiBan { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Số chỗ ngồi phải lớn hơn 0")]
        public int SoChoNgoi { get; set; }

        [Required(ErrorMessage = "Vị trí không được để trống")]
        [StringLength(100)]
        public string ViTri { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        [Required]
        public string TrangThai { get; set; } = "Sẵn sàng";
    }
}

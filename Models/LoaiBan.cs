using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang.Models
{
    public class LoaiBan
    {
        [Key]
        public int MaLoaiBan { get; set; }

        [Required]
        [StringLength(100)]
        public string TenLoaiBan { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        [Required]
        public bool TrangThai { get; set; } = true;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models
{
    public enum TrangThaiMonAn
    {
        DangPhucVu = 1,
        NgungPhucVu = 0
    }

    public class MonAn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaMon { get; set; }

        [Required]
        [StringLength(50)]
        public string TenMon { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LoaiMon { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal DonGia { get; set; }

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        [Required]
        public TrangThaiMonAn TrangThai { get; set; }
    }
}

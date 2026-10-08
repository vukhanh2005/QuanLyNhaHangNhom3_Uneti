using Microsoft.EntityFrameworkCore;
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
        public string TenMon { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiMon { get; set; }
        [Precision(18, 2)]
        [Range(0.01, double.MaxValue, ErrorMessage = "DonGia must be greater than 0.")]
        public decimal DonGia { get; set; }

        public string? MoTa { get; set; }

        [Required]
        public TrangThaiMonAn TrangThai { get; set; }
    }
}
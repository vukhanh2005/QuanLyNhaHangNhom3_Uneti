using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models
{
    public class PhieuDatBan
    {
        [Key]
        public int MaPhieuDatBan { get; set; }

        
        [Required]
        public int MaKhachHang { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang? KhachHang { get; set; }

        
        [Required]
        public int MaBan { get; set; }

        [ForeignKey(nameof(MaBan))]
        public BanAn? BanAn { get; set; }

       
        [Required]
        public DateTime NgayGioDat { get; set; }

        
        [Required]
        [Range(1, 100)]
        public int SoNguoi { get; set; }

        
        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ xác nhận";

        [StringLength(500)]
        public string? GhiChu { get; set; }

        
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
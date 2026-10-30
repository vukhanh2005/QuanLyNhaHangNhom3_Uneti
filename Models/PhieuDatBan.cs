using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.Models
{
    public enum TrangThaiPhieuDatBan
    {
        ChoXacNhan,
        DaXacNhan,
        DaSuDung,
        DaHuy
    }
    public class PhieuDatBan
    {
        [Key]
        public int MaPhieu { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        [Required]
        public int MaBan { get; set; }

        [Required]
        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Required]
        public DateTime NgaySuDung { get; set; }

        [Required]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        public TimeSpan GioKetThuc { get; set; }

        [Required]
        public int SoNguoi { get; set; }

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Required]
        public TrangThaiPhieuDatBan TrangThai { get; set; } = TrangThaiPhieuDatBan.ChoXacNhan;

        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang KhachHang { get; set; } = null!;

        [ForeignKey(nameof(MaBan))]
        public BanAn BanAn { get; set; } = null!;
    }
}

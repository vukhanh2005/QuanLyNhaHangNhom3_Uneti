using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models
{

    public class KhachHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaKhachHang { get; set; }
        [Required]
        [StringLength(250)]
        public string HoTen { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }
        [Required]
        public bool GioiTinh { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string SoDienThoai { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string DiaChi { get; set; }

        [Required]
        public bool TrangThai { get; set; }
        [Required]
        public int MaTaiKhoan { get; set; }
        public DateTime NgayDangKy { get; set; }

        [ForeignKey(nameof(MaTaiKhoan))]
        public TaiKhoan TaiKhoan { get; set; } = null!;
       // public ICollection<PhieuDatBan> PhieuDatBans { get; set; } = new List<PhieuDatBan>();

    }
}
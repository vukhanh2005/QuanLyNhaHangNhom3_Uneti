using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models
{
    public class KhachHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaKhachHang { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(250)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required]
        public bool GioiTinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [DataType(DataType.EmailAddress)]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string DiaChi { get; set; } = string.Empty;

        [Required]
        public bool TrangThai { get; set; } = true;

        [Required]
        public int MaTaiKhoan { get; set; }

        public DateTime NgayDangKy { get; set; }

        [ForeignKey(nameof(MaTaiKhoan))]
        public TaiKhoan TaiKhoan { get; set; } = null!;

        public ICollection<PhieuDatBan> PhieuDatBans { get; set; }
            = new List<PhieuDatBan>();
    }
}

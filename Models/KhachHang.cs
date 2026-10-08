using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(20)]
        public string SoDienThoai { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? DiaChi { get; set; }

        public bool TrangThai { get; set; } = true;

        
        public ICollection<PhieuDatBan> PhieuDatBans { get; set; }
            = new List<PhieuDatBan>();
    }
}
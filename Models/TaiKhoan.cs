using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models
{
    public enum VaiTro
    {
        [Description("Admin")]
        Admin,
        [Description("User")]
        User
    }
    public class TaiKhoan
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTaiKhoan { get; set; }
        [Required]
        [StringLength(50)]
        public string? TenDangNhap { get; set; }
        [Required]
        [StringLength(50)]
        public string? MatKhau { get; set; }
        [Required]
        [StringLength(50)]
        public string? HoTen { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string? Email { get; set; }
        [StringLength(50)]
        [Required]
        public VaiTro VaiTro { get; set; }
        [Required]
        public bool TrangThai { get; set; }
    }
}

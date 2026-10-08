using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang.Models
{
    public class LoaiBan
    {
        [Key]
        public int MaLoaiBan { get; set; }

        [Required(ErrorMessage = "Tên loại bàn không được để trống")]
        [StringLength(100)]
        public string TenLoaiBan { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        
        public ICollection<BanAn> BanAns { get; set; }
            = new List<BanAn>();
    }
}
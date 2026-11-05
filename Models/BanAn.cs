// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Xây dựng entity bàn ăn, validation và liên kết với loại bàn.
using System.ComponentModel.DataAnnotations;
using QuanLyNhaHang.Models;


namespace QuanLyNhaHangNhom3_Uneti.Models
{
    public class BanAn
    {
        [Key]
        public int MaBan { get; set; }

        // SQL Server tự đổi phiên bản sau mỗi lần cập nhật; EF dùng để phát hiện sửa đồng thời.
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        [Required(ErrorMessage = "Tên bàn không được để trống")]
        [StringLength(100)]
        public string TenBan { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn loại bàn.")]
        public int MaLoaiBan { get; set; }

        public LoaiBan LoaiBan { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số chỗ ngồi phải lớn hơn 0")]
        public int SoChoNgoi { get; set; }

        [Required(ErrorMessage = "Vị trí không được để trống")]
        [StringLength(100)]
        public string ViTri { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        [Required]
        [StringLength(30)]
        [RegularExpression(BanAnTrangThai.Pattern, ErrorMessage = "Trạng thái không hợp lệ.")]
        public string TrangThai { get; set; } = BanAnTrangThai.SanSang;

      
        // public ICollection<PhieuDatBan> PhieuDatBans { get; set; } = new List<PhieuDatBan>();
    }
}

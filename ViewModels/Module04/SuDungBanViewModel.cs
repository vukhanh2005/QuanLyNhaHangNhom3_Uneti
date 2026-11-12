namespace QuanLyNhaHang.ViewModels.Module04
{
    public class SuDungBanViewModel
    {
        public int MaPhieuDatBan { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string TenBan { get; set; } = string.Empty;
        public string TenLoaiBan { get; set; } = string.Empty;
        public DateTime NgayGioDat { get; set; }
        public int SoNguoi { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
    }
}

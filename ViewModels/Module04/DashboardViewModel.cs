namespace QuanLyNhaHang.ViewModels.Module04
{
    public class DashboardViewModel
    {
        public int TongSoLoaiBan { get; set; }
        public int TongSoBan { get; set; }
        public int TongSoMonAn { get; set; }
        public int TongSoKhachHang { get; set; }
        public int TongSoPhieuDatBan { get; set; }
        public int SoPhieuChoXacNhan { get; set; }
        public int SoPhieuDaXacNhan { get; set; }
        public int SoBanDangSuDung { get; set; }
        public int SoBanSanSang { get; set; }

        public List<ThongKeBanViewModel> SoBanTheoLoai { get; set; } = new();
        public List<ThongKeBanViewModel> SoBanTheoTrangThai { get; set; } = new();
    }

    public class ThongKeBanViewModel
    {
        public string Nhom { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }
}

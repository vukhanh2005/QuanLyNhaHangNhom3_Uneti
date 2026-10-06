namespace QuanLyNhaHang.ViewModels.Module04
{
    public class DashboardViewModel
    {
        // Tổng số loại bàn
        public int TongSoLoaiBan { get; set; }

        // Tổng số bàn ăn
        public int TongSoBan { get; set; }

        // Tổng số món ăn
        public int TongSoMonAn { get; set; }

        // Số bàn đang sử dụng
        public int SoBanDangSuDung { get; set; }

        // Số bàn sẵn sàng
        public int SoBanSanSang { get; set; }

        // Thống kê số bàn theo loại
        public List<ThongKeBanViewModel> SoBanTheoLoai { get; set; }
            = new List<ThongKeBanViewModel>();

        // Thống kê số bàn theo trạng thái
        public List<ThongKeBanViewModel> SoBanTheoTrangThai { get; set; }
            = new List<ThongKeBanViewModel>();
    }

    public class ThongKeBanViewModel
    {
        public string Nhom { get; set; } = string.Empty;

        public int SoLuong { get; set; }
    }
}
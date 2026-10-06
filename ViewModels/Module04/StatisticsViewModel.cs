namespace QuanLyNhaHang.ViewModels.Module04
{
    public class StatisticsViewModel
    {
        // Tổng số bàn
        public int TongSoBan { get; set; }

        // Tổng số món ăn
        public int TongSoMonAn { get; set; }

        // Số bàn theo loại
        public List<ThongKeViewModel> SoBanTheoLoai { get; set; }
            = new List<ThongKeViewModel>();

        // Số bàn theo trạng thái
        public List<ThongKeViewModel> SoBanTheoTrangThai { get; set; }
            = new List<ThongKeViewModel>();
    }

    public class ThongKeViewModel
    {
        // Tên nhóm thống kê
        public string Nhom { get; set; } = string.Empty;

        // Số lượng
        public int SoLuong { get; set; }
    }
}
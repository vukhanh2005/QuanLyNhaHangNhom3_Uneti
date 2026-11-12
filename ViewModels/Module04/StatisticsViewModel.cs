namespace QuanLyNhaHang.ViewModels.Module04
{
    public class StatisticsViewModel
    {
        public int TongSoBan { get; set; }
        public int TongSoMonAn { get; set; }
        public int TongSoPhieuDatBan { get; set; }

        // 1. Số bàn theo loại
        public List<ThongKeViewModel> SoBanTheoLoai { get; set; } = new();

        // 2. Số bàn theo trạng thái
        public List<ThongKeViewModel> SoBanTheoTrangThai { get; set; } = new();

        // 3. Phiếu đặt bàn theo trạng thái
        public List<ThongKeViewModel> PhieuDatBanTheoTrangThai { get; set; } = new();

        // 4. Phiếu đặt bàn theo bàn
        public List<ThongKeViewModel> PhieuTheoBan { get; set; } = new();

        // 5. Phiếu đặt bàn theo loại bàn
        public List<ThongKeViewModel> PhieuTheoLoaiBan { get; set; } = new();

        // 6. Bàn được đặt nhiều nhất
        public List<ThongKeViewModel> BanDuocDatNhieuNhat { get; set; } = new();

        // 7. Phiếu đặt bàn theo khách hàng
        public List<ThongKeViewModel> PhieuTheoKhachHang { get; set; } = new();

        // 8. Phiếu đặt bàn theo tháng
        public List<ThongKeViewModel> PhieuTheoThang { get; set; } = new();
    }

    public class ThongKeViewModel
    {
        public string Nhom { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }
}

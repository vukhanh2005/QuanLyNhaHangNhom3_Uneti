namespace QuanLyNhaHang.Models;

// Module 3: nhãn và màu trạng thái dùng chung cho lịch sử và chi tiết phiếu.
public static class PhieuDatBanHienThi
{
    public static string TenTrangThai(TrangThaiPhieuDatBan trangThai) => trangThai switch
    {
        TrangThaiPhieuDatBan.ChoXacNhan => "Chờ xác nhận",
        TrangThaiPhieuDatBan.DaXacNhan => "Đã xác nhận",
        TrangThaiPhieuDatBan.DaSuDung => "Đã sử dụng",
        TrangThaiPhieuDatBan.DaHuy => "Đã hủy",
        _ => "Chưa xác định"
    };

    public static string LopTrangThai(TrangThaiPhieuDatBan trangThai) => trangThai switch
    {
        TrangThaiPhieuDatBan.ChoXacNhan => "status-pending",
        TrangThaiPhieuDatBan.DaXacNhan => "status-confirmed",
        TrangThaiPhieuDatBan.DaSuDung => "status-used",
        TrangThaiPhieuDatBan.DaHuy => "status-cancelled",
        _ => string.Empty
    };
}

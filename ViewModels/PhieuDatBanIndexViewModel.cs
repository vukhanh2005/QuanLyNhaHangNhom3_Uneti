using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public class PhieuDatBanIndexViewModel
{
    public bool IsAdmin { get; set; }
    public IReadOnlyList<PhieuDatBan> PhieuDatBans { get; set; } = [];
}

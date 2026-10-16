// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Dữ liệu danh sách bàn ăn, điều kiện tra cứu và thông tin phân trang.
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.ViewModels;

public class BanAnIndexViewModel
{
    public List<BanAn> BanAns { get; set; } = [];
    public List<int> SoChoOptions { get; set; } = [];
    public string? Keyword { get; set; }
    public int? MaLoaiBan { get; set; }
    public int? SoChoNgoi { get; set; }
    public string? TrangThai { get; set; }
    public string SortOrder { get; set; } = "name_asc";
    public int CurrentPage { get; set; } = 1;
    public int TotalCount { get; set; }
    public int PageSize { get; } = 5;
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public IEnumerable<SelectListItem> LoaiBanOptions { get; set; } = [];
    public IEnumerable<SelectListItem> TrangThaiOptions =>
        BanAnTrangThai.TatCa.Select(x => new SelectListItem(x, x));
}


using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyNhaHang.DTO;

namespace QuanLyNhaHang.ViewModels;

public class PhieuDatBanFormViewModel
{
    public PhieuDatBanRequest Request { get; set; } = new();
    public List<SelectListItem> BanOptions { get; set; } = [];
    public bool CoBanDangPhucVu { get; set; }
    public string NgayToiThieu { get; set; } = "";
}

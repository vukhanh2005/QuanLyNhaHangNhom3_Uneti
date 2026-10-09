using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels.Module04
{
public class PhieuDatBanViewModel
{

public List<PhieuDatBan> DanhSachPhieuDatBan { get; set; }
= new List<PhieuDatBan>();


   
    public string? TenKhachHang { get; set; }

    
    public string? TenBan { get; set; }

    
    public string? TrangThai { get; set; }

  
    public DateTime? NgaySuDung { get; set; }
}


}

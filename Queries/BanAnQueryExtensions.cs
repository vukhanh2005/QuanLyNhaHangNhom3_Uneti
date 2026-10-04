// Họ và tên: Vi Thái Học
// Mã sinh viên: [ĐIỀN MÃ SINH VIÊN]
// Nội dung thực hiện: Quản lý bàn ăn, tìm kiếm, lọc, sắp xếp và phân trang.
using QuanLyNhaHang.ViewModels;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.Queries;

public static class BanAnQueryExtensions
{
    // Chỉ xây dựng biểu thức LINQ; chưa tải dữ liệu về bộ nhớ.
    public static IQueryable<BanAn> TraCuu(this IQueryable<BanAn> query, BanAnIndexViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Keyword))
            query = query.Where(x => x.TenBan.Contains(model.Keyword) || x.ViTri.Contains(model.Keyword));
        if (model.MaLoaiBan.HasValue)
            query = query.Where(x => x.MaLoaiBan == model.MaLoaiBan);
        if (model.SoChoNgoi.HasValue)
            query = query.Where(x => x.SoChoNgoi == model.SoChoNgoi);
        if (!string.IsNullOrWhiteSpace(model.TrangThai))
            query = query.Where(x => x.TrangThai == model.TrangThai);
        // Mã bàn là tiêu chí phụ ổn định khi tên/số chỗ bằng nhau.
        return model.SortOrder switch
        {
            "name_desc" => query.OrderByDescending(x => x.TenBan).ThenBy(x => x.MaBan),
            "seat_asc" => query.OrderBy(x => x.SoChoNgoi).ThenBy(x => x.MaBan),
            "seat_desc" => query.OrderByDescending(x => x.SoChoNgoi).ThenBy(x => x.MaBan),
            _ => query.OrderBy(x => x.TenBan).ThenBy(x => x.MaBan)
        };
    }
}


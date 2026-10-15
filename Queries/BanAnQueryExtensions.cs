// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Tìm kiếm theo tên, vị trí; lọc và sắp xếp bàn ăn bằng LINQ.
using QuanLyNhaHang.ViewModels;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.Queries;

public static class BanAnQueryExtensions
{
    // IQueryable chỉ mô tả truy vấn; dữ liệu chưa được tải về ở bước này.
    public static IQueryable<BanAn> TraCuu(this IQueryable<BanAn> query, BanAnIndexViewModel model)
    {
        // 1. Tìm tên bàn HOẶC vị trí có chứa từ khóa.
        var keyword = model.Keyword?.Trim();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.TenBan.Contains(keyword) || x.ViTri.Contains(keyword));
        }

        // 2. Mỗi Where thêm một điều kiện AND: các bộ lọc được kết hợp với nhau.
        if (model.MaLoaiBan.HasValue)
        {
            query = query.Where(x => x.MaLoaiBan == model.MaLoaiBan);
        }
        if (model.SoChoNgoi.HasValue)
        {
            query = query.Where(x => x.SoChoNgoi == model.SoChoNgoi);
        }
        if (!string.IsNullOrWhiteSpace(model.TrangThai))
        {
            query = query.Where(x => x.TrangThai == model.TrangThai);
        }

        // 3. Sắp xếp trước phân trang. Mã bàn giữ thứ tự ổn định nếu tên/số chỗ bằng nhau.
        switch (model.SortOrder)
        {
            case "name_desc":
                return query.OrderByDescending(x => x.TenBan).ThenBy(x => x.MaBan);
            case "seat_asc":
                return query.OrderBy(x => x.SoChoNgoi).ThenBy(x => x.MaBan);
            case "seat_desc":
                return query.OrderByDescending(x => x.SoChoNgoi).ThenBy(x => x.MaBan);
            default:
                return query.OrderBy(x => x.TenBan).ThenBy(x => x.MaBan);
        }
    }
}


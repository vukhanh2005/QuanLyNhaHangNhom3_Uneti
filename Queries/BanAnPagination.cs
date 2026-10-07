// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Phân trang bàn ăn bằng EF Core sau tìm kiếm, lọc và sắp xếp.
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.ViewModels;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.Queries;

public static class BanAnPagination
{
    // Controller truyền query Include(LoaiBan) khi entity của Module 1 đã được tích hợp.
    public static async Task LoadPageAsync(IQueryable<BanAn> source,
        BanAnIndexViewModel model, int page, CancellationToken cancellationToken = default)
    {
        model.Keyword = model.Keyword?.Trim();
        var query = source.AsNoTracking().TraCuu(model);
        model.TotalCount = await query.CountAsync(cancellationToken);
        model.CurrentPage = Math.Clamp(page, 1, model.TotalPages);
        // Chỉ tải tối đa PageSize bản ghi; không ToList trước Skip/Take.
        model.BanAns = await query
            .Skip((model.CurrentPage - 1) * model.PageSize)
            .Take(model.PageSize)
            .ToListAsync(cancellationToken);
    }
}

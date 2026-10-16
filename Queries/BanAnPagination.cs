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
        // Đếm sau khi lọc để tính đúng số trang của kết quả tìm kiếm.
        model.TotalCount = await query.CountAsync(cancellationToken);
        if (page < 1)
        {
            page = 1;
        }
        if (page > model.TotalPages)
        {
            page = model.TotalPages;
        }
        model.CurrentPage = page;

        // Ví dụ: trang 2, mỗi trang 5 bàn → bỏ 5 bàn đầu, lấy 5 bàn tiếp theo.
        int soBanBoQua = (model.CurrentPage - 1) * model.PageSize;
        // Chỉ tải tối đa PageSize bản ghi; không ToList trước Skip/Take.
        model.BanAns = await query
            .Skip(soBanBoQua)
            .Take(model.PageSize)
            .ToListAsync(cancellationToken);
    }
}

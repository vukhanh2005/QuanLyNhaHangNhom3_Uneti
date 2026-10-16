// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Chuẩn hóa dữ liệu form, kiểm tra tên bàn trùng và ánh xạ dữ liệu bàn ăn.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.ViewModels;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.Queries;

public static class BanAnFormLogic
{
    public static void Normalize(BanAnFormViewModel model)
    {
        model.TenBan = model.TenBan?.Trim() ?? "";
        model.ViTri = model.ViTri?.Trim() ?? "";
        model.MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? null : model.MoTa.Trim();
    }

    // Create truyền null; Edit truyền mã lấy từ route, không lấy từ form.
    // Controller vẫn phải kiểm tra loại bàn tồn tại trước khi SaveChangesAsync.
    public static async Task ValidateNameAsync(AppDbContext context,
        BanAnFormViewModel model, int? currentId, ModelStateDictionary modelState,
        CancellationToken cancellationToken = default)
    {
        Normalize(model);
        if (string.IsNullOrWhiteSpace(model.TenBan))
        {
            modelState.AddModelError(nameof(model.TenBan), "Tên bàn không được để trống.");
            return;
        }

        var cacBanCungTen = context.BanAns.Where(x => x.TenBan == model.TenBan);

        // Khi sửa, không coi chính bàn đang sửa là một bàn trùng tên.
        if (currentId.HasValue)
        {
            cacBanCungTen = cacBanCungTen.Where(x => x.MaBan != currentId.Value);
        }

        bool trungTen = await cacBanCungTen.AnyAsync(cancellationToken);
        if (trungTen)
        {
            modelState.AddModelError(nameof(model.TenBan), "Tên bàn đã tồn tại.");
        }
    }

    // Chỉ gán các trường cho phép sửa; không bao giờ gán MaBan từ dữ liệu gửi lên.
    public static void ApplyTo(BanAnFormViewModel model, BanAn entity)
    {
        Normalize(model);
        entity.TenBan = model.TenBan;
        entity.MaLoaiBan = model.MaLoaiBan;
        entity.SoChoNgoi = model.SoChoNgoi;
        entity.ViTri = model.ViTri;
        entity.MoTa = model.MoTa;
        entity.TrangThai = model.TrangThai;
    }

    public static BanAnFormViewModel FromEntity(BanAn entity)
    {
        return new BanAnFormViewModel
        {
            MaBan = entity.MaBan,
            TenBan = entity.TenBan,
            MaLoaiBan = entity.MaLoaiBan,
            SoChoNgoi = entity.SoChoNgoi,
            ViTri = entity.ViTri,
            MoTa = entity.MoTa,
            TrangThai = entity.TrangThai
        };
    }
}

// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Tạo 20 bàn ăn mẫu từ loại bàn đã có, tránh thêm trùng khi chạy lại.
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.Data;

public static class BanAnSeeder
{
    // Không tạo loại bàn thay Module 1; chỉ dùng khóa ngoại thật đang có.
    public static async Task<int> SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        var types = await context.LoaiBans.AsNoTracking().Where(x => x.TrangThai)
            .OrderBy(x => x.MaLoaiBan).Select(x => x.MaLoaiBan).ToListAsync(cancellationToken);
        if (types.Count == 0)
            throw new InvalidOperationException("Cần ít nhất một loại bàn đang hoạt động trước khi seed bàn ăn.");
        var names = await context.BanAns.Where(x => x.TenBan.StartsWith("Bàn "))
            .Select(x => x.TenBan).ToListAsync(cancellationToken);
        int[] seats = [2, 4, 6, 8];
        string[] positions = ["Tầng 1", "Tầng 2", "Gần cửa sổ", "Phòng VIP", "Ngoài trời"];
        var added = 0;
        for (var i = 1; i <= 20; i++)
        {
            var name = $"Bàn {i:00}";
            if (names.Contains(name)) continue;
            context.BanAns.Add(new BanAn
            {
                TenBan = name, MaLoaiBan = types[(i - 1) % types.Count],
                SoChoNgoi = seats[(i - 1) % seats.Length], ViTri = positions[(i - 1) % positions.Length],
                MoTa = "Bàn mẫu để kiểm tra tra cứu và phân trang.",
                TrangThai = i % 7 == 0 ? BanAnTrangThai.TamNgung
                    : i % 5 == 0 ? BanAnTrangThai.DangSuDung : BanAnTrangThai.SanSang
            });
            added++;
        }
        await context.SaveChangesAsync(cancellationToken);
        return added;
    }
}


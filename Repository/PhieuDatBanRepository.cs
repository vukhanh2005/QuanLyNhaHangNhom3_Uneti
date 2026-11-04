using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.Repository;

public class PhieuDatBanRepository(AppDbContext context)
{
    public Task<KhachHang?> TimKhachHangTheoTaiKhoan(int maTaiKhoan, CancellationToken cancellationToken) =>
        context.KhachHangs.AsNoTracking().Include(x => x.TaiKhoan)
            .FirstOrDefaultAsync(x => x.MaTaiKhoan == maTaiKhoan, cancellationToken);

    public Task<PhieuDatBan?> GetById(int maPhieu, CancellationToken cancellationToken) =>
        context.PhieuDatBans.AsNoTracking().Include(x => x.KhachHang)
            .FirstOrDefaultAsync(x => x.MaPhieu == maPhieu, cancellationToken);

    public async Task Them(PhieuDatBan phieu, CancellationToken cancellationToken)
    {
        context.PhieuDatBans.Add(phieu);
        await context.SaveChangesAsync(cancellationToken);
    }

    // Dọn entity nếu lưu hoặc commit thất bại để không bị lưu lại ở thao tác sau.
    public void BoTheoDoi(PhieuDatBan phieu) => context.Entry(phieu).State = EntityState.Detached;

    public async Task<int> Huy(int maPhieu, CancellationToken cancellationToken)
    {
        return await context.PhieuDatBans.Where(x => x.MaPhieu == maPhieu &&
                    (x.TrangThai == TrangThaiPhieuDatBan.ChoXacNhan ||
                     x.TrangThai == TrangThaiPhieuDatBan.DaXacNhan))
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.TrangThai, TrangThaiPhieuDatBan.DaHuy),
                    cancellationToken);
    }

    public Task<List<PhieuDatBan>> GetAll(int? maTaiKhoan = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.PhieuDatBans.AsNoTracking().Include(x => x.BanAn)
            .Include(x => x.KhachHang).AsQueryable();
        if (maTaiKhoan.HasValue) query = query.Where(x => x.KhachHang.MaTaiKhoan == maTaiKhoan.Value);
        return query.OrderByDescending(x => x.NgayDat).ThenByDescending(x => x.MaPhieu)
            .ToListAsync(cancellationToken);
    }

    // Luồng khách hàng bắt buộc có tài khoản; không dùng mặc định null để đọc toàn bộ phiếu.
    public Task<List<PhieuDatBan>> GetLichSu(int maTaiKhoan,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maTaiKhoan);
        return GetAll(maTaiKhoan, cancellationToken);
    }

    public Task<PhieuDatBan?> GetChiTietCaNhan(int maPhieu, int maTaiKhoan,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maTaiKhoan);
        // Kiểm tra chủ sở hữu ngay trong truy vấn, trước khi tải dữ liệu phiếu.
        return context.PhieuDatBans.AsNoTracking().Include(x => x.BanAn)
            .Include(x => x.KhachHang)
            .FirstOrDefaultAsync(x => x.MaPhieu == maPhieu &&
                x.KhachHang.MaTaiKhoan == maTaiKhoan, cancellationToken);
    }

    public Task<List<BanAn>> GetBanDangPhucVu(CancellationToken cancellationToken = default) =>
        context.BanAns.AsNoTracking().Include(x => x.LoaiBan)
            .Where(x => x.TrangThai == BanAnTrangThai.SanSang || x.TrangThai == BanAnTrangThai.DangSuDung)
            .OrderBy(x => x.TenBan).ThenBy(x => x.MaBan).ToListAsync(cancellationToken);

    public Task<int?> TimMaBan(int maPhieu, CancellationToken cancellationToken) =>
        context.PhieuDatBans.AsNoTracking().Where(x => x.MaPhieu == maPhieu)
            .Select(x => (int?)x.MaBan).FirstOrDefaultAsync(cancellationToken);

    public Task<bool> BiTrungLich(int maBan, DateTime ngay, TimeSpan batDau, TimeSpan ketThuc,
        int? loaiTruMaPhieu, CancellationToken cancellationToken)
    {
        // Hai khoảng [bắt đầu, kết thúc) giao nhau khi đầu mới < cuối cũ và cuối mới > đầu cũ.
        // Vì vậy 18:00–20:00 và 20:00–21:00 được phép nối tiếp; phiếu đã hủy không giữ chỗ.
        var query = context.PhieuDatBans.AsNoTracking().Where(x => x.MaBan == maBan &&
            x.NgaySuDung.Date == ngay && x.TrangThai != TrangThaiPhieuDatBan.DaHuy);
        if (loaiTruMaPhieu.HasValue) query = query.Where(x => x.MaPhieu != loaiTruMaPhieu.Value);
        return query.AnyAsync(x => batDau < x.GioKetThuc && ketThuc > x.GioBatDau, cancellationToken);
    }

    // Toàn bộ kiểm tra lịch và ghi dữ liệu dùng chung context và giao dịch.
    public async Task<T> KhoaBan<T>(int maBan, Func<BanAn?, Task<T>> thucHien,
        Func<T, bool> thanhCong, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        // Khóa cùng hàng bàn trước khi đọc lịch: hai thao tác đồng thời phải lần lượt kiểm tra.
        // Serializable giữ cả khóa khoảng trên lịch cho đến commit, tránh hai lượt cùng thấy bàn trống.
        var ban = context.Database.IsSqlServer()
            ? await context.BanAns.FromSqlInterpolated(
                $"SELECT * FROM [BanAns] WITH (UPDLOCK, HOLDLOCK) WHERE [MaBan] = {maBan}")
                .AsNoTracking().SingleOrDefaultAsync(cancellationToken)
            : await context.BanAns.AsNoTracking().SingleOrDefaultAsync(x => x.MaBan == maBan, cancellationToken);
        var result = await thucHien(ban);
        if (thanhCong(result)) await transaction.CommitAsync(cancellationToken);
        return result;
    }
}

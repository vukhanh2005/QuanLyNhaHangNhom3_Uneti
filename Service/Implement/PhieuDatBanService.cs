using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;
using QuanLyNhaHangNhom3_Uneti.Models;
using QuanLyNhaHang.Repository;

namespace QuanLyNhaHang.Service.Implement;

public class PhieuDatBanService(PhieuDatBanRepository repository, TimeProvider clock) : IPhieuDatBanService
{
    public async Task<PhieuDatBanResult> XacNhan(int maPhieu, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetById(maPhieu, cancellationToken);

        if(result == null)
        {
            return new PhieuDatBanResult{Message = "Co loi xay ra!", IsSuccess = false};
        }
        else
        {
            return new PhieuDatBanResult{Message = "Xac nhan thanh cong", IsSuccess = true};
        }
    }
    private static readonly TimeZoneInfo MuiGioVietNam =
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

    private DateTime LayGioHienTai() => TimeZoneInfo.ConvertTime(clock.GetUtcNow(), MuiGioVietNam).DateTime;

    public async Task<PhieuDatBanResult> TaoPhieu(int maTaiKhoan, PhieuDatBanRequest request,
        CancellationToken cancellationToken = default)
    {
        var errors = KiemTraDuLieu(request);
        if (errors.Count > 0) return DuLieuKhongHopLe(errors);

        PhieuDatBan? phieuMoi = null;
        var thanhCong = false;
        try
        {
            var result = await KhoaBan(request.MaBan, async ban =>
            {
                // Mã khách hàng luôn được tìm từ tài khoản đăng nhập, không nhận từ form.
                var khachHang = await repository.TimKhachHangTheoTaiKhoan(maTaiKhoan, cancellationToken);
                if (khachHang == null || khachHang.TaiKhoan.VaiTro != VaiTro.User)
                    return ThatBai("Không tìm thấy hồ sơ khách hàng của tài khoản đăng nhập.");
                if (!khachHang.TrangThai || !khachHang.TaiKhoan.TrangThai)
                    return ThatBai("Khách hàng hoặc tài khoản đang bị ngừng hoạt động.");

                // Kiểm tra lại ngày ngay trong giao dịch, kể cả khi lời gọi phải chờ khóa qua nửa đêm.
                var loi = KiemTraDuLieu(request);
                if (loi.Count > 0) return DuLieuKhongHopLe(loi);
                var loiBan = KiemTraBan(ban, request.SoNguoi);
                if (loiBan != null) return loiBan;
                if (await repository.BiTrungLich(request.MaBan, request.NgaySuDung!.Value.Date,
                    request.GioBatDau!.Value, request.GioKetThuc!.Value, null, cancellationToken))
                    return ThatBai("Bàn đã có phiếu đặt trong khoảng thời gian này. Vui lòng chọn giờ hoặc bàn khác.",
                        nameof(request.MaBan));

                phieuMoi = KhoiTaoPhieu(khachHang.MaKhachHang, request);
                await repository.Them(phieuMoi, cancellationToken);
                return ThanhCong(phieuMoi.MaPhieu, "Đặt bàn thành công. Phiếu đang chờ xác nhận.");
            }, cancellationToken);
            thanhCong = result.IsSuccess;
            return result;
        }
        finally
        {
            // Save hoặc commit thất bại: không để entity Added/Unchanged còn sót rồi bị lưu ở lần sau.
            if (!thanhCong && phieuMoi != null) repository.BoTheoDoi(phieuMoi);
        }
    }

    public async Task<PhieuDatBanResult> Huy(int maPhieu, int? maTaiKhoan = null,
        CancellationToken cancellationToken = default)
    {
        var maBan = await repository.TimMaBan(maPhieu, cancellationToken);
        if (maBan == null) return ThatBai("Không tìm thấy phiếu đặt bàn.");

        return await KhoaBan(maBan.Value, async _ =>
        {
            var phieu = await repository.GetById(maPhieu, cancellationToken);
            if (phieu == null) return ThatBai("Không tìm thấy phiếu đặt bàn.");
            if (phieu.MaBan != maBan.Value) return ThatBai("Phiếu đã thay đổi. Vui lòng tải lại.");
            if (maTaiKhoan.HasValue && phieu.KhachHang.MaTaiKhoan != maTaiKhoan.Value)
                return ThatBai("Bạn không có quyền hủy phiếu đặt bàn này.");
            if (phieu.TrangThai == TrangThaiPhieuDatBan.DaSuDung)
                return ThatBai("Không thể hủy phiếu đã sử dụng.");
            if (phieu.TrangThai == TrangThaiPhieuDatBan.DaHuy)
                return ThanhCong(maPhieu, "Phiếu đã được hủy.");

            var soDong = await repository.Huy(maPhieu, cancellationToken);
            return soDong == 1 ? ThanhCong(maPhieu, "Hủy phiếu đặt bàn thành công.")
                : ThatBai("Phiếu đã thay đổi. Vui lòng tải lại.");
        }, cancellationToken);
    }

    public Task<List<PhieuDatBan>> GetAll(int? maTaiKhoan = null,
        CancellationToken cancellationToken = default) => repository.GetAll(maTaiKhoan, cancellationToken);

    public Task<List<PhieuDatBan>> GetLichSu(int maTaiKhoan,
        CancellationToken cancellationToken = default) => repository.GetLichSu(maTaiKhoan, cancellationToken);

    public Task<PhieuDatBan?> GetChiTietCaNhan(int maPhieu, int maTaiKhoan,
        CancellationToken cancellationToken = default) =>
        repository.GetChiTietCaNhan(maPhieu, maTaiKhoan, cancellationToken);

    public Task<List<BanAn>> GetBanDangPhucVu(CancellationToken cancellationToken = default) =>
        repository.GetBanDangPhucVu(cancellationToken);

    // Chỉ gọi sau khi dữ liệu đặt bàn đã được kiểm tra hợp lệ.
    private PhieuDatBan KhoiTaoPhieu(int maKhachHang, PhieuDatBanRequest request)
    {
        return new PhieuDatBan
        {
            MaKhachHang = maKhachHang,
            MaBan = request.MaBan,
            NgayDat = LayGioHienTai(),
            NgaySuDung = request.NgaySuDung!.Value.Date,
            GioBatDau = request.GioBatDau!.Value,
            GioKetThuc = request.GioKetThuc!.Value,
            SoNguoi = request.SoNguoi,
            GhiChu = string.IsNullOrWhiteSpace(request.GhiChu) ? null : request.GhiChu.Trim(),
            TrangThai = TrangThaiPhieuDatBan.ChoXacNhan
        };
    }

    private Dictionary<string, string[]> KiemTraDuLieu(PhieuDatBanRequest? request)
    {
        if (request == null) return new() { [string.Empty] = ["Thông tin đặt bàn không hợp lệ."] };
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);
        if (request.NgaySuDung.HasValue && request.NgaySuDung.Value.Date < LayGioHienTai().Date)
            validationResults.Add(new("Ngày sử dụng không được trước ngày hiện tại.", [nameof(request.NgaySuDung)]));
        if (request.GioBatDau.HasValue &&
            (request.GioBatDau.Value < TimeSpan.Zero || request.GioBatDau.Value >= TimeSpan.FromDays(1)))
            validationResults.Add(new("Giờ bắt đầu phải nằm trong một ngày.", [nameof(request.GioBatDau)]));
        if (request.GioKetThuc.HasValue &&
            (request.GioKetThuc.Value < TimeSpan.Zero || request.GioKetThuc.Value >= TimeSpan.FromDays(1)))
            validationResults.Add(new("Giờ kết thúc phải nằm trong một ngày.", [nameof(request.GioKetThuc)]));
        if (request.GioBatDau.HasValue && request.GioKetThuc.HasValue &&
            request.GioKetThuc.Value <= request.GioBatDau.Value)
            validationResults.Add(new("Giờ kết thúc phải sau giờ bắt đầu.", [nameof(request.GioKetThuc)]));
        return validationResults.SelectMany(x => x.MemberNames.DefaultIfEmpty(string.Empty)
                .Select(field => new { Field = field, Message = x.ErrorMessage ?? "Dữ liệu không hợp lệ." }))
            .GroupBy(x => x.Field).ToDictionary(x => x.Key, x => x.Select(e => e.Message).Distinct().ToArray());
    }

    private static PhieuDatBanResult? KiemTraBan(BanAn? ban, int soNguoi)
    {
        if (ban == null) return ThatBai("Bàn không tồn tại.", nameof(PhieuDatBanRequest.MaBan));
        if (ban.TrangThai != BanAnTrangThai.SanSang && ban.TrangThai != BanAnTrangThai.DangSuDung)
            return ThatBai("Bàn đang tạm ngừng phục vụ.", nameof(PhieuDatBanRequest.MaBan));
        return soNguoi > ban.SoChoNgoi
            ? ThatBai($"Số người không được vượt quá {ban.SoChoNgoi} chỗ ngồi của bàn.", nameof(PhieuDatBanRequest.SoNguoi))
            : null;
    }

    private async Task<PhieuDatBanResult> KhoaBan(int maBan,
        Func<BanAn?, Task<PhieuDatBanResult>> thucHien, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.KhoaBan(maBan, thucHien,
                result => result.IsSuccess, cancellationToken);
        }
        catch (Exception ex)
        {
            for (Exception? loi = ex; loi != null; loi = loi.InnerException)
            {
                if (loi is SqlException sql && sql.Errors.Cast<SqlError>()
                    .Any(x => x.Number is 1205 or 1222 or -2))
                {
                    return ThatBai("Có lượt đặt bàn khác đang được xử lý. Vui lòng thử lại.");
                }
            }

            if (ex is DbUpdateException)
                return ThatBai("Không thể lưu phiếu đặt bàn. Vui lòng tải lại và thử lại.");

            throw;
        }
    }

    private static PhieuDatBanResult DuLieuKhongHopLe(Dictionary<string, string[]> errors) =>
        new() { Message = "Thông tin đặt bàn chưa hợp lệ. Vui lòng kiểm tra lại.", Errors = errors };

    private static PhieuDatBanResult ThatBai(string message, string field = "") =>
        new() { Message = message, Errors = new() { [field] = [message] } };

    private static PhieuDatBanResult ThanhCong(int maPhieu, string message) =>
        new() { IsSuccess = true, MaPhieu = maPhieu, Message = message };
}

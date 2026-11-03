using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repository;

namespace QuanLyNhaHang.Service.Implement;

public class MonAnService(MonAnRepository repository) : IMonAnService
{
    public Task<List<MonAn>> GetAll(bool chiDangPhucVu = false) => repository.GetAll(chiDangPhucVu);
    public async Task<MonAnResponse> GetById(int id) => Result(await repository.GetById(id));
    public async Task<MonAnResponse> Them(MonAnRequest request)
    {
        var error = Validate(request);
        if (error != null) return new() { Message = error };
        return Result(await repository.Them(request.ConvertToMonAn()), "Thêm món ăn thành công.");
    }
    public async Task<MonAnResponse> Sua(int id, MonAnRequest request)
    {
        var error = Validate(request);
        if (error != null) return new() { Message = error };
        try { return Result(await repository.Sua(id, request.ConvertToMonAn()), "Sửa món ăn thành công."); }
        catch (DbUpdateConcurrencyException) { return new() { Message = "Món ăn đã thay đổi hoặc bị xóa. Vui lòng tải lại trang." }; }
    }
    public async Task<MonAnResponse> Xoa(int id)
    {
        try { return Result(await repository.Xoa(id), "Xóa món ăn thành công."); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        { return new() { Message = "Không thể xóa món đang được dữ liệu khác sử dụng. Bạn có thể ngừng phục vụ món này." }; }
        catch (DbUpdateConcurrencyException)
        { return new() { Message = "Món ăn đã thay đổi hoặc bị xóa. Vui lòng tải lại trang." }; }
    }
    public async Task<MonAnResponse> CapNhatTrangThai(int id, TrangThaiMonAn trangThai)
    {
        if (!Enum.IsDefined(trangThai)) return new() { Message = "Trạng thái không hợp lệ." };
        var item = await repository.GetById(id);
        if (item == null) return Result(null);
        return await Sua(id, new MonAnRequest { TenMon = item.TenMon, LoaiMon = item.LoaiMon,
            DonGia = item.DonGia, MoTa = item.MoTa, TrangThai = trangThai });
    }
    private static string? Validate(MonAnRequest request)
    {
        var errors = new List<ValidationResult>();
        return Validator.TryValidateObject(request, new ValidationContext(request), errors, true)
            ? null : string.Join(" ", errors.Select(x => x.ErrorMessage));
    }
    private static MonAnResponse Result(MonAn? item, string? message = null)
        => new() { IsSuccess = item != null, Data = item, Message = item == null ? "Không tìm thấy món ăn." : message };
}

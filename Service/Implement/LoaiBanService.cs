using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repository;

namespace QuanLyNhaHang.Service.Implement;

public class LoaiBanService : ILoaiBanService
{
    private readonly LoaiBanRepository repository;

    public LoaiBanService(LoaiBanRepository repository)
    {
        this.repository = repository;
    }

    public Task<List<LoaiBan>> GetAllLoaiBan() => repository.GetAllLoaiBan();

    public async Task<LoaiBanResponse> Them(LoaiBanRequest request)
    {
        var error = Validate(request);
        if (error != null) return new LoaiBanResponse(error, false);
        if (await repository.TrungTen(request.TenLoaiBan))
            return new LoaiBanResponse("Tên loại bàn đã tồn tại.", false);

        return Response(await repository.Them(request.ConvertToLoaiBan()), "Thêm loại bàn thành công.");
    }

    public async Task<LoaiBanResponse> Sua(int MaLoaiBan, LoaiBanRequest request)
    {
        var error = Validate(request);
        if (error != null) return new LoaiBanResponse(error, false);
        if (await repository.TrungTen(request.TenLoaiBan, MaLoaiBan))
            return new LoaiBanResponse("Tên loại bàn đã tồn tại.", false);

        return Response(await repository.Sua(MaLoaiBan, request.ConvertToLoaiBan()), "Sửa loại bàn thành công.");
    }

    public async Task<LoaiBanResponse> Xoa(int MaLoaiBan)
    {
        try
        {
            return Response(await repository.Xoa(MaLoaiBan), "Xóa loại bàn thành công.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            return new LoaiBanResponse("Không thể xóa loại bàn đang được dữ liệu khác sử dụng.", false);
        }
    }

    public async Task<LoaiBanResponse> TimKiem(int MaLoaiBan)
        => Response(await repository.TimKiem(MaLoaiBan));

    public async Task<LoaiBanResponse> CapNhatTrangThai(int MaLoaiBan, bool trangThai)
    {
        var item = await repository.TimKiem(MaLoaiBan);
        if (item == null) return Response(null);
        item.TrangThai = trangThai;
        return Response(await repository.Sua(MaLoaiBan, item), "Cập nhật trạng thái thành công.");
    }

    private static string? Validate(LoaiBanRequest request)
    {
        var errors = new List<ValidationResult>();
        return Validator.TryValidateObject(request, new ValidationContext(request), errors, true)
            ? null : string.Join(" ", errors.Select(x => x.ErrorMessage));
    }

    private static LoaiBanResponse Response(LoaiBan? item, string? message = null)
        => item == null
            ? new LoaiBanResponse("Không tìm thấy loại bàn.", false)
            : new LoaiBanResponse(item.MaLoaiBan, item.TenLoaiBan, item.MoTa ?? "", item.TrangThai, true, message);
}

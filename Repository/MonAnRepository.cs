using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repository;

public class MonAnRepository(AppDbContext context)
{
    public Task<List<MonAn>> GetAll(bool chiDangPhucVu = false)
    {
        var query = context.MonAn.AsNoTracking();
        if (chiDangPhucVu) query = query.Where(x => x.TrangThai == TrangThaiMonAn.DangPhucVu);
        return query.OrderBy(x => x.TenMon).ThenBy(x => x.MaMon).ToListAsync();
    }

    public Task<MonAn?> GetById(int id) => context.MonAn.FirstOrDefaultAsync(x => x.MaMon == id);

    public async Task<MonAn> Them(MonAn item)
    {
        context.MonAn.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<MonAn?> Sua(int id, MonAn data)
    {
        var item = await GetById(id);
        if (item == null) return null;
        item.TenMon = data.TenMon;
        item.LoaiMon = data.LoaiMon;
        item.DonGia = data.DonGia;
        item.MoTa = data.MoTa;
        item.TrangThai = data.TrangThai;
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<MonAn?> Xoa(int id)
    {
        var item = await GetById(id);
        if (item == null) return null;
        context.MonAn.Remove(item);
        await context.SaveChangesAsync();
        return item;
    }
}

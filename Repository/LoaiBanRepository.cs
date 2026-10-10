using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repository
{
    public class LoaiBanRepository
    {
        public AppDbContext context;
        public LoaiBanRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<List<LoaiBan>> GetAllLoaiBan()
        {
            return await context.LoaiBans.AsNoTracking().OrderBy(x => x.TenLoaiBan).ToListAsync();
        }
        public Task<bool> TrungTen(string tenLoaiBan, int? excludedId = null)
        {
            return context.LoaiBans.AnyAsync(x => x.TenLoaiBan == tenLoaiBan.Trim()
                && (!excludedId.HasValue || x.MaLoaiBan != excludedId.Value));
        }
        public async Task<LoaiBan> Them(LoaiBan data)
        {
            context.LoaiBans.Add(data);
            await context.SaveChangesAsync();
            return data;
        }
        public async Task<LoaiBan?> Sua(int MaLoaiBan, LoaiBan NewData)
        {
            LoaiBan? loaiBan = await context.LoaiBans
                .FirstOrDefaultAsync(x => x.MaLoaiBan == MaLoaiBan);

            if (loaiBan == null)
            {
                return null;
            }

            loaiBan.TenLoaiBan = NewData.TenLoaiBan;
            loaiBan.MoTa = NewData.MoTa;
            loaiBan.TrangThai = NewData.TrangThai;

            await context.SaveChangesAsync();

            return loaiBan;
        }
        public async Task<LoaiBan?> Xoa(int MaLoaiBan)
        {
            LoaiBan? target = await context.LoaiBans.FirstOrDefaultAsync(x => x.MaLoaiBan == MaLoaiBan);
            if(target == null)
            {
                return null;
            }
            context.LoaiBans.Remove(target);

            await context.SaveChangesAsync();

            return target;
        }
        public async Task<LoaiBan?> TimKiem(int MaLoaiBan)
        {
            LoaiBan? target = await context.LoaiBans.FirstOrDefaultAsync(x => x.MaLoaiBan == MaLoaiBan);
            if(target == null)
            {
                return null;
            }
            return target;
        }
    }
}

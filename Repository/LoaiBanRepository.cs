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
            return await context.LoaiBans.ToListAsync();
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

            context.Entry(loaiBan).CurrentValues.SetValues(NewData);

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
            await context.SaveChangesAsync();

            return target;
        }
    }
}
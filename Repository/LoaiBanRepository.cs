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
    }
}
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repository
{
    public class AccountRepository
    {
        AppDbContext context;
        public AccountRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<List<TaiKhoan>> getAllAccounts()
        {
            return await context.TaiKhoans.ToListAsync();
        }
        public async Task<TaiKhoan> getAccountByUsername(string username)
        {
            List<TaiKhoan> accounts = await getAllAccounts();
            TaiKhoan result = null;

            foreach(var account in accounts)
            {
                if (account.TenDangNhap.Equals(username.Trim()))
                {
                    result = account;
                }
            }
            return result;
        }
        public TaiKhoan getAccountByPassword(string password)
        {
            return null;
        }
    }
}
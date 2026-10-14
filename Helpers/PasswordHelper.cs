using Microsoft.AspNetCore.Identity;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Helpers
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<TaiKhoan> _hasher = new PasswordHasher<TaiKhoan>();

        public static string HashPassword(TaiKhoan user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        public static bool VerifyPassword(TaiKhoan user, string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword))
            {
                return false;
            }

            var result = _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
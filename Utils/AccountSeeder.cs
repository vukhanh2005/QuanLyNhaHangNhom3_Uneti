using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Utils;

public static class AccountSeeder
{
    public static async Task<bool> CreateAsync(AppDbContext context, string username,
        string password, string hoTen, string email, VaiTro vaiTro)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(hoTen);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!Enum.IsDefined(vaiTro))
            throw new ArgumentOutOfRangeException(nameof(vaiTro));

        var account = new TaiKhoan
        {
            TenDangNhap = username.Trim(),
            MatKhau = password,
            HoTen = hoTen.Trim(),
            Email = email.Trim(),
            VaiTro = vaiTro,
            TrangThai = true
        };

        foreach (var property in new[] { nameof(TaiKhoan.TenDangNhap), nameof(TaiKhoan.MatKhau),
            nameof(TaiKhoan.HoTen), nameof(TaiKhoan.Email) })
        {
            Validator.ValidateProperty(typeof(TaiKhoan).GetProperty(property)!.GetValue(account),
                new ValidationContext(account) { MemberName = property });
        }

        if (await context.TaiKhoans.AnyAsync(x => x.TenDangNhap == account.TenDangNhap))
            return false;

        context.TaiKhoans.Add(account);
        await context.SaveChangesAsync();
        return true;
    }
}

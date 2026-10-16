// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Yêu cầu đăng nhập và kiểm tra vai trò trước khi truy cập Module 2.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyNhaHang.Filters;

public sealed class Module2SessionAttribute : Attribute, IAuthorizationFilter, IOrderedFilter
{
    // Kiểm tra đăng nhập trước filter kiểm tra quyền Admin.
    public int Order => -1000;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        int? maTaiKhoan = session.GetInt32("MaTaiKhoan");
        string? vaiTro = session.GetString("VaiTro");

        if (!maTaiKhoan.HasValue || maTaiKhoan.Value <= 0)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        if (vaiTro != "Admin" && vaiTro != "User")
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}

// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Kiểm tra quyền Admin qua Session cho các thao tác quản lý bàn ăn.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyNhaHang.Filters;


public sealed class Module2AdminAttribute : Attribute, IAuthorizationFilter
{
    public static bool IsAdmin(ISession session)
    {
        int? maTaiKhoan = session.GetInt32("MaTaiKhoan");
        string? vaiTro = session.GetString("VaiTro");
        return maTaiKhoan.HasValue && maTaiKhoan.Value > 0 && vaiTro == "Admin";
    }

    // Ẩn nút ở View chưa đủ: filter chặn cả URL trực tiếp và POST của người không có quyền.
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!IsAdmin(context.HttpContext.Session))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}


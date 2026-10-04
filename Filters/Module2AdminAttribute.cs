// Họ và tên: Vi Thái Học
// Mã sinh viên: [ĐIỀN MÃ SINH VIÊN]
// Nội dung thực hiện: Quản lý bàn ăn, tìm kiếm, lọc, sắp xếp và phân trang.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyNhaHang.Filters;

// Hợp đồng tích hợp: MaTaiKhoan = SetInt32; VaiTro = SetString("Admin").
// Không đọc quyền từ form/query string và không cung cấp đăng nhập giả.
public sealed class Module2AdminAttribute : Attribute, IAuthorizationFilter
{
    public static bool IsAdmin(ISession session) =>
        session.GetInt32("MaTaiKhoan") is > 0 && session.GetString("VaiTro") == "Admin";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!IsAdmin(context.HttpContext.Session))
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.ViewModels;

namespace QuanLyNhaHang.Controllers.Module03;

[AutoValidateAntiforgeryToken]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class KhachHangController(IKhachHangService service) : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        if (!maTaiKhoan.HasValue || maTaiKhoan.Value <= 0)
            context.Result = RedirectToAction("Login", "Account");
        else
        {
            var action = context.ActionDescriptor.RouteValues["action"];
            var caNhan = string.Equals(action, nameof(CaNhan), StringComparison.OrdinalIgnoreCase)
                || string.Equals(action, nameof(CapNhatCaNhan), StringComparison.OrdinalIgnoreCase);
            var role = caNhan ? nameof(VaiTro.User) : nameof(VaiTro.Admin);
            if (HttpContext.Session.GetString("VaiTro") != role)
                context.Result = StatusCode(StatusCodes.Status403Forbidden);
        }
        base.OnActionExecuting(context);
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? tuKhoa = null, bool? trangThai = null)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        ViewData["TuKhoa"] = tuKhoa?.Trim();
        ViewData["TrangThai"] = trangThai;
        return View("QuanLyKhachHang", await service.GetAll(tuKhoa, trangThai));
    }

    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id, string? tuKhoa = null, bool? locTrangThai = null)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await service.GetById(id);
        ViewData["TuKhoa"] = tuKhoa;
        ViewData["TrangThai"] = locTrangThai;
        return result.IsSuccess ? View(result.Data) : NotFound(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> CapNhatTrangThai(int id, bool? trangThai, string? tuKhoa = null, bool? locTrangThai = null)
    {
        if (!ModelState.IsValid || trangThai == null) return BadRequest("Trạng thái không hợp lệ.");
        if (!(await service.GetById(id)).IsSuccess) return NotFound("Không tìm thấy khách hàng.");
        var result = await service.CapNhatTrangThai(id, trangThai.Value);
        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Index), new { tuKhoa, trangThai = locTrangThai });
    }

    [HttpGet]
    public async Task<IActionResult> CaNhan()
    {
        var result = await service.GetCaNhan(HttpContext.Session.GetInt32("MaTaiKhoan")!.Value);
        return View(TaoModelCaNhan(result.Data));
    }

    [HttpPost]
    public async Task<IActionResult> CapNhatCaNhan([Bind(Prefix = "Request")] KhachHangRequest request)
    {
        // Chỉ dùng tài khoản trong Session; request không chứa mã hoặc trạng thái hồ sơ.
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
        var hienTai = await service.GetCaNhan(maTaiKhoan);
        if (!hienTai.IsSuccess || hienTai.Data == null)
            return View(nameof(CaNhan), TaoModelCaNhan(null));

        if (!ModelState.IsValid)
            return View(nameof(CaNhan), TaoModelCaNhan(hienTai.Data, request));

        var result = await service.CapNhatCaNhan(maTaiKhoan, request);
        if (!result.IsSuccess || result.Data == null)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Không thể cập nhật thông tin. Vui lòng thử lại.");
            var moiNhat = await service.GetCaNhan(maTaiKhoan);
            return View(nameof(CaNhan), TaoModelCaNhan(moiNhat.Data, request));
        }
        HttpContext.Session.SetString("HoTen", result.Data!.HoTen);
        TempData["KhachHangSuccess"] = result.Message ?? "Cập nhật thông tin cá nhân thành công.";
        return RedirectToAction(nameof(CaNhan));
    }

    private static KhachHangCaNhanViewModel TaoModelCaNhan(KhachHangResponse? thongTin,
        KhachHangRequest? request = null) => new()
    {
        ThongTin = thongTin,
        Request = request ?? (thongTin == null ? new() : new KhachHangRequest
        {
            HoTen = thongTin.HoTen,
            NgaySinh = thongTin.NgaySinh,
            GioiTinh = thongTin.GioiTinh,
            SoDienThoai = thongTin.SoDienThoai,
            Email = thongTin.Email,
            DiaChi = thongTin.DiaChi
        }),
        ThongBao = thongTin == null
            ? "Tài khoản của bạn chưa có hồ sơ khách hàng. Vui lòng liên hệ nhà hàng để được bổ sung thông tin trước khi đặt bàn."
            : null
    };
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Filters;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.ViewModels;

namespace QuanLyNhaHang.Controllers.Module03;

[AutoValidateAntiforgeryToken]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class PhieuDatBanController(IPhieuDatBanService service) : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        if (!maTaiKhoan.HasValue || maTaiKhoan.Value <= 0)
        {
            context.Result = RedirectToAction("Login", "Account");
            return;
        }

        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro != "Admin" && vaiTro != "User")
        {
            context.Result = StatusCode(StatusCodes.Status403Forbidden);
            return;
        }

        base.OnActionExecuting(context);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (IsCustomer()) return RedirectToAction(nameof(LichSu));

        List<PhieuDatBan> list = await service.GetAll();

        return View("Admin/Index", list);
    }
    [HttpPost]
    public async Task<IActionResult> XacNhan(int idPhieu, CancellationToken cancellationToken = default)
    {
        PhieuDatBanResult datBan = await service.XacNhan(idPhieu, cancellationToken);
        ViewBag.IsSuccess = datBan.IsSuccess;
        ViewBag.SuccessMessage = datBan.Message;
        ViewBag.MaPhieu = datBan.MaPhieu;
        return View("Admin/Index");
    }
    [HttpGet]
    public async Task<IActionResult> LichSu(CancellationToken cancellationToken = default)
    {
        if (!IsCustomer()) return StatusCode(StatusCodes.Status403Forbidden);
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
        return View(await service.GetLichSu(maTaiKhoan, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id, CancellationToken cancellationToken = default)
    {
        if (!IsCustomer()) return StatusCode(StatusCodes.Status403Forbidden);
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
        var phieu = ModelState.IsValid && id > 0
            ? await service.GetChiTietCaNhan(id, maTaiKhoan, cancellationToken) : null;
        // Phiếu không tồn tại và phiếu của người khác có cùng phản hồi.
        if (phieu == null) Response.StatusCode = StatusCodes.Status404NotFound;
        return View("ChiTiet", phieu);
    }

    [HttpGet]
    public async Task<IActionResult> DatBan(int? maBan = null, CancellationToken cancellationToken = default)
    {
        if (!IsCustomer()) return StatusCode(StatusCodes.Status403Forbidden);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var request = new PhieuDatBanRequest
        {
            MaBan = maBan ?? 0,
            NgaySuDung = NgayHienTai(),
            SoNguoi = 1
        };
        return View(await TaoForm(request, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> DatBan([Bind(Prefix = "Request")] PhieuDatBanRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsCustomer()) return StatusCode(StatusCodes.Status403Forbidden);
        if (ModelState.IsValid)
        {
            // Danh tính lấy từ phiên đăng nhập, không nhận mã khách hàng từ biểu mẫu.
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            var result = await service.TaoPhieu(maTaiKhoan, request, cancellationToken);
            if (result.IsSuccess)
            {
                TempData["PhieuDatBanSuccess"] = result.Message;
                return RedirectToAction(nameof(ChiTiet), new { id = result.MaPhieu });
            }
            foreach (var error in result.Errors)
            {
                var key = string.IsNullOrEmpty(error.Key) ? "" : $"Request.{error.Key}";
                foreach (var message in error.Value) ModelState.AddModelError(key, message);
            }
            if (result.Errors.Count == 0)
                ModelState.AddModelError("", result.Message);
        }
        return View(await TaoForm(request, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Huy([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid || id <= 0) return BadRequest("Mã phiếu không hợp lệ.");
        var isAdmin = Module2AdminAttribute.IsAdmin(HttpContext.Session);
        var maTaiKhoan = isAdmin
            ? (int?)null : HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
        var result = await service.Huy(id, maTaiKhoan, cancellationToken);
        TempData[result.IsSuccess ? "PhieuDatBanSuccess" : "PhieuDatBanError"] = result.Message;
        if (isAdmin) return RedirectToAction("Index", "TrangChu");
        return RedirectToAction(nameof(LichSu));
    }

    private bool IsCustomer() => HttpContext.Session.GetString("VaiTro") == "User";

    private static DateTime NgayHienTai() =>
        TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, "SE Asia Standard Time").Date;

    private async Task<PhieuDatBanFormViewModel> TaoForm(PhieuDatBanRequest request,
        CancellationToken cancellationToken)
    {
        var banAns = await service.GetBanDangPhucVu(cancellationToken);
        var options = banAns.Select(x => new SelectListItem
        {
            Value = x.MaBan.ToString(),
            Text = $"{x.TenBan} · {x.SoChoNgoi} chỗ · {x.ViTri}"
        }).ToList();
        // Giữ mã bàn đã chọn để người dùng hiểu lỗi khi bàn vừa ngừng phục vụ.
        if (request.MaBan > 0 && banAns.All(x => x.MaBan != request.MaBan))
        {
            options.Insert(0, new SelectListItem($"Bàn #{request.MaBan} không còn phục vụ", request.MaBan.ToString()));
        }
        return new PhieuDatBanFormViewModel
        {
            Request = request,
            BanOptions = options,
            CoBanDangPhucVu = banAns.Count > 0,
            NgayToiThieu = NgayHienTai().ToString("yyyy-MM-dd")
        };
    }
}

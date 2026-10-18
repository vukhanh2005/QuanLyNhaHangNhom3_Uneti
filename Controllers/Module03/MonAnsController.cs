using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Controllers.Module03;

[AutoValidateAntiforgeryToken]
public class MonAnController(IMonAnService service) : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!string.Equals(context.ActionDescriptor.RouteValues["action"], nameof(ThucDon), StringComparison.OrdinalIgnoreCase))
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
                context.Result = RedirectToAction("Login", "Account");
            else if (HttpContext.Session.GetString("VaiTro") != nameof(VaiTro.Admin))
                context.Result = StatusCode(StatusCodes.Status403Forbidden);
        }
        base.OnActionExecuting(context);
    }

    [HttpGet]
    public async Task<IActionResult> Index() => View(await service.GetAll());

    [HttpGet]
    public async Task<IActionResult> ThucDon() => View(await service.GetAll(true));

    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id)
    {
        var result = await service.GetById(id);
        return result.IsSuccess ? View(result.Data) : NotFound();
    }

    [HttpGet]
    public IActionResult Them() => View("Form", new MonAnRequest());

    [HttpPost]
    public async Task<IActionResult> Them(MonAnRequest request)
    {
        if (!ModelState.IsValid) return View("Form", request);
        var result = await service.Them(request);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Không thể thêm món ăn.");
            return View("Form", request);
        }
        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Sua(int id)
    {
        var result = await service.GetById(id);
        if (!result.IsSuccess) return NotFound();
        var item = result.Data!;
        ViewData["Id"] = id;
        return View("Form", new MonAnRequest
        {
            TenMon = item.TenMon, LoaiMon = item.LoaiMon, DonGia = item.DonGia,
            MoTa = item.MoTa, TrangThai = item.TrangThai
        });
    }

    [HttpPost]
    public async Task<IActionResult> Sua(int id, MonAnRequest request)
    {
        if (!(await service.GetById(id)).IsSuccess) return NotFound();
        ViewData["Id"] = id;
        if (!ModelState.IsValid) return View("Form", request);
        var result = await service.Sua(id, request);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Không thể sửa món ăn.");
            return View("Form", request);
        }
        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Xoa(int id)
    {
        var result = await service.GetById(id);
        if (!result.IsSuccess) return NotFound();
        ViewData["ConfirmDelete"] = true;
        return View("ChiTiet", result.Data);
    }

    [HttpPost, ActionName("Xoa")]
    public async Task<IActionResult> XacNhanXoa(int id)
    {
        var item = await service.GetById(id);
        if (!item.IsSuccess) return NotFound();
        var result = await service.Xoa(id);
        if (!result.IsSuccess)
        {
            ViewData["ConfirmDelete"] = true;
            ModelState.AddModelError(string.Empty, result.Message ?? "Không thể xóa món ăn.");
            return View("ChiTiet", item.Data);
        }
        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CapNhatTrangThai(int id, TrangThaiMonAn? trangThai)
    {
        if (!ModelState.IsValid || trangThai == null || !Enum.IsDefined(trangThai.Value)) return BadRequest();
        if (!(await service.GetById(id)).IsSuccess) return NotFound();
        var result = await service.CapNhatTrangThai(id, trangThai.Value);
        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Filters;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Controllers.Module01;
[Module2Admin]
[AutoValidateAntiforgeryToken]
public class LoaiBanController : Controller
{
    private readonly ILoaiBanService service;

    public LoaiBanController(ILoaiBanService service)
    {
        this.service = service;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
            context.Result = RedirectToAction("Login", "Account");
        else if (HttpContext.Session.GetString("VaiTro") != nameof(VaiTro.Admin))
            context.Result = StatusCode(StatusCodes.Status403Forbidden);
        base.OnActionExecuting(context);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await service.GetAllLoaiBan());
    }

    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id)
    {
        var item = await service.TimKiem(id);
        return item.IsSuccess ? View(item) : NotFound();
    }

    [HttpGet]
    public IActionResult Them()
    {
        return View("Form", new LoaiBanRequest());
    }

    [HttpPost]
    public async Task<IActionResult> Them(LoaiBanRequest request)
    {
        if (!ModelState.IsValid) return View("Form", request);
        var result = await service.Them(request);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.message ?? "Không thể thêm loại bàn.");
            return View("Form", request);
        }
        TempData["SuccessMessage"] = result.message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Sua(int id)
    {
        var item = await service.TimKiem(id);
        if (!item.IsSuccess) return NotFound();
        ViewData["Id"] = id;
        return View("Form", new LoaiBanRequest
        {
            TenLoaiBan = item.TenLoaiBan, MoTa = item.MoTa, TrangThai = item.TrangThai
        });
    }

    [HttpPost]
    public async Task<IActionResult> Sua(int id, LoaiBanRequest request)
    {
        if (!(await service.TimKiem(id)).IsSuccess) return NotFound();
        ViewData["Id"] = id;
        if (!ModelState.IsValid) return View("Form", request);
        var result = await service.Sua(id, request);
        if (!result.IsSuccess)
        {
            ViewData["FailMessage"] = result.message;
            return View("Form", request);
        }
        TempData["SuccessMessage"] = result.message;
        return RedirectToAction(nameof(Index)); 
    }

    [HttpGet]
    public async Task<IActionResult> Xoa(int id)
    {
        var item = await service.TimKiem(id);
        if (!item.IsSuccess) return NotFound();
        ViewData["ConfirmDelete"] = true;
        return View("ChiTiet", item);
    }

    [HttpPost, ActionName("Xoa")]
    public async Task<IActionResult> XacNhanXoa(int id)
    {
        var item = await service.TimKiem(id);
        if (!item.IsSuccess) return NotFound();
        var result = await service.Xoa(id);
        if (!result.IsSuccess)
        {
            ViewData["ConfirmDelete"] = true;
            ViewData["FailMessage"] = result.message;
            return View("ChiTiet", item);
        }
        TempData["SuccessMessage"] = result.message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CapNhatTrangThai(int id, bool? trangThai)
    {
        if (!ModelState.IsValid || trangThai == null) return BadRequest();
        var result = await service.CapNhatTrangThai(id, trangThai.Value);
        if (!result.IsSuccess) return NotFound();
        TempData["SuccessMessage"] = result.message;
        return RedirectToAction(nameof(Index));
    }
}

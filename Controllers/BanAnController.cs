// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: CRUD bàn ăn, kiểm tra dữ liệu, phân quyền và điều phối tra cứu, phân trang.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Filters;
using QuanLyNhaHang.Queries;
using QuanLyNhaHang.ViewModels;
using QuanLyNhaHangNhom3_Uneti.Models;

namespace QuanLyNhaHang.Controllers;

[Module2Session]
public class BanAnController : Controller
{
    private readonly AppDbContext context;

    public BanAnController(AppDbContext context)
    {
        this.context = context;
    }

    // GET: đọc điều kiện trên URL, truy vấn dữ liệu rồi chọn giao diện theo vai trò.
    public async Task<IActionResult> Index(string? keyword, int? maLoaiBan, int? soChoNgoi,
        string? trangThai, string? sortOrder, int page = 1, CancellationToken cancellationToken = default)
    {
        // 1. Nếu kiểu sắp xếp không hợp lệ, mặc định tên bàn A → Z.
        if (sortOrder != "name_desc" && sortOrder != "seat_asc" && sortOrder != "seat_desc")
        {
            sortOrder = "name_asc";
        }

        var model = new BanAnIndexViewModel
        {
            Keyword = keyword?.Trim(),
            MaLoaiBan = maLoaiBan,
            SoChoNgoi = soChoNgoi,
            TrangThai = trangThai,
            SortOrder = sortOrder,
            LoaiBanOptions = await LoaiBanOptionsAsync(cancellationToken)
        };
        // 2. Include lấy tên loại bàn. Hàm phân trang xử lý tìm kiếm → lọc → sắp xếp → Skip/Take.
        await BanAnPagination.LoadPageAsync(context.BanAns.Include(x => x.LoaiBan),
            model, page, cancellationToken);
        // 3. Lấy các số chỗ khác nhau để hiển thị bộ lọc.
        model.SoChoOptions = await context.BanAns.Select(x => x.SoChoNgoi).Distinct()
            .OrderBy(x => x).ToListAsync(cancellationToken);
        if (soChoNgoi.HasValue && !model.SoChoOptions.Contains(soChoNgoi.Value))
        {
            // Giữ lựa chọn trên URL ngay cả khi không có bàn phù hợp.
            model.SoChoOptions = model.SoChoOptions.Append(soChoNgoi.Value).OrderBy(x => x).ToList();
        }

        // 4. Cùng dữ liệu tra cứu, nhưng mỗi vai trò nhận một giao diện riêng.
        if (Module2AdminAttribute.IsAdmin(HttpContext.Session))
        {
            return View("Admin", model);
        }
        return View("Client", model);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity == null)
        {
            return NotFound();
        }
        return View(entity);
    }

    [Module2Admin]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new BanAnFormViewModel();
        model.LoaiBanOptions = await LoaiBanOptionsAsync(cancellationToken);
        return View(model);
    }

    [HttpPost, Module2Admin, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BanAnFormViewModel model, CancellationToken cancellationToken)
    {
        model.MaBan = null;
        await ValidateFormAsync(model, null, cancellationToken);
        if (ModelState.IsValid)
        {
            var entity = new BanAn();
            BanAnFormLogic.ApplyTo(model, entity);
            context.BanAns.Add(entity);
            if (await SaveAsync(cancellationToken))
            {
                TempData["BanAnSuccess"] = "Đã thêm bàn ăn.";
                return RedirectToAction(nameof(Index));
            }
        }
        model.LoaiBanOptions = await LoaiBanOptionsAsync(cancellationToken);
        return View(model);
    }

    [Module2Admin]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = BanAnFormLogic.FromEntity(entity);
        model.LoaiBanOptions = await LoaiBanOptionsAsync(cancellationToken);
        return View(model);
    }

    [HttpPost, Module2Admin, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, BanAnFormViewModel model,
        CancellationToken cancellationToken)
    {
        var entity = await context.BanAns.FirstOrDefaultAsync(x => x.MaBan == id, cancellationToken);
        if (entity is null) return NotFound();
        model.MaBan = id; // Không tin mã bàn do form gửi lên.
        await ValidateFormAsync(model, id, cancellationToken);
        if (ModelState.IsValid)
        {
            BanAnFormLogic.ApplyTo(model, entity);
            if (await SaveAsync(cancellationToken))
            {
                TempData["BanAnSuccess"] = "Đã cập nhật bàn ăn.";
                return RedirectToAction(nameof(Index));
            }
        }
        model.LoaiBanOptions = await LoaiBanOptionsAsync(cancellationToken);
        return View(model);
    }

    [Module2Admin]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity == null)
        {
            return NotFound();
        }
        return View(entity);
    }

    [HttpPost, ActionName("Delete"), Module2Admin, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken cancellationToken)
    {
        var entity = await context.BanAns.Include(x => x.LoaiBan)
            .FirstOrDefaultAsync(x => x.MaBan == id, cancellationToken);
        if (entity is null) return NotFound();
        // Module 3: thêm AnyAsync(PhieuDatBan.MaBan == id) tại đây và FK Restrict.
        // Không tạo entity phiếu giả. FK Restrict sẽ chặn cả xóa khi phát sinh quan hệ đồng thời.
        if (entity.TrangThai == BanAnTrangThai.DangSuDung)
        {
            ModelState.AddModelError("", "Không thể xóa bàn đang sử dụng.");
            return View("Delete", entity);
        }
        context.BanAns.Remove(entity);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            TempData["BanAnSuccess"] = "Đã xóa bàn ăn.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            ModelState.AddModelError("", "Bàn có dữ liệu liên quan nên không thể xóa. Hãy chuyển sang Tạm ngừng phục vụ.");
            return View("Delete", entity);
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }
    }

    // Chỉ đọc dữ liệu nên không cần EF theo dõi thay đổi (AsNoTracking).
    private async Task<BanAn?> FindAsync(int id, CancellationToken cancellationToken)
    {
        return await context.BanAns.AsNoTracking().Include(x => x.LoaiBan)
            .FirstOrDefaultAsync(x => x.MaBan == id, cancellationToken);
    }

    private async Task<List<SelectListItem>> LoaiBanOptionsAsync(CancellationToken cancellationToken)
    {
        return await context.LoaiBans.AsNoTracking().OrderBy(x => x.TenLoaiBan)
            .Select(x => new SelectListItem(x.TenLoaiBan, x.MaLoaiBan.ToString()))
            .ToListAsync(cancellationToken);
    }

    private async Task ValidateFormAsync(BanAnFormViewModel model, int? currentId,
        CancellationToken cancellationToken)
    {
        await BanAnFormLogic.ValidateNameAsync(context, model, currentId, ModelState, cancellationToken);
        if (!await context.LoaiBans.AnyAsync(x => x.MaLoaiBan == model.MaLoaiBan, cancellationToken))
            ModelState.AddModelError(nameof(model.MaLoaiBan), "Vui lòng chọn loại bàn tồn tại.");
    }

    private async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            ModelState.AddModelError(nameof(BanAnFormViewModel.TenBan), "Tên bàn đã tồn tại.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            ModelState.AddModelError("", "Loại bàn hoặc dữ liệu đã thay đổi. Vui lòng kiểm tra và thử lại.");
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError("", "Bàn đã bị xóa hoặc thay đổi. Vui lòng tải lại danh sách.");
        }
        return false;
    }
}


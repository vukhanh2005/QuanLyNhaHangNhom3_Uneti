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

// Kiểm tra đăng nhập trước mọi action, kể cả khi người dùng tự nhập URL.
[Module2Session]
public class BanAnController : Controller
{
    private readonly AppDbContext context;

    // ASP.NET Core cung cấp DbContext qua constructor để truy cập database.
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

    // Xem chi tiết theo mã; không tìm thấy bàn thì trả HTTP 404.
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
    // GET: mở form trống và lấy danh sách loại bàn, chưa lưu dữ liệu.
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new BanAnFormViewModel();
        model.LoaiBanOptions = await LoaiBanOptionsAsync(cancellationToken);
        return View(model);
    }

    [HttpPost, Module2Admin, ValidateAntiForgeryToken]
    // POST: nhận form; quyền Admin và token chống giả mạo được kiểm tra trước action.
    public async Task<IActionResult> Create(BanAnFormViewModel model, CancellationToken cancellationToken)
    {
        // Mã bàn mới do database sinh, không lấy từ form.
        model.MaBan = null;
        await ValidateFormAsync(model, null, cancellationToken);
        // Chỉ lưu khi không có lỗi từ validation của ViewModel và kiểm tra nghiệp vụ.
        if (ModelState.IsValid)
        {
            var entity = new BanAn();
            BanAnFormLogic.ApplyTo(model, entity);
            // Add đánh dấu thêm mới; SaveAsync mới thực sự ghi vào database.
            context.BanAns.Add(entity);
            if (await SaveAsync(cancellationToken))
            {
                // TempData giữ thông báo qua redirect; redirect tránh gửi lại POST khi tải lại trang.
                TempData["BanAnSuccess"] = "Đã thêm bàn ăn.";
                return RedirectToAction(nameof(Index));
            }
        }
        model.LoaiBanOptions = await LoaiBanOptionsAsync(cancellationToken);
        return View(model);
    }

    [Module2Admin]
    // GET: lấy thông tin bàn hiện tại để điền vào form sửa.
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var model = BanAnFormLogic.FromEntity(entity);
        model.LoaiBanOptions = await LoaiBanOptionsAsync(cancellationToken);
        return View(model);
    }

    [HttpPost, Module2Admin, ValidateAntiForgeryToken]
    // POST: lấy mã bàn từ URL và chỉ cập nhật các trường được phép sửa.
    public async Task<IActionResult> Edit([FromRoute] int id, BanAnFormViewModel model,
        CancellationToken cancellationToken)
    {
        // EF theo dõi entity này để SaveChangesAsync nhận biết các trường đã thay đổi.
        var entity = await context.BanAns.FirstOrDefaultAsync(x => x.MaBan == id, cancellationToken);
        if (entity is null) return NotFound();
        model.MaBan = id; // Không tin mã bàn do form gửi lên.
        // Phải so với phiên bản lúc Admin mở form, không dùng phiên bản vừa đọc ở POST.
        byte[] phienBanCu;
        try
        {
            phienBanCu = Convert.FromBase64String(model.RowVersion ?? "");
        }
        catch (FormatException)
        {
            phienBanCu = Array.Empty<byte>();
        }
        if (phienBanCu.Length != 8)
        {
            ModelState.AddModelError("", "Thiếu hoặc sai phiên bản dữ liệu. Vui lòng tải lại dữ liệu mới nhất.");
        }
        else if (!entity.RowVersion.SequenceEqual(phienBanCu))
        {
            ModelState.AddModelError("", "Bàn đã được Admin khác cập nhật. Thay đổi của bạn chưa được lưu. Hãy tải lại dữ liệu mới nhất và kiểm tra trước khi sửa tiếp.");
        }
        await ValidateFormAsync(model, id, cancellationToken);
        // Chỉ lưu khi không có lỗi từ validation của ViewModel và kiểm tra nghiệp vụ.
        if (ModelState.IsValid)
        {
            BanAnFormLogic.ApplyTo(model, entity);
            // UPDATE chỉ thành công khi RowVersion vẫn trùng, kể cả có người lưu trong lúc POST xử lý.
            context.Entry(entity).Property(x => x.RowVersion).OriginalValue = phienBanCu;
            // Luôn kiểm tra phiên bản khi bấm Lưu, kể cả dữ liệu nhập không đổi.
            context.Entry(entity).Property(x => x.TenBan).IsModified = true;
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
    // GET: chỉ hiện xác nhận, tuyệt đối chưa xóa bàn.
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
    // POST: chỉ xóa sau xác nhận; ActionName giữ URL /BanAn/Delete.
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
        // Remove đánh dấu xóa; chỉ sau SaveChangesAsync thành công dữ liệu mới bị xóa.
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

    // Dropdown hiển thị tên loại bàn, nhưng giá trị gửi về là mã loại bàn.
    private async Task<List<SelectListItem>> LoaiBanOptionsAsync(CancellationToken cancellationToken)
    {
        return await context.LoaiBans.AsNoTracking().OrderBy(x => x.TenLoaiBan)
            .Select(x => new SelectListItem(x.TenLoaiBan, x.MaLoaiBan.ToString()))
            .ToListAsync(cancellationToken);
    }

    // Không chỉ tin dropdown: người dùng có thể tự gửi một mã loại bàn không tồn tại.
    private async Task ValidateFormAsync(BanAnFormViewModel model, int? currentId,
        CancellationToken cancellationToken)
    {
        await BanAnFormLogic.ValidateNameAsync(context, model, currentId, ModelState, cancellationToken);
        if (!await context.LoaiBans.AnyAsync(x => x.MaLoaiBan == model.MaLoaiBan, cancellationToken))
            ModelState.AddModelError(nameof(model.MaLoaiBan), "Vui lòng chọn loại bàn tồn tại.");
    }

    // Xử lý lỗi lưu chung cho thêm/sửa. SQL 2601/2627: trùng khóa; 547: vi phạm ràng buộc.
    // Unique index vẫn bảo vệ tên bàn nếu hai yêu cầu thêm trùng chạy đồng thời.
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


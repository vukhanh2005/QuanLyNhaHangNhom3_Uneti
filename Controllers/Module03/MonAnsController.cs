
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

public class MonAnController : Controller
{
    private readonly AppDbContext _context;

    public MonAnController(AppDbContext context)
    {
        _context = context;
    }
    

    // GET: MONANS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.MonAn.ToListAsync());
    }

    // GET: MONANS/Details/5
    public async Task<IActionResult> Details(int? mamon)
    {
        if (mamon == null)
        {
            return NotFound();
        }

        var monan = await _context.MonAn
            .FirstOrDefaultAsync(m => m.MaMon == mamon);
        if (monan == null)
        {
            return NotFound();
        }

        return View(monan);
    }

    // GET: MONANS/Create
    public IActionResult Create()
    {
        return View();
    }


    // POST: MONANS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(MonAn monan)
{
    if (!ModelState.IsValid)
    {
        Console.WriteLine("=== DU LIEU KHONG HOP LE ===");

        foreach (var item in ModelState)
        {
            foreach (var error in item.Value.Errors)
            {
                Console.WriteLine($"{item.Key}: {error.ErrorMessage}");
            }
        }

        TempData["Error"] = "Dữ liệu không hợp lệ!";
        return RedirectToAction("Index", "MonAn");
    }

    Console.WriteLine("=== DU LIEU HOP LE ===");

    _context.MonAn.Add(monan);
    await _context.SaveChangesAsync();

    Console.WriteLine("=== THEM MON THANH CONG ===");

    return RedirectToAction("Index", "MonAn");
}

    // GET: MONANS/Edit/5
    public async Task<IActionResult> Edit(int? mamon)
    {
        if (mamon == null)
        {
            return NotFound();
        }

        var monan = await _context.MonAn.FindAsync(mamon);
        if (monan == null)
        {
            return NotFound();
        }
        return View(monan);
    }

    // POST: MONANS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? mamon, [Bind("MaMon,TenMon,LoaiMon,DonGia,MoTa,TrangThai")] MonAn monan)
    {
        if (mamon != monan.MaMon)
        {
            TempData["Error"] = "Mã món ăn không hợp lệ!";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(monan);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Cập nhật món ăn thành công!";

            }
            catch (DbUpdateConcurrencyException)
            {
               TempData["Error"] = "Có lỗi xảy ra khi cập nhật món ăn!";
            } 
        }
        else
        {
            TempData["Error"] = "Dữ liệu không hợp lệ!";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: MONANS/Delete/5
    public async Task<IActionResult> Delete(int? mamon)
    {
        if (mamon == null)
        {
            return NotFound();
        }

        var monan = await _context.MonAn
            .FirstOrDefaultAsync(m => m.MaMon == mamon);
        if (monan == null)
        {
            return NotFound();
        }

        return View(monan);
    }

    // POST: MONANS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? mamon)
    {
        var monan = await _context.MonAn.FindAsync(mamon);
        if (monan != null)
        {   
            try{
            _context.MonAn.Remove(monan);
            TempData["Message"] = "Xóa món ăn thành công!";
        }catch(Exception ex){
            TempData["Error"] = "Có lỗi xảy ra khi xóa món ăn!";
        }
        }
        else
        {
            TempData["Error"] = "Món ăn không tồn tại!";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MonAnExists(int? mamon)
    {
        return _context.MonAn.Any(e => e.MaMon == mamon);
    }
    public async Task<IActionResult> ThucDon()
    {
        var monAnList = await _context.MonAn.ToListAsync();
        return View(monAnList);
    }
}

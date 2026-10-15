
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
    public async Task<IActionResult> Create([Bind("MaMon,TenMon,LoaiMon,DonGia,MoTa,TrangThai")] MonAn monan)
    {
        if (ModelState.IsValid)
        {
            _context.Add(monan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(monan);
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
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(monan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonAnExists(monan.MaMon))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(monan);
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
            _context.MonAn.Remove(monan);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MonAnExists(int? mamon)
    {
        return _context.MonAn.Any(e => e.MaMon == mamon);
    }
}

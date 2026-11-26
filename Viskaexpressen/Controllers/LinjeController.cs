using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Viskaexpressen.Data;

namespace Viskaexpressen.Controllers;

public class LinjeController : Controller
{
    private readonly ApplicationDbContext _db;

    public LinjeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var linjer = await _db.Linjer
            .OrderBy(l => l.Namn)
            .ToListAsync();

        return View(linjer);
    }

    public async Task<IActionResult> Details(int id)
    {
        // Tågsätten hämtas via linjens avgångar. Tidtabellen visas inte här.
        var linje = await _db.Linjer
            .Include(l => l.Avgangar)
            .ThenInclude(a => a.Tagsatt)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (linje == null)
        {
            return NotFound();
        }

        return View(linje);
    }
}

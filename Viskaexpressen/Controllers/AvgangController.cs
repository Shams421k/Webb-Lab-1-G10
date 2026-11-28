using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Viskaexpressen.Data;
using Viskaexpressen.Models;

namespace Viskaexpressen.Controllers;

public class AvgangController : Controller
{
    private readonly ApplicationDbContext _db;

    public AvgangController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(DateTime? datum, int? linjeId)
    {
        var dagar = await _db.Avgangar
            .Select(a => a.Avgangstid.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync();

        var valtDatum = datum?.Date ?? ForstaDatum(dagar);

        string? linjeNamn = null;
        if (linjeId != null)
        {
            linjeNamn = await _db.Linjer
                .Where(l => l.Id == linjeId)
                .Select(l => l.Namn)
                .FirstOrDefaultAsync();

            if (linjeNamn == null)
            {
                return NotFound();
            }
        }

        var fragan = _db.Avgangar
            .Where(a => a.Avgangstid.Date == valtDatum);

        if (linjeId != null)
        {
            fragan = fragan.Where(a => a.LinjeId == linjeId);
        }

        // Lediga platser är tågsättets kapacitet minus bokningarna på just den här avgången.
        var avgangar = await fragan
            .OrderBy(a => a.Avgangstid)
            .Select(a => new AvgangRad
            {
                Id = a.Id,
                Avgangstid = a.Avgangstid,
                LinjeNamn = a.Linje.Namn,
                Tagtyp = a.Linje.Tagtyp,
                TagsattNummer = a.Tagsatt.Nummer,
                RestidMinuter = a.Linje.RestidMinuter,
                Pris = a.Linje.Pris,
                LedigaPlatser = a.Tagsatt.AntalPlatser - a.Bokningar.Count
            })
            .ToListAsync();

        foreach (var avgang in avgangar)
        {
            if (avgang.LedigaPlatser < 0)
            {
                avgang.LedigaPlatser = 0;
            }
        }

        return View(new AvgangIndexViewModel
        {
            ValtDatum = valtDatum,
            Dagar = dagar,
            LinjeId = linjeId,
            LinjeNamn = linjeNamn,
            Avgangar = avgangar
        });
    }

    // Idag om det finns avgångar, annars nästa dag i tidtabellen.
    private static DateTime ForstaDatum(List<DateTime> dagar)
    {
        if (dagar.Count == 0)
        {
            return DateTime.Today;
        }

        var kommande = dagar.FirstOrDefault(d => d >= DateTime.Today);
        return kommande == default ? dagar[^1] : kommande;
    }
}

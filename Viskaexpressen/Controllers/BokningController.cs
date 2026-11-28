using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Viskaexpressen.Data;
using Viskaexpressen.Models;
using Viskaexpressen.Services;

namespace Viskaexpressen.Controllers;

public class BokningController : Controller
{
    private const string PlatsenTagen = "Platsen hann bokas av någon annan, välj en ny.";

    private readonly ApplicationDbContext _db;

    public BokningController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: /Bokning/ValjPlats/5
    public async Task<IActionResult> ValjPlats(int id, int? plats)
    {
        var avgang = await HamtaAvgang(id);
        if (avgang == null)
        {
            return NotFound();
        }

        var upptagna = await _db.Bokningar
            .Where(b => b.AvgangId == id)
            .Select(b => b.PlatsNummer)
            .ToListAsync();

        var model = new ValjPlatsViewModel
        {
            Avgang = avgang,
            UpptagnaPlatser = upptagna.ToHashSet(),
            ValdPlats = plats
        };

        return View(model);
    }

    // GET: /Bokning/Uppgifter?avgangId=5&plats=24
    public async Task<IActionResult> Uppgifter(int avgangId, int? plats)
    {
        var avgang = await HamtaAvgang(avgangId);
        if (avgang == null)
        {
            return NotFound();
        }

        if (plats == null)
        {
            TempData["Fel"] = "Välj en plats i kartan först.";
            return RedirectToAction(nameof(ValjPlats), new { id = avgangId });
        }

        if (plats < 1 || plats > avgang.Tagsatt.AntalPlatser)
        {
            TempData["Fel"] = $"Plats {plats} finns inte i det här tågsättet.";
            return RedirectToAction(nameof(ValjPlats), new { id = avgangId });
        }

        if (await ArUpptagen(avgangId, plats.Value))
        {
            TempData["Fel"] = $"Plats {plats} är redan bokad, välj en annan.";
            return RedirectToAction(nameof(ValjPlats), new { id = avgangId });
        }

        return View(new BokningFormular { AvgangId = avgangId, Plats = plats.Value, Avgang = avgang });
    }

    // POST: /Bokning/Uppgifter
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Uppgifter(BokningFormular formular)
    {
        var avgang = await HamtaAvgang(formular.AvgangId);
        if (avgang == null)
        {
            return NotFound();
        }

        formular.Avgang = avgang;

        if (formular.Plats < 1 || formular.Plats > avgang.Tagsatt.AntalPlatser)
        {
            ModelState.AddModelError(nameof(formular.Plats), "Platsen finns inte i det här tågsättet.");
        }

        if (!ModelState.IsValid)
        {
            return View(formular);
        }

        // Första kontrollen. Den räcker inte ensam eftersom någon kan hinna boka
        // mellan den här raden och SaveChanges, därför finns det unika indexet.
        if (await ArUpptagen(formular.AvgangId, formular.Plats))
        {
            TempData["Fel"] = PlatsenTagen;
            return RedirectToAction(nameof(ValjPlats), new { id = formular.AvgangId });
        }

        // Några försök ifall bokningsnumret mot förmodan redan finns.
        for (var forsok = 0; forsok < 5; forsok++)
        {
            var bokning = new Bokning
            {
                AvgangId = formular.AvgangId,
                PlatsNummer = formular.Plats,
                Bokningsnummer = BokningsnummerGenerator.Skapa(),
                Namn = formular.Namn.Trim(),
                Epost = formular.Epost.Trim()
            };

            _db.Bokningar.Add(bokning);

            try
            {
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Bekraftelse), new { id = bokning.Bokningsnummer });
            }
            catch (DbUpdateException)
            {
                _db.Entry(bokning).State = EntityState.Detached;

                // Det unika indexet på (AvgangId, PlatsNummer) slog till: någon annan var snabbare.
                if (await ArUpptagen(formular.AvgangId, formular.Plats))
                {
                    TempData["Fel"] = PlatsenTagen;
                    return RedirectToAction(nameof(ValjPlats), new { id = formular.AvgangId });
                }

                // Annars var det bokningsnumret som krockade, försök igen med ett nytt.
            }
        }

        ModelState.AddModelError(string.Empty, "Bokningen kunde inte sparas just nu. Försök igen.");
        return View(formular);
    }

    // GET: /Bokning/Bekraftelse/VX-7K3Q92
    public async Task<IActionResult> Bekraftelse(string id)
    {
        var bokning = await HamtaBokning(id);
        if (bokning == null)
        {
            return NotFound();
        }

        return View(bokning);
    }

    // GET: /Bokning/Hitta?nummer=VX-7K3Q92
    public async Task<IActionResult> Hitta(string? nummer)
    {
        ViewData["Nummer"] = nummer;

        if (string.IsNullOrWhiteSpace(nummer))
        {
            return View((Bokning?)null);
        }

        if (nummer.Length > 20)
        {
            ViewData["Fel"] = "Bokningsnumret är för långt. Det ser ut ungefär som VX-7K3Q92.";
            return View((Bokning?)null);
        }

        var normaliserat = BokningsnummerGenerator.Normalisera(nummer);
        var bokning = await HamtaBokning(normaliserat);

        if (bokning == null)
        {
            ViewData["Fel"] = $"Ingen bokning hittades för {normaliserat}. Kontrollera numret och försök igen.";
        }

        return View(bokning);
    }

    private Task<Avgang?> HamtaAvgang(int id)
    {
        return _db.Avgangar
            .Include(a => a.Linje)
            .Include(a => a.Tagsatt)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    private Task<Bokning?> HamtaBokning(string nummer)
    {
        return _db.Bokningar
            .Include(b => b.Avgang).ThenInclude(a => a.Linje)
            .Include(b => b.Avgang).ThenInclude(a => a.Tagsatt)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Bokningsnummer == nummer);
    }

    private Task<bool> ArUpptagen(int avgangId, int plats)
    {
        return _db.Bokningar.AnyAsync(b => b.AvgangId == avgangId && b.PlatsNummer == plats);
    }
}

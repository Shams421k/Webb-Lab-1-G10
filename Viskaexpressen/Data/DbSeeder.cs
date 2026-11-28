using Viskaexpressen.Models;
using Viskaexpressen.Services;

namespace Viskaexpressen.Data;

public static class DbSeeder
{
    private const int AntalDagar = 7;

    private static readonly string[] Fornamn =
    {
        "Anna", "Erik", "Maria", "Johan", "Sara", "Karl", "Emma", "Lars",
        "Elin", "Oskar", "Fatima", "Ali", "Ida", "Mohammed", "Linnea", "Peter"
    };

    private static readonly string[] Efternamn =
    {
        "Andersson", "Johansson", "Karlsson", "Nilsson", "Eriksson", "Larsson",
        "Olsson", "Persson", "Svensson", "Gustafsson", "Hassan", "Lindberg"
    };

    public static void Seed(ApplicationDbContext db)
    {
        // Databasen är redan ifylld.
        if (db.Linjer.Any())
        {
            return;
        }

        var goteborg = new Linje
        {
            Namn = "Borås C → Göteborg C",
            Tagtyp = "Regionaltåg",
            Beskrivning = "Vardagslinje längs Viskadalen med stopp i bland annat Kinna och Skene.",
            RestidMinuter = 65,
            Pris = 95
        };
        var varberg = new Linje
        {
            Namn = "Borås C → Varberg",
            Tagtyp = "Regionaltåg",
            Beskrivning = "Regionaltur mot kusten via Veddige.",
            RestidMinuter = 55,
            Pris = 85
        };
        var ulricehamn = new Linje
        {
            Namn = "Borås C → Ulricehamn",
            Tagtyp = "Regionaltåg",
            Beskrivning = "Kort tur österut mellan Borås och Ulricehamn.",
            RestidMinuter = 28,
            Pris = 49
        };
        var jonkoping = new Linje
        {
            Namn = "Borås C → Jönköping C",
            Tagtyp = "Express",
            Beskrivning = "Expressavgång mot Jönköping med få stopp.",
            RestidMinuter = 75,
            Pris = 139
        };
        var retur = new Linje
        {
            Namn = "Göteborg C → Borås C",
            Tagtyp = "Express",
            Beskrivning = "Direkt express tillbaka mot Borås med färre uppehåll.",
            RestidMinuter = 48,
            Pris = 119
        };

        var litet = new Tagsatt { Nummer = "VE-101", AntalPlatser = 48 };
        var mellan = new Tagsatt { Nummer = "VE-202", AntalPlatser = 96 };
        var stort = new Tagsatt { Nummer = "VE-303", AntalPlatser = 160 };
        var kort = new Tagsatt { Nummer = "VE-404", AntalPlatser = 64 };

        db.Linjer.AddRange(goteborg, varberg, ulricehamn, jonkoping, retur);
        db.Tagsatter.AddRange(litet, mellan, stort, kort);
        db.SaveChanges();

        // Samma tidtabell varje dag, men tågsätten byter plats mellan dagarna.
        var tidtabell = new (Linje Linje, int Timme, int Minut)[]
        {
            (goteborg, 6, 42),
            (goteborg, 7, 15),
            (varberg, 8, 20),
            (ulricehamn, 9, 5),
            (jonkoping, 11, 40),
            (goteborg, 14, 25),
            (retur, 16, 40),
            (varberg, 17, 30),
            (jonkoping, 19, 15)
        };
        var tagsatt = new[] { mellan, kort, litet, stort };

        // Avgångarna räknas från den dag databasen skapas, så tidtabellen är aktuell vid redovisning.
        var idag = DateTime.Today;
        var avgangar = new List<Avgang>();

        for (var dag = 0; dag < AntalDagar; dag++)
        {
            for (var i = 0; i < tidtabell.Length; i++)
            {
                var (linje, timme, minut) = tidtabell[i];
                var satt = tagsatt[(i + dag) % tagsatt.Length];

                avgangar.Add(new Avgang
                {
                    LinjeId = linje.Id,
                    TagsattId = satt.Id,
                    Tagsatt = satt,
                    Avgangstid = idag.AddDays(dag).AddHours(timme).AddMinutes(minut)
                });
            }
        }

        db.Avgangar.AddRange(avgangar);
        db.SaveChanges();

        // Fast frö så att samma platser blir upptagna varje gång databasen skapas.
        var slump = new Random(2026);
        var anvandaNummer = new HashSet<string>();

        // En avgång i morgon med det minsta tågsättet blir helt fullbokad.
        var fullbokad = avgangar.First(a => a.Avgangstid.Date == idag.AddDays(1) && a.Tagsatt == litet);

        foreach (var avgang in avgangar)
        {
            var antalPlatser = avgang.Tagsatt.AntalPlatser;
            var antalBokade = avgang == fullbokad
                ? antalPlatser
                : slump.Next(0, antalPlatser * 6 / 10);

            var platser = Enumerable.Range(1, antalPlatser)
                .OrderBy(_ => slump.Next())
                .Take(antalBokade);

            foreach (var plats in platser)
            {
                var fornamn = Fornamn[slump.Next(Fornamn.Length)];
                var efternamn = Efternamn[slump.Next(Efternamn.Length)];

                string nummer;
                do
                {
                    nummer = BokningsnummerGenerator.Skapa();
                } while (!anvandaNummer.Add(nummer));

                db.Bokningar.Add(new Bokning
                {
                    AvgangId = avgang.Id,
                    PlatsNummer = plats,
                    Bokningsnummer = nummer,
                    Namn = $"{fornamn} {efternamn}",
                    Epost = $"{fornamn}.{efternamn}@exempel.se".ToLowerInvariant()
                });
            }
        }

        db.SaveChanges();
    }
}

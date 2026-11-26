using Viskaexpressen.Models;

namespace Viskaexpressen.Data;

public static class DbSeeder
{
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

        // Avgångarna räknas från den dag databasen fylls, så tidtabellen är aktuell vid redovisning.
        var dag = DateTime.Today;

        db.Avgangar.AddRange(
            Avgang(goteborg, mellan, dag, 7, 15),
            Avgang(goteborg, litet, dag, 9, 5),
            Avgang(varberg, stort, dag, 11, 40),
            Avgang(goteborg, mellan, dag, 16, 40),
            Avgang(ulricehamn, kort, dag.AddDays(1), 8, 20),
            Avgang(jonkoping, mellan, dag.AddDays(1), 12, 10),
            Avgang(retur, litet, dag.AddDays(1), 18, 5),
            Avgang(varberg, kort, dag.AddDays(2), 6, 50),
            Avgang(goteborg, stort, dag.AddDays(2), 14, 25),
            Avgang(ulricehamn, litet, dag.AddDays(2), 19, 15),
            Avgang(jonkoping, stort, dag.AddDays(3), 10, 0),
            Avgang(retur, kort, dag.AddDays(3), 17, 30));

        db.SaveChanges();
    }

    private static Avgang Avgang(Linje linje, Tagsatt tagsatt, DateTime dag, int timme, int minut)
    {
        return new Avgang
        {
            LinjeId = linje.Id,
            TagsattId = tagsatt.Id,
            Avgangstid = dag.AddHours(timme).AddMinutes(minut)
        };
    }
}

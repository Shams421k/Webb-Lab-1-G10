namespace Viskaexpressen.Models;

public class AvgangIndexViewModel
{
    public DateTime ValtDatum { get; set; }

    public List<DateTime> Dagar { get; set; } = new();

    public int? LinjeId { get; set; }

    public string? LinjeNamn { get; set; }

    public List<AvgangRad> Avgangar { get; set; } = new();
}

public class AvgangRad
{
    public int Id { get; set; }

    public DateTime Avgangstid { get; set; }

    public string LinjeNamn { get; set; } = string.Empty;

    public string Tagtyp { get; set; } = string.Empty;

    public string TagsattNummer { get; set; } = string.Empty;

    public int RestidMinuter { get; set; }

    public decimal Pris { get; set; }

    public int LedigaPlatser { get; set; }

    public bool Fullbokad => LedigaPlatser <= 0;
}

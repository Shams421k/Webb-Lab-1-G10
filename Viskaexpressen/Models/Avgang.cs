namespace Viskaexpressen.Models;

public class Avgang
{
    public int Id { get; set; }

    public int LinjeId { get; set; }

    public int TagsattId { get; set; }

    // Datum och avgångstid i samma värde.
    public DateTime Avgangstid { get; set; }

    public Linje Linje { get; set; } = null!;

    public Tagsatt Tagsatt { get; set; } = null!;

    public ICollection<Bokning> Bokningar { get; set; } = new List<Bokning>();
}

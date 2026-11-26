using System.ComponentModel.DataAnnotations;

namespace Viskaexpressen.Models;

public class Linje
{
    public int Id { get; set; }

    // Namn på sträckan, till exempel "Borås C → Göteborg C".
    [Required]
    [StringLength(100)]
    public string Namn { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Tagtyp { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Beskrivning { get; set; } = string.Empty;

    // Restid i minuter.
    [Range(1, 24 * 60)]
    public int RestidMinuter { get; set; }

    [Range(0, 10000)]
    public decimal Pris { get; set; }

    public ICollection<Avgang> Avgangar { get; set; } = new List<Avgang>();
}

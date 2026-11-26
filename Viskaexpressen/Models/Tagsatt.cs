using System.ComponentModel.DataAnnotations;

namespace Viskaexpressen.Models;

public class Tagsatt
{
    public int Id { get; set; }

    // Tågsättets nummer, till exempel "VE-202".
    [Required]
    [StringLength(20)]
    public string Nummer { get; set; } = string.Empty;

    [Range(1, 500)]
    public int AntalPlatser { get; set; }

    public ICollection<Avgang> Avgangar { get; set; } = new List<Avgang>();
}

using System.ComponentModel.DataAnnotations;

namespace Viskaexpressen.Models;

public class Bokning
{
    public int Id { get; set; }

    public int AvgangId { get; set; }

    [Range(1, 500)]
    public int PlatsNummer { get; set; }

    // Kort kod som resenären kan skriva in senare, till exempel "VX-7K3Q92".
    [Required]
    [StringLength(20)]
    public string Bokningsnummer { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Namn { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Epost { get; set; } = string.Empty;

    public Avgang Avgang { get; set; } = null!;
}

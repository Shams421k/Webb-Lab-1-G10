using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Viskaexpressen.Models;

// Det som skickas in från formuläret på sidan Uppgifter.
public class BokningFormular
{
    public int AvgangId { get; set; }

    public int Plats { get; set; }

    [Required(ErrorMessage = "Namn måste anges.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Namnet ska vara mellan 2 och 100 tecken.")]
    [Display(Name = "Namn")]
    public string Namn { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-post måste anges.")]
    [EmailAddress(ErrorMessage = "Ange en giltig e-postadress, t.ex. namn@doman.se.")]
    [StringLength(200, ErrorMessage = "E-postadressen får vara högst 200 tecken.")]
    [Display(Name = "E-post")]
    public string Epost { get; set; } = string.Empty;

    // Fylls i av controllern för sammanfattningen, skickas inte med formuläret.
    [BindNever]
    [ValidateNever]
    public Avgang Avgang { get; set; } = null!;
}

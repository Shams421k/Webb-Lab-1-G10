using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Viskaexpressen.Models;

namespace Viskaexpressen.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    // Tillfälliga sidor så att menyn fungerar tills funktionerna byggs.
    public IActionResult Taglinjer()
    {
        return View();
    }

    public IActionResult Avgangar()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

using System.Security.Cryptography;

namespace Viskaexpressen.Services;

public static class BokningsnummerGenerator
{
    // Utan 0, O, 1 och I så att numret inte går att läsa fel.
    private const string Tecken = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string Skapa()
    {
        return "VX-" + RandomNumberGenerator.GetString(Tecken, 6);
    }

    // Gör om det användaren skrev till samma format som i databasen, t.ex. " vx 7k3q92" -> "VX-7K3Q92".
    public static string Normalisera(string nummer)
    {
        var tecken = new string(nummer.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

        if (tecken.Length == 8 && tecken.StartsWith("VX"))
        {
            tecken = tecken[2..];
        }

        return "VX-" + tecken;
    }
}

namespace Viskaexpressen.Models;

public class ValjPlatsViewModel
{
    public Avgang Avgang { get; set; } = null!;

    public HashSet<int> UpptagnaPlatser { get; set; } = new();

    public int? ValdPlats { get; set; }

    public int LedigaPlatser => Avgang.Tagsatt.AntalPlatser - UpptagnaPlatser.Count;
}

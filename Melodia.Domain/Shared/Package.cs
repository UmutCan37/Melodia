using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Shared;

public sealed record Package
{
    public static readonly Package Basic = new("Basic", 1);
    public static readonly Package Gold = new("Gold", 2);
    public static readonly Package Premium = new("Premium", 3);
    public static readonly Package Elit = new("Elit", 4);

    private static readonly IReadOnlyList<Package> All = [Basic, Gold, Premium, Elit];

    private Package(string name, int level)
    {
        Name = name;
        Level = level;
    }

    public string Name { get; }
    public int Level { get; }

    // Projenin kalbi: bu paket, şarkının istediği paketi karşılıyor mu?
    public bool CanAccess(Package required) => Level >= required.Level;

    // Veritabanından veya JWT'den gelen "Gold" yazısını hazır pakete çevirir
    public static Result<Package> FromName(string name)
    {
        var package = All.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (package is null)
            return Error.Validation("Package.Invalid", $"'{name}' geçerli bir paket değil.");

        return package;
    }
}
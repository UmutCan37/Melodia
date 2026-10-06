using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Artists;

public sealed class Artist : Entity
{
    public const int NameMaxLength = 100;
    public const int BiographyMaxLength = 2000;

    private Artist(Guid id, string name, string? biography) : base(id)
    {
        Name = name;
        Biography = biography;
    }

    // EF Core için
    private Artist() { }

    public string Name { get; private set; } = null!;
    public string? Biography { get; private set; }

    public static Result<Artist> Create(string name, string? biography)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var biographyResult = ValidateBiography(biography);
        if (biographyResult.IsFailure)
            return biographyResult.Error;

        return new Artist(Guid.CreateVersion7(), nameResult.Value, biographyResult.Value);
    }

    public Result UpdateProfile(string name, string? biography)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var biographyResult = ValidateBiography(biography);
        if (biographyResult.IsFailure)
            return biographyResult.Error;

        Name = nameResult.Value;
        Biography = biographyResult.Value;
        return Result.Success();
    }

    private static Result<string> ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ArtistErrors.NameEmpty;

        name = name.Trim();

        if (name.Length > NameMaxLength)
            return ArtistErrors.NameTooLong(NameMaxLength);

        return name;
    }

    private static Result<string?> ValidateBiography(string? biography)
    {
        if (string.IsNullOrWhiteSpace(biography))
            return Result.Success<string?>(null);

        biography = biography.Trim();

        if (biography.Length > BiographyMaxLength)
            return ArtistErrors.BiographyTooLong(BiographyMaxLength);

        return biography;
    }
}
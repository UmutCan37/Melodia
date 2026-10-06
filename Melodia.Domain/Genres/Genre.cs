using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Genres;

public sealed class Genre : Entity
{
    public const int NameMaxLength = 50;

    private Genre(Guid id, string name) : base(id)
    {
        Name = name;
    }

    // EF Core için
    private Genre() { }

    public string Name { get; private set; } = null!;

    public static Result<Genre> Create(string name)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        return new Genre(Guid.CreateVersion7(), nameResult.Value);
    }

    public Result Rename(string name)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        Name = nameResult.Value;
        return Result.Success();
    }

    private static Result<string> ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return GenreErrors.NameEmpty;

        name = name.Trim();

        if (name.Length > NameMaxLength)
            return GenreErrors.NameTooLong(NameMaxLength);

        return name;
    }
}
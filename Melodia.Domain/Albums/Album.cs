using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Albums;

public sealed class Album : Entity
{
    public const int TitleMaxLength = 200;

    private Album(Guid id, string title, Guid artistId, DateOnly releaseDate) : base(id)
    {
        Title = title;
        ArtistId = artistId;
        ReleaseDate = releaseDate;
    }

    // EF Core için
    private Album() { }

    public string Title { get; private set; } = null!;
    public Guid ArtistId { get; private set; }
    public DateOnly ReleaseDate { get; private set; }

    public static Result<Album> Create(string title, Guid artistId, DateOnly releaseDate)
    {
        var titleResult = ValidateTitle(title);
        if (titleResult.IsFailure)
            return titleResult.Error;

        return new Album(Guid.CreateVersion7(), titleResult.Value, artistId, releaseDate);
    }

    public Result UpdateDetails(string title, DateOnly releaseDate)
    {
        var titleResult = ValidateTitle(title);
        if (titleResult.IsFailure)
            return titleResult.Error;

        Title = titleResult.Value;
        ReleaseDate = releaseDate;
        return Result.Success();
    }

    private static Result<string> ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return AlbumErrors.TitleEmpty;

        title = title.Trim();

        if (title.Length > TitleMaxLength)
            return AlbumErrors.TitleTooLong(TitleMaxLength);

        return title;
    }
}
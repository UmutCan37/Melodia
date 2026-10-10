using Melodia.Domain.Abstractions;
using Melodia.Domain.Albums;
using Melodia.Domain.Shared;

namespace Melodia.Domain.Songs;

public sealed class Song : Entity
{
    public const int TitleMaxLength = 200;

    private Song(
        Guid id,
        string title,
        Duration duration,
        Guid artistId,
        Guid genreId,
        AudioFile audioFile,
        Package requiredPackage) : base(id)
    {
        Title = title;
        Duration = duration;
        ArtistId = artistId;
        GenreId = genreId;
        AudioFile = audioFile;
        RequiredPackage = requiredPackage;
    }

    // EF Core için
    private Song() { }

    public string Title { get; private set; } = null!;
    public Duration Duration { get; private set; } = null!;
    public Guid ArtistId { get; private set; }
    public Guid GenreId { get; private set; }
    public Guid? AlbumId { get; private set; }
    public AudioFile AudioFile { get; private set; } = null!;
    public Package RequiredPackage { get; private set; } = null!;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public static Result<Song> Create(
        string title,
        Duration duration,
        Guid artistId,
        Guid genreId,
        AudioFile audioFile,
        Package requiredPackage)
    {
        var titleResult = ValidateTitle(title);
        if (titleResult.IsFailure)
            return titleResult.Error;

        return new Song(
            Guid.CreateVersion7(),
            titleResult.Value,
            duration,
            artistId,
            genreId,
            audioFile,
            requiredPackage);
    }

    public Result Rename(string title)
    {
        var titleResult = ValidateTitle(title);
        if (titleResult.IsFailure)
            return titleResult.Error;

        Title = titleResult.Value;
        return Result.Success();
    }

    public void ChangeRequiredPackage(Package requiredPackage) => RequiredPackage = requiredPackage;

    // Şarkı sadece kendi sanatçısının albümüne eklenebilir
    public Result AssignToAlbum(Album album)
    {
        if (album.ArtistId != ArtistId)
            return AlbumErrors.ArtistMismatch;

        AlbumId = album.Id;
        return Result.Success();
    }

    public void RemoveFromAlbum() => AlbumId = null;

    public void Delete(DateTime deletedAtUtc)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAtUtc = deletedAtUtc;
    }
    private static Result<string> ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return SongErrors.TitleEmpty;

        title = title.Trim();

        if (title.Length > TitleMaxLength)
            return SongErrors.TitleTooLong(TitleMaxLength);

        return title;
    }
}
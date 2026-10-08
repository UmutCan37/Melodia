using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Songs.CreateSong;

public sealed record CreateSongCommand(
    string Title,
    int DurationSeconds,
    Guid ArtistId,
    Guid GenreId,
    Guid? AlbumId,
    string RequiredPackage,
    Stream AudioContent,
    string AudioFileName) : ICommand<Guid>;
namespace Melodia.Application.Features.Songs.PlaySong;

public sealed record PlaySongResponse(Guid SongId, string StreamUrl);
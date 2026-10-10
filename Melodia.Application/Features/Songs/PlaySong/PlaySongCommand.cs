using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Songs.PlaySong;

public sealed record PlaySongCommand(Guid SongId) : ICommand<PlaySongResponse>;
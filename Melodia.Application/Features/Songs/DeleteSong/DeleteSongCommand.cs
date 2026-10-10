using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Songs.DeleteSong;

public sealed record DeleteSongCommand(Guid SongId) : ICommand;
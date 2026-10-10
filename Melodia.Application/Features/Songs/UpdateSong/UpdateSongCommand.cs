using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Songs.UpdateSong;

public sealed record UpdateSongCommand(Guid SongId, string Title, string RequiredPackage) : ICommand;
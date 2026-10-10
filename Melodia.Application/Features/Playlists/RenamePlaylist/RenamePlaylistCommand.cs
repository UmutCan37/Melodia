using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Playlists.RenamePlaylist;

public sealed record RenamePlaylistCommand(Guid PlaylistId, string Name) : ICommand;
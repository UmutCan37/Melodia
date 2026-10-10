using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Playlists.DeletePlaylist;

public sealed record DeletePlaylistCommand(Guid PlaylistId) : ICommand;
using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Playlists.CreatePlaylist;

public sealed record CreatePlaylistCommand(string Name) : ICommand<Guid>;
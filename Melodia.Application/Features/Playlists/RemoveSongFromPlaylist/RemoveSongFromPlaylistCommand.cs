using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Playlists.RemoveSongFromPlaylist;

public sealed record RemoveSongFromPlaylistCommand(Guid PlaylistId, Guid SongId) : ICommand;
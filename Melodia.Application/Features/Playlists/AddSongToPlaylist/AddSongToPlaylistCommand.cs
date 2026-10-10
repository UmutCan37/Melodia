using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Playlists.AddSongToPlaylist;

public sealed record AddSongToPlaylistCommand(Guid PlaylistId, Guid SongId) : ICommand;
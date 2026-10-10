using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Playlists.GetMyPlaylists;

public sealed record GetMyPlaylistsQuery : IQuery<IReadOnlyList<PlaylistSummaryResponse>>;
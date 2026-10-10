using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Playlists.GetPlaylistById;

public sealed record GetPlaylistByIdQuery(Guid PlaylistId) : IQuery<PlaylistResponse>;
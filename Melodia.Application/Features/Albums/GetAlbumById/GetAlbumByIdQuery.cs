using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Albums.GetAlbumById;

public sealed record GetAlbumByIdQuery(Guid AlbumId) : IQuery<AlbumResponse>;
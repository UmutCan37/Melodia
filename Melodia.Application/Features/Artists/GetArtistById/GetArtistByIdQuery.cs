using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Artists.GetArtistById;

public sealed record GetArtistByIdQuery(Guid ArtistId) : IQuery<ArtistResponse>;
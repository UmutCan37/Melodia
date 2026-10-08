namespace Melodia.Application.Features.Artists.GetArtistById;

public sealed record ArtistResponse(Guid Id, string Name, string? Biography);
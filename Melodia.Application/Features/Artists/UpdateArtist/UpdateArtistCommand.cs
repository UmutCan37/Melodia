using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Artists.UpdateArtist;

public sealed record UpdateArtistCommand(Guid ArtistId, string Name, string? Biography) : ICommand;
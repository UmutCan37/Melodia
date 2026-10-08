using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Artists.DeleteArtist;

public sealed record DeleteArtistCommand(Guid ArtistId) : ICommand;
using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Artists.CreateArtist;

public sealed record CreateArtistCommand(string Name, string? Biography) : ICommand<Guid>;
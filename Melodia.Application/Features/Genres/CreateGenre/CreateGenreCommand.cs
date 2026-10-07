using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Genres.CreateGenre;

public sealed record CreateGenreCommand(string Name) : ICommand<Guid>;
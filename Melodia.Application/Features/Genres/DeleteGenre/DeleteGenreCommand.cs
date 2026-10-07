using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Genres.DeleteGenre;

public sealed record DeleteGenreCommand(Guid GenreId) : ICommand;
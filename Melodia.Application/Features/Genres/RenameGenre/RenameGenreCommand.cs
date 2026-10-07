using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Genres.RenameGenre;

public sealed record RenameGenreCommand(Guid GenreId, string Name) : ICommand;
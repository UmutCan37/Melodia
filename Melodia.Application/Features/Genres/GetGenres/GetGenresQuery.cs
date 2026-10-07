using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Genres.GetGenres;

public sealed record GetGenresQuery : IQuery<IReadOnlyList<GenreResponse>>;
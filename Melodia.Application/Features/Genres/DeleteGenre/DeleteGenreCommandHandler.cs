using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Genres;
using Melodia.Domain.Songs;

namespace Melodia.Application.Features.Genres.DeleteGenre;

internal sealed class DeleteGenreCommandHandler(
    IGenreRepository genreRepository,
    ISongRepository songRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteGenreCommand>
{
    public async Task<Result> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.GetByIdAsync(request.GenreId, cancellationToken);
        if (genre is null)
            return GenreErrors.NotFound(request.GenreId);

        if (await songRepository.ExistsByGenreAsync(genre.Id, cancellationToken))
            return GenreErrors.HasSongs;

        genreRepository.Remove(genre);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
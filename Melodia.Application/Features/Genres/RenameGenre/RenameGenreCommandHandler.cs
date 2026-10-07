using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Genres;

namespace Melodia.Application.Features.Genres.RenameGenre;

internal sealed class RenameGenreCommandHandler(
    IGenreRepository genreRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RenameGenreCommand>
{
    public async Task<Result> Handle(RenameGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.GetByIdAsync(request.GenreId, cancellationToken);
        if (genre is null)
            return GenreErrors.NotFound(request.GenreId);

        var newName = request.Name.Trim();

        // Kendi adını tekrar göndermek çakışma sayılmaz
        if (!string.Equals(genre.Name, newName, StringComparison.OrdinalIgnoreCase) &&
            await genreRepository.IsNameInUseAsync(newName, cancellationToken))
        {
            return GenreErrors.NameAlreadyExists;
        }

        var renameResult = genre.Rename(newName);
        if (renameResult.IsFailure)
            return renameResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
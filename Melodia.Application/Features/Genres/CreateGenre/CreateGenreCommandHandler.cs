using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Genres;

namespace Melodia.Application.Features.Genres.CreateGenre;

internal sealed class CreateGenreCommandHandler(
    IGenreRepository genreRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateGenreCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        if (await genreRepository.IsNameInUseAsync(request.Name.Trim(), cancellationToken))
            return GenreErrors.NameAlreadyExists;

        var genreResult = Genre.Create(request.Name);
        if (genreResult.IsFailure)
            return genreResult.Error;

        genreRepository.Add(genreResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return genreResult.Value.Id;
    }
}
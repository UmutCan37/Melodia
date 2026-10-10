using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Melodia.Domain.Shared;

namespace Melodia.Application.Features.Listeners.ChangePackage;

internal sealed class ChangePackageCommandHandler(
    IUserContext userContext,
    IListenerRepository listenerRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ChangePackageCommand>
{
    public async Task<Result> Handle(ChangePackageCommand request, CancellationToken cancellationToken)
    {
        var packageResult = Package.FromName(request.Package);
        if (packageResult.IsFailure)
            return packageResult.Error;

        var listener = await listenerRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (listener is null)
            return ListenerErrors.NotFound(userContext.UserId);

        // Aynı pakete geçiş → 409; başarılıysa PackageChangedEvent bırakılır
        var changeResult = listener.ChangePackage(packageResult.Value);
        if (changeResult.IsFailure)
            return changeResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
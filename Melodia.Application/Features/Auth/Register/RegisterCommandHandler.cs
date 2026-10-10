using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Melodia.Domain.Shared;

namespace Melodia.Application.Features.Auth.Register;

internal sealed class RegisterCommandHandler(
    IIdentityService identityService,
    IListenerRepository listenerRepository,
    IDatabaseTransactionManager transactionManager,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<RegisterCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var email = emailResult.Value;

        if (await listenerRepository.IsEmailInUseAsync(email, cancellationToken))
            return ListenerErrors.EmailAlreadyInUse;

        // Commit edilmeden çıkılırsa (hata dönüşü ya da exception) her şey otomatik geri alınır
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);

        // 1. Identity: kullanıcı + şifre
        var userIdResult = await identityService.RegisterAsync(email, request.Password, cancellationToken);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        // 2. Domain: aynı Id ile Listener
        var listenerResult = Listener.Register(
            userIdResult.Value,
            email,
            request.DisplayName,
            timeProvider.GetUtcNow().UtcDateTime);

        if (listenerResult.IsFailure)
            return listenerResult.Error;

        listenerRepository.Add(listenerResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 3. İkisi de başarılı: kalıcı hâle getir
        await transaction.CommitAsync(cancellationToken);

        return listenerResult.Value.Id;
    }
}
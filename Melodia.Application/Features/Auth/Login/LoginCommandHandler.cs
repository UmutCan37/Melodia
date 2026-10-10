using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Melodia.Domain.Shared;

namespace Melodia.Application.Features.Auth.Login;

internal sealed class LoginCommandHandler(
    IIdentityService identityService,
    IListenerRepository listenerRepository,
    IJwtProvider jwtProvider) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Geçersiz format da "hatalı giriş" sayılır; ayrı bir mesaj bilgi sızdırır
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return AuthErrors.InvalidCredentials;

        var email = emailResult.Value;

        // 1. Şifre doğru mu? (Identity)
        var userIdResult = await identityService.ValidateCredentialsAsync(email, request.Password, cancellationToken);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        var userId = userIdResult.Value;

        // 2. Token'a yazılacak paket (Domain)
        var listener = await listenerRepository.GetByIdAsync(userId, cancellationToken);
        if (listener is null)
            return AuthErrors.InvalidCredentials;

        // 3. Token'a yazılacak roller (Identity)
        var roles = await identityService.GetRolesAsync(userId, cancellationToken);

        // 4. Token'ları üret
        var accessToken = jwtProvider.CreateAccessToken(userId, email.Value, roles, listener.Package);
        var refreshToken = await identityService.IssueRefreshTokenAsync(userId, cancellationToken);

        return new LoginResponse(accessToken, refreshToken);
    }
}
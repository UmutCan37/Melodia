using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;

namespace Melodia.Application.Features.Auth.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IIdentityService identityService,
    IListenerRepository listenerRepository,
    IJwtProvider jwtProvider) : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Eski token'ı doğrula, geçersiz kıl, yenisini üret
        var rotationResult = await identityService.RotateRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (rotationResult.IsFailure)
            return rotationResult.Error;

        var rotation = rotationResult.Value;

        // 2. Güncel paket ve e-posta
        var listener = await listenerRepository.GetByIdAsync(rotation.UserId, cancellationToken);
        if (listener is null)
            return AuthErrors.InvalidRefreshToken;

        // 3. Güncel roller
        var roles = await identityService.GetRolesAsync(rotation.UserId, cancellationToken);

        // 4. Yeni access token
        var accessToken = jwtProvider.CreateAccessToken(
            rotation.UserId,
            listener.Email.Value,
            roles,
            listener.Package);

        return new RefreshTokenResponse(accessToken, rotation.NewRefreshToken);
    }
}
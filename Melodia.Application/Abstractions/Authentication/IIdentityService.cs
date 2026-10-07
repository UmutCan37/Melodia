using Melodia.Domain.Abstractions;
using Melodia.Domain.Shared;

namespace Melodia.Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(Email email, string password, CancellationToken cancellationToken = default);

    Task<Result<Guid>> ValidateCredentialsAsync(Email email, string password, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<string> IssueRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<RefreshTokenRotation>> RotateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
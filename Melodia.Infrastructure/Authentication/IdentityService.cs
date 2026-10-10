using System.Security.Cryptography;
using System.Text;
using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Features.Auth;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Melodia.Domain.Shared;
using Melodia.Persistence;
using Melodia.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Melodia.Infrastructure.Authentication;

internal sealed class IdentityService(
    UserManager<AppUser> userManager,
    AppDbContext context,
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider) : IIdentityService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<Guid>> RegisterAsync(Email email, string password, CancellationToken cancellationToken = default)
    {
        var user = new AppUser
        {
            Id = Guid.CreateVersion7(),
            UserName = email.Value,
            Email = email.Value
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            return user.Id;

        if (result.Errors.Any(e => e.Code is "DuplicateEmail" or "DuplicateUserName"))
            return ListenerErrors.EmailAlreadyInUse;

        var description = string.Join(" ", result.Errors.Select(e => e.Description));
        return Error.Validation("Auth.RegistrationFailed", description);
    }

    public async Task<Result<Guid>> ValidateCredentialsAsync(Email email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email.Value);
        if (user is null)
            return AuthErrors.InvalidCredentials;

        // Çok sayıda hatalı denemeden sonra hesap geçici olarak kilitlenir (brute force koruması)
        if (await userManager.IsLockedOutAsync(user))
            return AuthErrors.InvalidCredentials;

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            await userManager.AccessFailedAsync(user);
            return AuthErrors.InvalidCredentials;
        }

        await userManager.ResetAccessFailedCountAsync(user);
        return user.Id;
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return [];

        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<string> IssueRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var (token, entity) = CreateRefreshToken(userId);

        context.RefreshTokens.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return token;
    }

    public async Task<Result<RefreshTokenRotation>> RotateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return AuthErrors.InvalidRefreshToken;

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var hash = Hash(refreshToken);

        var existing = await context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);
        if (existing is null)
            return AuthErrors.InvalidRefreshToken;

        // Daha önce kullanılmış bir token tekrar geldi: çalınmış olabilir, kullanıcının bütün oturumlarını kapat
        if (existing.RevokedAtUtc is not null)
        {
            await RevokeAllAsync(existing.UserId, now, cancellationToken);
            return AuthErrors.InvalidRefreshToken;
        }

        if (!existing.IsActive(now))
            return AuthErrors.InvalidRefreshToken;

        existing.Revoke(now);

        var (newToken, newEntity) = CreateRefreshToken(existing.UserId);
        context.RefreshTokens.Add(newEntity);

        // Eskiyi iptal + yenisini ekle: tek SaveChanges, tek transaction
        await context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenRotation(existing.UserId, newToken);
    }

    private (string Token, RefreshToken Entity) CreateRefreshToken(Guid userId)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var entity = new RefreshToken
        {
            UserId = userId,
            TokenHash = Hash(token),
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(_jwtOptions.RefreshTokenDays)
        };

        return (token, entity);
    }

    private async Task RevokeAllAsync(Guid userId, DateTime now, CancellationToken cancellationToken)
    {
        var activeTokens = await context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
            token.Revoke(now);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
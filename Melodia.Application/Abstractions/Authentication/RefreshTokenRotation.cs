namespace Melodia.Application.Abstractions.Authentication;

public sealed record RefreshTokenRotation(Guid UserId, string NewRefreshToken);
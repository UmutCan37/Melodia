namespace Melodia.Application.Features.Listeners.GetMyProfile;

public sealed record ProfileResponse(
    Guid Id,
    string Email,
    string DisplayName,
    string Package,
    DateTime RegisteredAtUtc);
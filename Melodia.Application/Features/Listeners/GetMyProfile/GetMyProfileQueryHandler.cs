using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Data;
using Melodia.Application.Abstractions.Messaging;
using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Application.Features.Listeners.GetMyProfile;

internal sealed class GetMyProfileQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext) : IQueryHandler<GetMyProfileQuery, ProfileResponse>
{
    public async Task<Result<ProfileResponse>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;

        var profile = await context.Listeners
            .AsNoTracking()
            .Where(l => l.Id == userId)
            .Select(l => new ProfileResponse(
                l.Id,
                l.Email.Value,
                l.DisplayName,
                l.Package.Name,
                l.RegisteredAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (profile is null)
            return ListenerErrors.NotFound(userId);

        return profile;
    }
}
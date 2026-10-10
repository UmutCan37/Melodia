using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Listeners.GetMyProfile;

public sealed record GetMyProfileQuery : IQuery<ProfileResponse>;
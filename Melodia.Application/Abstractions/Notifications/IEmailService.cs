using Melodia.Domain.Shared;

namespace Melodia.Application.Abstractions.Notifications;

public interface IEmailService
{
    Task SendAsync(Email to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
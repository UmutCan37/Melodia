using MediatR;
using Melodia.Application.Abstractions.Notifications;
using Melodia.Domain.Listeners;
using Melodia.Domain.Listeners.Events;
using Microsoft.Extensions.Logging;

namespace Melodia.Application.Features.Listeners.EventHandlers;

internal sealed class PackageChangedEventHandler(
    IListenerRepository listenerRepository,
    IEmailService emailService,
    ILogger<PackageChangedEventHandler> logger) : INotificationHandler<PackageChangedEvent>
{
    public async Task Handle(PackageChangedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Paket değişti. ListenerId: {ListenerId} | {OldPackage} → {NewPackage}",
            notification.ListenerId, notification.OldPackage.Name, notification.NewPackage.Name);

        var listener = await listenerRepository.GetByIdAsync(notification.ListenerId, cancellationToken);
        if (listener is null)
            return;

        var isUpgrade = notification.NewPackage.Level > notification.OldPackage.Level;

        var subject = isUpgrade
            ? $"{notification.NewPackage.Name} paketine hoş geldin!"
            : "Paketin değiştirildi";

        var body = isUpgrade
            ? $"<p>Merhaba {listener.DisplayName},</p><p>Paketin {notification.OldPackage.Name} → {notification.NewPackage.Name} olarak yükseltildi. Artık daha fazla şarkıya erişebilirsin.</p>"
            : $"<p>Merhaba {listener.DisplayName},</p><p>Paketin {notification.OldPackage.Name} → {notification.NewPackage.Name} olarak değiştirildi.</p>";

        try
        {
            await emailService.SendAsync(listener.Email, subject, body, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Paket değişikliği e-postası gönderilemedi. ListenerId: {ListenerId}", notification.ListenerId);
        }
    }
}
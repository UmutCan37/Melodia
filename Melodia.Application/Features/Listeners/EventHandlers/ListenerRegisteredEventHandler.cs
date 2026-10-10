using MediatR;
using Melodia.Application.Abstractions.Notifications;
using Melodia.Domain.Listeners;
using Melodia.Domain.Listeners.Events;
using Microsoft.Extensions.Logging;

namespace Melodia.Application.Features.Listeners.EventHandlers;

internal sealed class ListenerRegisteredEventHandler(
    IListenerRepository listenerRepository,
    IEmailService emailService,
    ILogger<ListenerRegisteredEventHandler> logger) : INotificationHandler<ListenerRegisteredEvent>
{
    public async Task Handle(ListenerRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var listener = await listenerRepository.GetByIdAsync(notification.ListenerId, cancellationToken);
        if (listener is null)
            return;

        try
        {
            await emailService.SendAsync(
                listener.Email,
                "Melodia'ya hoş geldin!",
                $"<p>Merhaba {listener.DisplayName},</p><p>Hesabın oluşturuldu. Basic paketle dinlemeye hemen başlayabilirsin.</p>",
                cancellationToken);
        }
        catch (Exception ex)
        {
            // E-posta gönderilemedi diye kayıt başarısız olmasın
            logger.LogError(ex, "Hoş geldin e-postası gönderilemedi. ListenerId: {ListenerId}", notification.ListenerId);
        }
    }
}
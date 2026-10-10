using MailKit.Net.Smtp;
using MailKit.Security;
using Melodia.Application.Abstractions.Notifications;
using Melodia.Domain.Shared;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Melodia.Infrastructure.Notifications;

internal sealed class EmailService(IOptions<EmailOptions> options) : IEmailService
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(Email to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        message.To.Add(MailboxAddress.Parse(to.Value));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();

        
        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.Auto, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.Username))
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken);
    }
}
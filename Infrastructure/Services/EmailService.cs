using Application.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Preparing to send email to {Recipient} with subject '{Subject}'", to, subject);
        var message = CreateMessage(to, subject, htmlBody);
        await SendAsync(message, cancellationToken);
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string htmlBody, string attachmentName, byte[] attachmentData, string contentType, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Preparing to send email with attachment '{Attachment}' ({Size} bytes) to {Recipient}", attachmentName, attachmentData.Length, to);
        var message = CreateMessage(to, subject, htmlBody);

        var body = message.Body;
        var multipart = new Multipart("mixed");
        multipart.Add(body);

        var attachment = new MimePart(contentType)
        {
            Content = new MimeContent(new MemoryStream(attachmentData)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = attachmentName
        };
        multipart.Add(attachment);

        message.Body = multipart;
        await SendAsync(message, cancellationToken);
    }

    private MimeMessage CreateMessage(string to, string subject, string htmlBody)
    {
        var senderName = _configuration["EmailSettings:SenderName"];
        var senderEmail = _configuration["EmailSettings:SenderEmail"];

        if (string.IsNullOrWhiteSpace(senderEmail))
        {
            _logger.LogError("EmailSettings:SenderEmail is not configured");
            throw new InvalidOperationException("Email sender is not configured. Set EmailSettings__SenderEmail environment variable or check .env file.");
        }

        _logger.LogDebug("Building email message from {Sender} to {Recipient}", senderEmail, to);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };
        return message;
    }

    private async Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var portString = _configuration["EmailSettings:Port"];
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];

        if (string.IsNullOrWhiteSpace(smtpServer) || string.IsNullOrWhiteSpace(portString))
        {
            _logger.LogError("SMTP server or port is not configured. SmtpServer='{SmtpServer}', Port='{Port}'", smtpServer, portString);
            throw new InvalidOperationException("SMTP server settings are not configured. Set EmailSettings__SmtpServer and EmailSettings__Port environment variables or check .env file.");
        }

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogError("SMTP credentials are not configured");
            throw new InvalidOperationException("SMTP credentials are not configured. Set EmailSettings__Username and EmailSettings__Password environment variables or check .env file.");
        }

        var port = int.Parse(portString);
        var recipient = message.To.ToString();

        using var client = new SmtpClient();
        client.Timeout = 30000;
        try
        {
            _logger.LogDebug("Connecting to SMTP server {SmtpServer}:{Port}", smtpServer, port);
            await client.ConnectAsync(smtpServer, port, SecureSocketOptions.Auto, cancellationToken);
            _logger.LogDebug("Connected to SMTP server successfully");

            _logger.LogDebug("Authenticating with SMTP server as {Username}", username);
            await client.AuthenticateAsync(username, password, cancellationToken);
            _logger.LogDebug("SMTP authentication successful");

            await client.SendAsync(message, cancellationToken);
            _logger.LogInformation("Email sent successfully to {Recipient}", recipient);

            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (AuthenticationException ex)
        {
            _logger.LogError(ex, "SMTP authentication failed for user {Username} on {SmtpServer}:{Port}", username, smtpServer, port);
            throw;
        }
        catch (SslHandshakeException ex)
        {
            _logger.LogError(ex, "SSL/TLS handshake failed with {SmtpServer}:{Port}", smtpServer, port);
            throw;
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "SMTP command error while sending to {Recipient}. StatusCode={StatusCode}", recipient, ex.StatusCode);
            throw;
        }
        catch (SmtpProtocolException ex)
        {
            _logger.LogError(ex, "SMTP protocol error while communicating with {SmtpServer}:{Port}", smtpServer, port);
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Email send to {Recipient} was cancelled", recipient);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email to {Recipient}", recipient);
            throw;
        }
    }
}

namespace Application.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
    Task SendEmailWithAttachmentAsync(string to, string subject, string htmlBody, string attachmentName, byte[] attachmentData, string contentType, CancellationToken cancellationToken = default);
}

using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(HttpClient httpClient, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Preparing to send email to {Recipient} with subject '{Subject}'", to, subject);

        var payload = BuildPayload(to, subject, htmlBody);
        await SendViaBrevoAsync(payload, to, cancellationToken);
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string htmlBody, string attachmentName, byte[] attachmentData, string contentType, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Preparing to send email with attachment '{Attachment}' ({Size} bytes) to {Recipient}", attachmentName, attachmentData.Length, to);

        var payload = BuildPayload(to, subject, htmlBody);
        payload.Attachment =
        [
            new BrevoAttachment
            {
                Name = attachmentName,
                Content = Convert.ToBase64String(attachmentData)
            }
        ];

        await SendViaBrevoAsync(payload, to, cancellationToken);
    }

    private BrevoEmailPayload BuildPayload(string to, string subject, string htmlBody)
    {
        var senderName = _configuration["EmailSettings:SenderName"];
        var senderEmail = _configuration["EmailSettings:SenderEmail"];

        if (string.IsNullOrWhiteSpace(senderEmail))
        {
            _logger.LogError("EmailSettings:SenderEmail is not configured");
            throw new InvalidOperationException("Email sender is not configured. Set EmailSettings__SenderEmail environment variable.");
        }

        return new BrevoEmailPayload
        {
            Sender = new BrevoContact { Name = senderName ?? "Gatepass System", Email = senderEmail },
            To = [new BrevoContact { Email = to }],
            Subject = subject,
            HtmlContent = htmlBody
        };
    }

    private async Task SendViaBrevoAsync(BrevoEmailPayload payload, string recipient, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["EmailSettings:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogError("EmailSettings:ApiKey is not configured");
            throw new InvalidOperationException("Brevo API key is not configured. Set EmailSettings__ApiKey environment variable.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
        request.Headers.Add("api-key", apiKey);
        request.Content = JsonContent.Create(payload);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email sent successfully to {Recipient} via Brevo", recipient);
        }
        else
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Brevo API returned {StatusCode} for {Recipient}: {Body}", (int)response.StatusCode, recipient, body);
            throw new InvalidOperationException($"Brevo API error ({(int)response.StatusCode}): {body}");
        }
    }

    private sealed class BrevoEmailPayload
    {
        [JsonPropertyName("sender")]
        public BrevoContact Sender { get; set; } = new();

        [JsonPropertyName("to")]
        public List<BrevoContact> To { get; set; } = [];

        [JsonPropertyName("subject")]
        public string Subject { get; set; } = string.Empty;

        [JsonPropertyName("htmlContent")]
        public string HtmlContent { get; set; } = string.Empty;

        [JsonPropertyName("attachment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BrevoAttachment>? Attachment { get; set; }
    }

    private sealed class BrevoContact
    {
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }

    private sealed class BrevoAttachment
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}

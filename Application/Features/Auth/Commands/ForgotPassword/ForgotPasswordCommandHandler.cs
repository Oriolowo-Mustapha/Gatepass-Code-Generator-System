using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService, ILogger<ForgotPasswordCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Forgot password request received for {Email}", request.Email);

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Forgot password requested for non-existent email {Email}", request.Email);
            return ApiResponse<bool>.Success(true, "If the email exists, a reset link has been sent.");
        }

        var tokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        var resetToken = Convert.ToBase64String(tokenBytes);

        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("Password reset token generated for user {UserId}, expires at {Expiry}", user.Id, user.PasswordResetTokenExpiry);

        var emailBody = $"""
            <h2>Password Reset Request</h2>
            <p>Dear {user.FirstName},</p>
            <p>You have requested to reset your password. Use the token below to reset it:</p>
            <p><strong>{resetToken}</strong></p>
            <p>This token is valid for 1 hour.</p>
            <p>If you did not request a password reset, please ignore this email.</p>
            <br/>
            <p>Regards,<br/>Gatepass Code Generator System</p>
            """;

        try
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Password Reset Request",
                emailBody,
                cancellationToken);

            _logger.LogInformation("Password reset email sent successfully to {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}. Token was saved but email delivery failed.", user.Email);
        }

        return ApiResponse<bool>.Success(true, "If the email exists, a reset link has been sent.");
    }
}

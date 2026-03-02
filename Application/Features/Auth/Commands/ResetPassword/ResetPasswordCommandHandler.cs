using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ILogger<ResetPasswordCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password reset attempt for {Email}", request.Email);

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Password reset attempted for non-existent email {Email}", request.Email);
            return ApiResponse<bool>.Failure("Invalid reset request.");
        }

        if (user.PasswordResetToken is null ||
            user.PasswordResetToken != request.Token ||
            user.PasswordResetTokenExpiry is null ||
            user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            _logger.LogWarning("Invalid or expired reset token used for user {UserId}", user.Id);
            return ApiResponse<bool>.Failure("Invalid or expired reset token.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);
        return ApiResponse<bool>.Success(true, "Password has been reset successfully.");
    }
}

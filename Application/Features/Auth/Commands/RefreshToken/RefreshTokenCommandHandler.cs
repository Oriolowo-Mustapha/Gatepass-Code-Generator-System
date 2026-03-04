using Application.DTOS;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, ILogger<RefreshTokenCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _tokenService.GetUserIdFromExpiredToken(request.JwtToken);
        if (userId is null)
        {
            throw new UnauthorizedException("Invalid token");
        }

        var user = await _unitOfWork.Users.GetByIdWithRoleAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedException("Invalid token");
        }

        if (!string.Equals(
                user.RefreshToken,
                request.RefreshToken?.Trim()?.Replace(' ', '+'),
                StringComparison.Ordinal))
        {
            _logger.LogWarning("Refresh token mismatch for user {UserId}. DB token length: {DbLen}, Request token length: {ReqLen}",
                user.Id, user.RefreshToken?.Length, request.RefreshToken?.Length);
            throw new UnauthorizedException("Invalid or expired refresh token");
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token expired for user {UserId}. Expiry: {Expiry}, Now: {Now}",
                user.Id, user.RefreshTokenExpiryTime, DateTime.UtcNow);
            throw new UnauthorizedException("Invalid or expired refresh token");
        }

        var newJwtToken = _tokenService.GenerateJwtToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken,
            UserName = user.UserName,
            Email = user.Email,
            RoleName = user.Role?.RoleName ?? string.Empty
        };

        return ApiResponse<AuthResponseDto>.Success(response, "Token Refreshed Successfully");
    }
}

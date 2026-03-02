using Application.DTOS;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for {Email}", request.Email);

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Login failed: email {Email} not found", request.Email);
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: invalid password for user {UserId}", user.Id);
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed: account disabled for user {UserId}", user.Id);
            throw new UnauthorizedException("Account is disabled");
        }

        var jwtToken = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.LastLoginDate = DateTime.UtcNow;
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Login successful for user {UserId} ({Email})", user.Id, user.Email);

        var response = new AuthResponseDto
        {
            Token = jwtToken,
            RefreshToken = refreshToken,
            UserName = user.UserName,
            Email = user.Email,
            RoleName = user.Role?.RoleName ?? string.Empty
        };

        return ApiResponse<AuthResponseDto>.Success(response, "Login Successful");
    }
}

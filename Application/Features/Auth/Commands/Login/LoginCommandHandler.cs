using Application.DTOS;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("Account is disabled");
        }

        var jwtToken = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.LastLoginDate = DateTime.UtcNow;
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

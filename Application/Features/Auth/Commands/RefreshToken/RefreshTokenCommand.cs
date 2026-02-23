using Application.DTOS;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string JwtToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}

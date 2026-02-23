using Application.DTOS;
using MediatR;

namespace Application.Features.Auth.Commands.Login;

public record LoginCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

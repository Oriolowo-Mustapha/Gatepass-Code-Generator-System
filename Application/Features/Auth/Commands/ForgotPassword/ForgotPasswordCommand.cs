using Application.DTOS;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand : IRequest<ApiResponse<bool>>
{
    public string Email { get; init; } = string.Empty;
}

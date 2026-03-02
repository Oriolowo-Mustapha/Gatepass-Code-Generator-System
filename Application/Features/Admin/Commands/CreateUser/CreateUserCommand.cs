using Application.DTOS;
using MediatR;

namespace Application.Features.Admin.Commands.CreateUser;

public record CreateUserCommand : IRequest<ApiResponse<Guid>>
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
    public string? DepartmentCode { get; init; }
}

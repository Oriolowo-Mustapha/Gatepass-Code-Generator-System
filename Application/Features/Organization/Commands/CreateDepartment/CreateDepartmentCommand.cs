using Application.DTOS;
using MediatR;

namespace Application.Features.Organization.Commands.CreateDepartment;

public record CreateDepartmentCommand : IRequest<ApiResponse<Guid>>
{
    public string DepartmentName { get; init; } = string.Empty;
    public string DepartmentCode { get; init; } = string.Empty;
}

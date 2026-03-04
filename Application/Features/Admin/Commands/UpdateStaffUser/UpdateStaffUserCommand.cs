using Application.DTOS;
using MediatR;

namespace Application.Features.Admin.Commands.UpdateStaffUser;

public record UpdateStaffUserCommand : IRequest<ApiResponse<StaffDto>>
{
    public Guid UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? DepartmentCode { get; init; }
    public bool? IsActive { get; init; }
}

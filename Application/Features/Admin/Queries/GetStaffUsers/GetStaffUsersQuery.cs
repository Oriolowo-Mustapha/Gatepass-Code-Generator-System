using Application.DTOS;
using MediatR;

namespace Application.Features.Admin.Queries.GetStaffUsers;

public record GetStaffUsersQuery : IRequest<ApiResponse<List<StaffDto>>>
{
    public string? RoleName { get; init; }
}

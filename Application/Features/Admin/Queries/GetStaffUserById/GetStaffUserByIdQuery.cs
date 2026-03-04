using Application.DTOS;
using MediatR;

namespace Application.Features.Admin.Queries.GetStaffUserById;

public record GetStaffUserByIdQuery : IRequest<ApiResponse<StaffDto>>
{
    public Guid UserId { get; init; }
}

using Application.DTOS;
using MediatR;

namespace Application.Features.Admin.Commands.DeleteStaffUser;

public record DeleteStaffUserCommand : IRequest<ApiResponse<Guid>>
{
    public Guid UserId { get; init; }
}

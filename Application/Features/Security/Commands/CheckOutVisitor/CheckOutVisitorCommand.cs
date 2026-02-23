using Application.DTOS;
using MediatR;

namespace Application.Features.Security.Commands.CheckOutVisitor;

public record CheckOutVisitorCommand : IRequest<ApiResponse<bool>>
{
    public Guid CheckInOutId { get; init; }
    public Guid AccessPointId { get; init; }
    public Guid SecurityPersonnelId { get; init; }
}

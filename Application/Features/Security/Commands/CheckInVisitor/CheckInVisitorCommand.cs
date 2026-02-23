using Application.DTOS;
using MediatR;

namespace Application.Features.Security.Commands.CheckInVisitor;

public record CheckInVisitorCommand : IRequest<ApiResponse<Guid>>
{
    public Guid GatepassId { get; init; }
    public Guid AccessPointId { get; init; }
    public Guid SecurityPersonnelId { get; init; }
}

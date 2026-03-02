using Application.DTOS;
using MediatR;

namespace Application.Features.Security.Commands.CheckInVisitor;

public record CheckInVisitorCommand : IRequest<ApiResponse<Guid>>
{
    public string GatepassCode { get; init; } = string.Empty;
    public string AccessPointName { get; init; } = string.Empty;
}

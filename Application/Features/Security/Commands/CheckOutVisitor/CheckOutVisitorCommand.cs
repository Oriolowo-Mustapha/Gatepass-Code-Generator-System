using Application.DTOS;
using MediatR;

namespace Application.Features.Security.Commands.CheckOutVisitor;

public record CheckOutVisitorCommand : IRequest<ApiResponse<bool>>
{
    public string GatepassCode { get; init; } = string.Empty;
    public string AccessPointName { get; init; } = string.Empty;
}

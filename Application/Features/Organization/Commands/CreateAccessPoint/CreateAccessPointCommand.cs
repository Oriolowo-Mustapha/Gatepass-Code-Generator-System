using Application.DTOS;
using MediatR;

namespace Application.Features.Organization.Commands.CreateAccessPoint;

public record CreateAccessPointCommand : IRequest<ApiResponse<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string LocationDescription { get; init; } = string.Empty;
}

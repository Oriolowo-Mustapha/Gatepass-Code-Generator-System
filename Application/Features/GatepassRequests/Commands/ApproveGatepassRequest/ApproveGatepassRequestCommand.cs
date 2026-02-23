using Application.DTOS;
using MediatR;

namespace Application.Features.GatepassRequests.Commands.ApproveGatepassRequest;

public record ApproveGatepassRequestCommand : IRequest<ApiResponse<string>>
{
    public Guid RequestId { get; init; }
    public Guid ApproverId { get; init; }
}

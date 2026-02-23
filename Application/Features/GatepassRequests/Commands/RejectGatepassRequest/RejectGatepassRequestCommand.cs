using Application.DTOS;
using MediatR;

namespace Application.Features.GatepassRequests.Commands.RejectGatepassRequest;

public record RejectGatepassRequestCommand : IRequest<ApiResponse<string>>
{
    public Guid RequestId { get; init; }
    public Guid ApproverId { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}

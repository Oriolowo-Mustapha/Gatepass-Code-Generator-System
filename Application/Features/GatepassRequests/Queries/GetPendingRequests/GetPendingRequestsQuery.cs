using Application.DTOS;
using MediatR;

namespace Application.Features.GatepassRequests.Queries.GetPendingRequests;

public record GetPendingRequestsQuery : IRequest<ApiResponse<List<GatepassRequestSummaryDto>>>
{
    public Guid? HostId { get; init; }
}

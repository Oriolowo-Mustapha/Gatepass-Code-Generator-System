using Application.DTOS;
using Application.Interfaces;
using MediatR;

namespace Application.Features.GatepassRequests.Queries.GetPendingRequests;

public class GetPendingRequestsQueryHandler
    : IRequestHandler<GetPendingRequestsQuery, ApiResponse<List<GatepassRequestSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingRequestsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<GatepassRequestSummaryDto>>> Handle(
        GetPendingRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var pendingRequests = await _unitOfWork.GatepassRequests
            .GetPendingRequestsForHostAsync(request.HostId, cancellationToken);

        var dtos = pendingRequests.Select(r => new GatepassRequestSummaryDto
        {
            Id = r.Id,
            VisitorName = r.Visitor?.FirstName + " " + r.Visitor?.LastName,
            VisitPurpose = r.VisitPurpose,
            RequestDate = r.RequestDate,
            ApprovalStatus = r.ApprovalStatus.ToString()
        }).ToList();

        return ApiResponse<List<GatepassRequestSummaryDto>>.Success(dtos);
    }
}

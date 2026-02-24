using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Features.GatepassRequests.Queries.GetPendingRequests;

public class GetPendingRequestsQueryHandler
    : IRequestHandler<GetPendingRequestsQuery, ApiResponse<List<GatepassRequestSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetPendingRequestsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<List<GatepassRequestSummaryDto>>> Handle(
        GetPendingRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var hostId = request.HostId ?? _currentUserService.UserId;

        if (!hostId.HasValue)
            return ApiResponse<List<GatepassRequestSummaryDto>>.Failure("Host ID is required or you must be logged in.");

        var pendingRequests = await _unitOfWork.GatepassRequests
            .GetPendingRequestsForHostAsync(hostId.Value, cancellationToken);

        var dtos = pendingRequests.Select(r => new GatepassRequestSummaryDto
        {
            Id = r.Id,
            VisitorName = r.Visitor != null ? $"{r.Visitor.FirstName} {r.Visitor.LastName}" : "Unknown Visitor",
            VisitPurpose = r.VisitPurpose,
            RequestDate = r.RequestDate,
            ApprovalStatus = r.ApprovalStatus.ToString()
        }).ToList();

        return ApiResponse<List<GatepassRequestSummaryDto>>.Success(dtos);
    }
}

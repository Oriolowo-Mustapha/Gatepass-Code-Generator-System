using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enum;
using MediatR;

namespace Application.Features.Reports.Queries.GetHostStatistics;

public class GetHostStatisticsQueryHandler
    : IRequestHandler<GetHostStatisticsQuery, ApiResponse<HostStatisticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetHostStatisticsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<HostStatisticsDto>> Handle(
        GetHostStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var hostId = _currentUserService.UserId;
        if (hostId is null)
            return ApiResponse<HostStatisticsDto>.Failure("User not authenticated.");

        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        var allHostRequests = await _unitOfWork.GatepassRequests
            .FindAsync(r => r.HostUserId == hostId.Value, cancellationToken);

        var todayRequests = allHostRequests
            .Where(r => r.RequestDate >= todayStart && r.RequestDate < todayEnd)
            .ToList();

        // Visitors today: approved requests for today that have a check-in
        var approvedGatepassIds = allHostRequests
            .Where(r => r.ApprovalStatus == ApprovalStatus.Approved && r.Gatepass != null)
            .Select(r => r.Gatepass!.Id)
            .ToList();

        var todayCheckIns = await _unitOfWork.Repository<CheckInOut>()
            .FindAsync(c => approvedGatepassIds.Contains(c.GatePassId)
                         && c.CheckInTime >= todayStart && c.CheckInTime < todayEnd, cancellationToken);

        var dto = new HostStatisticsDto
        {
            TotalRequestsToday = todayRequests.Count,
            TotalRequestsAllTime = allHostRequests.Count,
            TotalVisitorsToday = todayCheckIns.Select(c => c.GatePassId).Distinct().Count(),
            TotalApprovedRequests = allHostRequests.Count(r => r.ApprovalStatus == ApprovalStatus.Approved),
            TotalPendingRequests = allHostRequests.Count(r => r.ApprovalStatus == ApprovalStatus.Pending),
            TotalRejectedRequests = allHostRequests.Count(r => r.ApprovalStatus == ApprovalStatus.Rejected)
        };

        return ApiResponse<HostStatisticsDto>.Success(dto);
    }
}

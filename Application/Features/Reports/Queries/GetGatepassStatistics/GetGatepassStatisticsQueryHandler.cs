using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enum;
using MediatR;

namespace Application.Features.Reports.Queries.GetGatepassStatistics;

public class GetGatepassStatisticsQueryHandler
    : IRequestHandler<GetGatepassStatisticsQuery, ApiResponse<GatepassStatisticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetGatepassStatisticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GatepassStatisticsDto>> Handle(
        GetGatepassStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var allRequests = await _unitOfWork.GatepassRequests.GetAllAsync(cancellationToken);

        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        var todayCheckIns = await _unitOfWork.Repository<CheckInOut>()
            .FindAsync(c => c.CheckInTime >= todayStart && c.CheckInTime < todayEnd, cancellationToken);

        var dto = new GatepassStatisticsDto
        {
            TotalApproved = allRequests.Count(r => r.ApprovalStatus == ApprovalStatus.Approved),
            TotalVisitorsToday = todayCheckIns.Count
        };

        return ApiResponse<GatepassStatisticsDto>.Success(dto);
    }
}

using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enum;
using MediatR;

namespace Application.Features.Reports.Queries.GetSecurityStatistics;

public class GetSecurityStatisticsQueryHandler
    : IRequestHandler<GetSecurityStatisticsQuery, ApiResponse<SecurityStatisticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSecurityStatisticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<SecurityStatisticsDto>> Handle(
        GetSecurityStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        var todayRecords = await _unitOfWork.Repository<CheckInOut>()
            .FindAsync(c => c.CheckInTime >= todayStart && c.CheckInTime < todayEnd, cancellationToken);

        var totalCheckInsToday = todayRecords.Count;
        var totalCheckOutsToday = todayRecords.Count(r => r.CheckOutTime != null);

        var distinctGatepassIds = todayRecords.Select(r => r.GatePassId).Distinct().ToList();
        var totalVisitorsToday = distinctGatepassIds.Count;
 
        var gatepasses = await _unitOfWork.Gatepasses
            .FindAsync(g => distinctGatepassIds.Contains(g.Id), cancellationToken);

        var gatepassRequestIds = gatepasses.Select(g => g.GatePassRequestId).Distinct().ToList();
        var gatepassRequests = await _unitOfWork.GatepassRequests
            .FindAsync(r => gatepassRequestIds.Contains(r.Id), cancellationToken);

        var countByType = gatepassRequests
            .GroupBy(r => r.GatepassType)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        foreach (var type in System.Enum.GetValues<GatepassType>())
        {
            countByType.TryAdd(type.ToString(), 0);
        }

        var dto = new SecurityStatisticsDto
        {
            TotalVisitorsToday = totalVisitorsToday,
            TotalCheckInsToday = totalCheckInsToday,
            TotalCheckOutsToday = totalCheckOutsToday,
            CountByGatepassType = countByType
        };

        return ApiResponse<SecurityStatisticsDto>.Success(dto);
    }
}

using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Reports.Queries.GetDailyVisitorLog;

public class GetDailyVisitorLogQueryHandler
    : IRequestHandler<GetDailyVisitorLogQuery, ApiResponse<List<DailyVisitorLogDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDailyVisitorLogQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<DailyVisitorLogDto>>> Handle(
        GetDailyVisitorLogQuery request,
        CancellationToken cancellationToken)
    {
        var dayStart = request.Date.Date;
        var dayEnd = dayStart.AddDays(1);

        var records = await _unitOfWork.Repository<CheckInOut>()
            .FindAsync(c => c.CheckInTime >= dayStart && c.CheckInTime < dayEnd, cancellationToken);

        var gatepassIds = records.Select(r => r.GatePassId).Distinct().ToList();
        var gatepasses = await _unitOfWork.Gatepasses
            .FindAsync(g => gatepassIds.Contains(g.Id), cancellationToken);

        var gatepassLookup = gatepasses.ToDictionary(g => g.Id);

        var result = records.Select(r =>
        {
            gatepassLookup.TryGetValue(r.GatePassId, out var gatepass);
            var visitor = gatepass?.GatepassRequest?.Visitor;

            return new DailyVisitorLogDto
            {
                CheckInOutId = r.Id,
                VisitorName = visitor is not null
                    ? $"{visitor.FirstName} {visitor.LastName}"
                    : "Unknown",
                GatepassCode = gatepass?.UniqueCode ?? "N/A",
                AccessPointName = r.CheckInAccessPoint?.Name ?? "N/A",
                CheckInTime = r.CheckInTime,
                CheckOutTime = r.CheckOutTime,
                SecurityPersonnel = r.CheckInPersonnel?.UserName ?? "N/A",
                IsOverstay = r.IsOverstay
            };
        }).ToList();

        return ApiResponse<List<DailyVisitorLogDto>>.Success(result);
    }
}

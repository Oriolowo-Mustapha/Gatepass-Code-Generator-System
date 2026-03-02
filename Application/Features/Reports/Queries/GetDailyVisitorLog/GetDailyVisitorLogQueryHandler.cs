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
            .FindAsync(
                c => c.CheckInTime >= dayStart && c.CheckInTime < dayEnd,
                cancellationToken,
                "Gatepass.GatepassRequest.Visitor",
                "Gatepass.GatepassRequest.DestinationDepartment",
                "CheckInAccessPoint",
                "CheckInPersonnel");

        var result = records.Select(r =>
        {
            var gatepass = r.Gatepass;
            var gatepassRequest = gatepass?.GatepassRequest;
            var visitor = gatepassRequest?.Visitor;
            var department = gatepassRequest?.DestinationDepartment;

            return new DailyVisitorLogDto
            {
                CheckInOutId = r.Id,
                VisitorName = visitor != null ? $"{visitor.FirstName} {visitor.LastName}" : "Unknown",
                Email = visitor?.Email ?? "—",
                Department = department?.DepartmentName ?? "—",
                Purpose = gatepassRequest?.VisitPurpose ?? "—",
                GatepassCode = gatepass?.UniqueCode ?? "N/A",
                AccessPointName = r.CheckInAccessPoint?.Name ?? "N/A",
                CheckInTime = r.CheckInTime,
                CheckOutTime = r.CheckOutTime,
                SecurityPersonnel = r.CheckInPersonnel?.UserName ?? "N/A",
                IsOverstay = r.IsOverstay,
                Status = r.CheckOutTime.HasValue ? "Checked Out" : "On-site"
            };
        }).ToList();

        return ApiResponse<List<DailyVisitorLogDto>>.Success(result);
    }
}

using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Reports.Queries.GetOverstayReport;

public class GetOverstayReportQueryHandler
    : IRequestHandler<GetOverstayReportQuery, ApiResponse<List<OverstayReportDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOverstayReportQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<OverstayReportDto>>> Handle(
        GetOverstayReportQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var openCheckIns = await _unitOfWork.Repository<CheckInOut>()
            .FindAsync(c => c.CheckOutTime == null, cancellationToken);

        var gatepassIds = openCheckIns.Select(c => c.GatePassId).Distinct().ToList();
        var expiredGatepasses = await _unitOfWork.Gatepasses
            .FindAsync(g => gatepassIds.Contains(g.Id) && g.ValidUntil < now, cancellationToken);

        var expiredLookup = expiredGatepasses.ToDictionary(g => g.Id);

        var result = openCheckIns
            .Where(c => expiredLookup.ContainsKey(c.GatePassId))
            .Select(c =>
            {
                var gatepass = expiredLookup[c.GatePassId];
                var visitor = gatepass.GatepassRequest?.Visitor;

                return new OverstayReportDto
                {
                    GatepassId = gatepass.Id,
                    UniqueCode = gatepass.UniqueCode,
                    VisitorName = visitor is not null
                        ? $"{visitor.FirstName} {visitor.LastName}"
                        : "Unknown",
                    ValidUntil = gatepass.ValidUntil,
                    CheckInTime = c.CheckInTime,
                    AccessPointName = c.CheckInAccessPoint?.Name ?? "N/A"
                };
            }).ToList();

        return ApiResponse<List<OverstayReportDto>>.Success(result);
    }
}

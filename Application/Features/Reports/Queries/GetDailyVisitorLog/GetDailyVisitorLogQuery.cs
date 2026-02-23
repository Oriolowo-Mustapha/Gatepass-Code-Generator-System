using Application.DTOS;
using MediatR;

namespace Application.Features.Reports.Queries.GetDailyVisitorLog;

public record GetDailyVisitorLogQuery : IRequest<ApiResponse<List<DailyVisitorLogDto>>>
{
    public DateTime Date { get; init; }
}

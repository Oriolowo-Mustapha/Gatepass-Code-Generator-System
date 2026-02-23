using Application.DTOS;
using MediatR;

namespace Application.Features.Admin.Queries.GetAuditLogs;

public record GetAuditLogsQuery : IRequest<ApiResponse<List<AuditLogDto>>>
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

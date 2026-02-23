using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Admin.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, ApiResponse<List<AuditLogDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuditLogsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<AuditLogDto>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.MinValue;
        var endDate = request.EndDate?.AddDays(1) ?? DateTime.MaxValue;

        var logs = await _unitOfWork.Repository<AuditLog>()
            .FindAsync(a => a.Timestamp >= startDate && a.Timestamp < endDate, cancellationToken);

        var result = logs
            .OrderByDescending(a => a.Timestamp)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                Timestamp = a.Timestamp,
                UserID = a.UserID,
                Action = a.Action,
                EntityType = a.EntityType,
                EntityID = a.EntityID,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                Description = a.Description
            }).ToList();

        return ApiResponse<List<AuditLogDto>>.Success(result);
    }
}

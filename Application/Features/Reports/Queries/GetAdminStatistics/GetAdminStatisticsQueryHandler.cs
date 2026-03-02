using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enum;
using MediatR;

namespace Application.Features.Reports.Queries.GetAdminStatistics;

public class GetAdminStatisticsQueryHandler
    : IRequestHandler<GetAdminStatisticsQuery, ApiResponse<AdminStatisticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminStatisticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<AdminStatisticsDto>> Handle(
        GetAdminStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var allRequests = await _unitOfWork.GatepassRequests.GetAllAsync(cancellationToken);

        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        var todayCheckIns = await _unitOfWork.Repository<CheckInOut>()
            .FindAsync(c => c.CheckInTime >= todayStart && c.CheckInTime < todayEnd, cancellationToken);

        var accessPoints = await _unitOfWork.Repository<AccessPoint>().GetAllAsync(cancellationToken);
        var departments = await _unitOfWork.Departements.GetAllAsync(cancellationToken);

        var roles = await _unitOfWork.Roles.GetAllAsync(cancellationToken);
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

        var hostRoleId = roles.FirstOrDefault(r => r.RoleName == "Host")?.Id;
        var securityRoleId = roles.FirstOrDefault(r => r.RoleName == "Security")?.Id;

        var dto = new AdminStatisticsDto
        {
            TotalApprovedRequests = allRequests.Count(r => r.ApprovalStatus == ApprovalStatus.Approved),
            TotalVisitorsToday = todayCheckIns.Count,
            TotalPendingRequests = allRequests.Count(r => r.ApprovalStatus == ApprovalStatus.Pending),
            TotalRejectedRequests = allRequests.Count(r => r.ApprovalStatus == ApprovalStatus.Rejected),
            TotalAccessPoints = accessPoints.Count,
            TotalDepartments = departments.Count,
            TotalHosts = hostRoleId.HasValue ? users.Count(u => u.RoleId == hostRoleId.Value) : 0,
            TotalSecurity = securityRoleId.HasValue ? users.Count(u => u.RoleId == securityRoleId.Value) : 0
        };

        return ApiResponse<AdminStatisticsDto>.Success(dto);
    }
}

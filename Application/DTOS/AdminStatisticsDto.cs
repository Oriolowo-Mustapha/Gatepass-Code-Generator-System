namespace Application.DTOS;

public record AdminStatisticsDto
{
    public int TotalApprovedRequests { get; init; }
    public int TotalVisitorsToday { get; init; }
    public int TotalPendingRequests { get; init; }
    public int TotalRejectedRequests { get; init; }
    public int TotalAccessPoints { get; init; }
    public int TotalDepartments { get; init; }
    public int TotalHosts { get; init; }
    public int TotalSecurity { get; init; }
}

namespace Application.DTOS;

public record HostStatisticsDto
{
    public int TotalRequestsToday { get; init; }
    public int TotalRequestsAllTime { get; init; }
    public int TotalVisitorsToday { get; init; }
    public int TotalApprovedRequests { get; init; }
    public int TotalPendingRequests { get; init; }
    public int TotalRejectedRequests { get; init; }
}

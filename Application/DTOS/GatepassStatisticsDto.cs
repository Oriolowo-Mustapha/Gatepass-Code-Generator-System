namespace Application.DTOS;

public record GatepassStatisticsDto
{
    public int TotalPending { get; init; }
    public int TotalApproved { get; init; }
    public int TotalRejected { get; init; }
    public int TotalVisitorsToday { get; init; }
}

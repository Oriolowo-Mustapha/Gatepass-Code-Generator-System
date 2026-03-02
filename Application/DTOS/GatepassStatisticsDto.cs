namespace Application.DTOS;

public record GatepassStatisticsDto
{
    public int TotalApproved { get; init; }
    public int TotalVisitorsToday { get; init; }
}

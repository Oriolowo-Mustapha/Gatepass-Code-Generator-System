namespace Application.DTOS;

public record SecurityStatisticsDto
{
    public int TotalVisitorsToday { get; init; }
    public int TotalCheckInsToday { get; init; }
    public int TotalCheckOutsToday { get; init; }
    public Dictionary<string, int> CountByGatepassType { get; init; } = new();
}

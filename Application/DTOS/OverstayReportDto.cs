namespace Application.DTOS;

public record OverstayReportDto
{
    public Guid GatepassId { get; init; }
    public string UniqueCode { get; init; } = string.Empty;
    public string VisitorName { get; init; } = string.Empty;
    public DateTime ValidUntil { get; init; }
    public DateTime CheckInTime { get; init; }
    public string AccessPointName { get; init; } = string.Empty;
}

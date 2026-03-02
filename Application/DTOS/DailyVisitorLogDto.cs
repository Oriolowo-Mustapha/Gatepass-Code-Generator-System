namespace Application.DTOS;

public record DailyVisitorLogDto
{
    public Guid CheckInOutId { get; init; }
    public string VisitorName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string Purpose { get; init; } = string.Empty;
    public string GatepassCode { get; init; } = string.Empty;
    public string AccessPointName { get; init; } = string.Empty;
    public DateTime CheckInTime { get; init; }
    public DateTime? CheckOutTime { get; init; }
    public string SecurityPersonnel { get; init; } = string.Empty;
    public bool IsOverstay { get; init; }
    public string Status { get; init; } = string.Empty;
}

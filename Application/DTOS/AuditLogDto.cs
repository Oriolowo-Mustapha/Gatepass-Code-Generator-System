namespace Application.DTOS;

public record AuditLogDto
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
    public Guid? UserID { get; init; }
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid? EntityID { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public string Description { get; init; } = string.Empty;
}

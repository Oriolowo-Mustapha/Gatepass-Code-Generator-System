namespace Application.DTOS;

public record GatepassVerificationDto
{
    public bool IsValid { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? VisitorName { get; init; }
    public string? Email { get; init; }
}

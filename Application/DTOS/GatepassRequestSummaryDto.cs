namespace Application.DTOS;

public record GatepassRequestSummaryDto
{
    public Guid Id { get; init; }
    public string VisitorName { get; init; } = string.Empty;
    public string VisitPurpose { get; init; } = string.Empty;
    public DateTime RequestDate { get; init; }
    public string ApprovalStatus { get; init; } = string.Empty;
}

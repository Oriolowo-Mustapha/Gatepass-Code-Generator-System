namespace Application.DTOS;

public record AccessPointDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string LocationDescription { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

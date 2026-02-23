namespace Application.DTOS;

public record SystemConfigurationDto
{
    public Guid Id { get; init; }
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime LastModified { get; init; }
}

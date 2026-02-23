namespace Application.DTOS;

public record DepartmentDto
{
    public Guid Id { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public string DepartmentCode { get; init; } = string.Empty;
    public string HeadOfDepartment { get; init; } = string.Empty;
}

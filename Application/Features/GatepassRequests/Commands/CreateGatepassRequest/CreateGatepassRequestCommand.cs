using Application.DTOS;
using MediatR;

namespace Application.Features.GatepassRequests.Commands.CreateGatepassRequest;

public record CreateGatepassRequestCommand : IRequest<ApiResponse<Guid>>
{
    public string VisitorFirstName { get; init; } = string.Empty;
    public string VisitorLastName { get; init; } = string.Empty;
    public string VisitorContactNumber { get; init; } = string.Empty;
    public string VisitorEmail { get; init; } = string.Empty;
    public Guid? DestinationDepartmentId { get; init; }
    public string VisitPurpose { get; init; } = string.Empty;
    public DateTime ValidFrom { get; init; }
    public DateTime ValidUntil { get; init; }

    public string? PlateNumber { get; init; }
    public int? VehicleType { get; init; }
    public string? VehicleColor { get; init; }
    public string? VehicleModel { get; init; }
    public string? DriverName { get; init; }
}

using Application.DTOS;
using MediatR;
using System;

namespace Application.Features.GatepassRequests.Commands.CreateGatepassRequest;

public record CreateGatepassRequestCommand : IRequest<ApiResponse<string>>
{
    public string VisitorFirstName { get; init; } = string.Empty;
    public string VisitorLastName { get; init; } = string.Empty;
    public string VisitorEmail { get; init; } = string.Empty;
    public string VisitorPhoneNumber { get; init; } = string.Empty;

    public string DestinationDepartmentCode { get; init; } = string.Empty;
    public string VisitPurpose { get; init; } = string.Empty;
    public DateTime ValidFrom { get; init; }
    public DateTime ValidUntil { get; init; }
    
    public VehicleDetailsRequest? Vehicle { get; init; }
}

public record VehicleDetailsRequest(
    string? PlateNumber
);

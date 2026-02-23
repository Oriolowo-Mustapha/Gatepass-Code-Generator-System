using Application.DTOS;
using MediatR;

namespace Application.Features.Security.Queries.VerifyGatepass;

public record VerifyGatepassQuery : IRequest<ApiResponse<GatepassVerificationDto>>
{
    public string ScannedUniqueCode { get; init; } = string.Empty;
}

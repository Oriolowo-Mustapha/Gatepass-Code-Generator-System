using Application.DTOS;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Security.Queries.VerifyGatepass;

public class VerifyGatepassQueryHandler
    : IRequestHandler<VerifyGatepassQuery, ApiResponse<GatepassVerificationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public VerifyGatepassQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GatepassVerificationDto>> Handle(
        VerifyGatepassQuery request,
        CancellationToken cancellationToken)
    {
        var gatepass = await _unitOfWork.Gatepasses
            .GetByUniqueCodeAsync(request.ScannedUniqueCode, cancellationToken);

        if (gatepass is null)
        {
            return ApiResponse<GatepassVerificationDto>.Success(
                new GatepassVerificationDto
                {
                    IsValid = false,
                    Message = "Gatepass not found. Invalid QR code."
                });
        }

        if (!gatepass.IsActive)
        {
            return ApiResponse<GatepassVerificationDto>.Success(
                new GatepassVerificationDto
                {
                    IsValid = false,
                    Message = "This gatepass is no longer active."
                });
        }

        if (gatepass.IsRevoked)
        {
            return ApiResponse<GatepassVerificationDto>.Success(
                new GatepassVerificationDto
                {
                    IsValid = false,
                    Message = "This gatepass has been revoked."
                });
        }

        var now = DateTime.UtcNow;
        if (now < gatepass.ValidFrom || now > gatepass.ValidUntil)
        {
            return ApiResponse<GatepassVerificationDto>.Success(
                new GatepassVerificationDto
                {
                    IsValid = false,
                    Message = now < gatepass.ValidFrom
                        ? "This gatepass is not yet valid."
                        : "This gatepass has expired."
                });
        }

        var visitor = gatepass.GatepassRequest?.Visitor;

        return ApiResponse<GatepassVerificationDto>.Success(
            new GatepassVerificationDto
            {
                IsValid = true,
                Message = "Gatepass is valid. Access granted.",
                VisitorName = visitor is not null
                    ? $"{visitor.FirstName} {visitor.LastName}"
                    : null,
                PhotoUrl = visitor?.PhotoUrl
            });
    }
}

using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Security.Commands.CheckOutVisitor;

public class CheckOutVisitorCommandHandler
    : IRequestHandler<CheckOutVisitorCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CheckOutVisitorCommandHandler> _logger;

    public CheckOutVisitorCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, ILogger<CheckOutVisitorCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(
        CheckOutVisitorCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Check-out attempt for gatepass code {Code} at access point '{AccessPoint}'", request.GatepassCode, request.AccessPointName);

        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            _logger.LogWarning("Unauthenticated check-out attempt");
            return ApiResponse<bool>.Failure("User is not authenticated.");
        }

        if (!_currentUser.IsInRole("Security"))
        {
            _logger.LogWarning("Non-security user {UserId} attempted check-out", _currentUser.UserId);
            return ApiResponse<bool>.Failure("Only security personnel can perform check-outs.");
        }

        var securityPersonnelId = _currentUser.UserId.Value;

        var accessPoints = await _unitOfWork.Repository<AccessPoint>()
            .FindAsync(a => a.Name == request.AccessPointName, cancellationToken);

        var accessPoint = accessPoints.FirstOrDefault();

        if (accessPoint is null)
        {
            _logger.LogWarning("Access point '{AccessPoint}' not found", request.AccessPointName);
            return ApiResponse<bool>.Failure("Access point not found.");
        }

        if (!accessPoint.IsActive)
        {
            _logger.LogWarning("Access point '{AccessPoint}' is inactive", request.AccessPointName);
            return ApiResponse<bool>.Failure("Access point is not active.");
        }

        var gatepass = await _unitOfWork.Gatepasses
            .GetByUniqueCodeAsync(request.GatepassCode, cancellationToken);

        if (gatepass is null)
        {
            _logger.LogWarning("Invalid gatepass code '{Code}' scanned at {AccessPoint}", request.GatepassCode, request.AccessPointName);
            return ApiResponse<bool>.Failure("Invalid gatepass code.");
        }

        var checkInRecords = await _unitOfWork.Repository<CheckInOut>()
            .FindAsync(c => c.GatePassId == gatepass.Id && c.CheckOutTime == null, cancellationToken);

        var checkInOut = checkInRecords.FirstOrDefault();

        if (checkInOut is null)
        {
            _logger.LogWarning("No active check-in found for gatepass {GatepassId}", gatepass.Id);
            return ApiResponse<bool>.Failure("No active check-in found for this gatepass.");
        }

        var now = DateTime.UtcNow;
        checkInOut.CheckOutTime = now;
        checkInOut.CheckOutAccessPointId = accessPoint.Id;
        checkInOut.CheckOutPersonnelId = securityPersonnelId;

        if (now > gatepass.ValidUntil)
        {
            checkInOut.IsOverstay = true;
            _logger.LogWarning("Overstay detected for gatepass {GatepassId}, valid until {ValidUntil}, checked out at {CheckOutTime}", gatepass.Id, gatepass.ValidUntil, now);
        }

        _unitOfWork.Repository<CheckInOut>().Update(checkInOut);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Check-out successful: Record {CheckInOutId}, Gatepass {GatepassId}, AccessPoint '{AccessPoint}', Personnel {PersonnelId}",
            checkInOut.Id, gatepass.Id, request.AccessPointName, securityPersonnelId);

        return ApiResponse<bool>.Success(true, "Visitor checked out successfully.");
    }
}

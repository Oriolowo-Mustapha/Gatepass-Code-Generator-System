using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Security.Commands.CheckInVisitor;

public class CheckInVisitorCommandHandler
    : IRequestHandler<CheckInVisitorCommand, ApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CheckInVisitorCommandHandler> _logger;

    public CheckInVisitorCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, ILogger<CheckInVisitorCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiResponse<Guid>> Handle(
        CheckInVisitorCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Check-in attempt for gatepass code {Code} at access point '{AccessPoint}'", request.GatepassCode, request.AccessPointName);

        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            _logger.LogWarning("Unauthenticated check-in attempt");
            return ApiResponse<Guid>.Failure("User is not authenticated.");
        }

        if (!_currentUser.IsInRole("Security"))
        {
            _logger.LogWarning("Non-security user {UserId} attempted check-in", _currentUser.UserId);
            return ApiResponse<Guid>.Failure("Only security personnel can perform check-ins.");
        }

        var securityPersonnelId = _currentUser.UserId.Value;

        var accessPoints = await _unitOfWork.Repository<AccessPoint>()
            .FindAsync(a => a.Name == request.AccessPointName, cancellationToken);

        var accessPoint = accessPoints.FirstOrDefault();

        if (accessPoint is null)
        {
            _logger.LogWarning("Access point '{AccessPoint}' not found", request.AccessPointName);
            return ApiResponse<Guid>.Failure("Access point not found.");
        }

        if (!accessPoint.IsActive)
        {
            _logger.LogWarning("Access point '{AccessPoint}' is inactive", request.AccessPointName);
            return ApiResponse<Guid>.Failure("Access point is not active.");
        }

        var gatepass = await _unitOfWork.Gatepasses
            .GetByUniqueCodeAsync(request.GatepassCode, cancellationToken);

        if (gatepass is null)
        {
            _logger.LogWarning("Invalid gatepass code '{Code}' scanned at {AccessPoint}", request.GatepassCode, request.AccessPointName);
            return ApiResponse<Guid>.Failure("Invalid gatepass code.");
        }

        if (!gatepass.IsActive || gatepass.IsRevoked)
        {
            _logger.LogWarning("Gatepass {GatepassId} is inactive/revoked, check-in denied", gatepass.Id);
            return ApiResponse<Guid>.Failure("Gatepass is not active or has been revoked.");
        }

        var now = DateTime.UtcNow;
        if (now < gatepass.ValidFrom || now > gatepass.ValidUntil)
        {
            _logger.LogWarning("Gatepass {GatepassId} is outside validity period ({From} - {Until})", gatepass.Id, gatepass.ValidFrom, gatepass.ValidUntil);
            return ApiResponse<Guid>.Failure("Gatepass is outside its validity period.");
        }

        var checkInOut = new CheckInOut
        {
            GatePassId = gatepass.Id,
            CheckInTime = now,
            CheckInAccessPointId = accessPoint.Id,
            CheckInPersonnelId = securityPersonnelId
        };

        await _unitOfWork.Repository<CheckInOut>().AddAsync(checkInOut, cancellationToken);

        gatepass.UsageCount++;
        _unitOfWork.Gatepasses.Update(gatepass);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Check-in successful: Record {CheckInOutId}, Gatepass {GatepassId}, AccessPoint '{AccessPoint}', Personnel {PersonnelId}",
            checkInOut.Id, gatepass.Id, request.AccessPointName, securityPersonnelId);

        return ApiResponse<Guid>.Success(checkInOut.Id, "Visitor checked in successfully.");
    }
}

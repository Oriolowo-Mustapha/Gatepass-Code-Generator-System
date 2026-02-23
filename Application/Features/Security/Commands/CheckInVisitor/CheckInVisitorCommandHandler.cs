using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Security.Commands.CheckInVisitor;

public class CheckInVisitorCommandHandler
    : IRequestHandler<CheckInVisitorCommand, ApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckInVisitorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<Guid>> Handle(
        CheckInVisitorCommand request,
        CancellationToken cancellationToken)
    {
        var gatepass = await _unitOfWork.Gatepasses
            .GetByIdAsync(request.GatepassId, cancellationToken);

        if (gatepass is null)
            return ApiResponse<Guid>.Failure("Gatepass not found.");

        if (!gatepass.IsActive || gatepass.IsRevoked)
            return ApiResponse<Guid>.Failure("Gatepass is not active or has been revoked.");

        var now = DateTime.UtcNow;
        if (now < gatepass.ValidFrom || now > gatepass.ValidUntil)
            return ApiResponse<Guid>.Failure("Gatepass is outside its validity period.");

        var checkInOut = new CheckInOut
        {
            GatePassId = request.GatepassId,
            CheckInTime = now,
            CheckInAccessPointId = request.AccessPointId,
            CheckInPersonnelId = request.SecurityPersonnelId
        };

        await _unitOfWork.Repository<CheckInOut>().AddAsync(checkInOut, cancellationToken);

        gatepass.UsageCount++;
        _unitOfWork.Gatepasses.Update(gatepass);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.Success(checkInOut.Id, "Visitor checked in successfully.");
    }
}

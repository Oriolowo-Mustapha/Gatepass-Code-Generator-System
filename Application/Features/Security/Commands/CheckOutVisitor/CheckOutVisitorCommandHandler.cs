using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Security.Commands.CheckOutVisitor;

public class CheckOutVisitorCommandHandler
    : IRequestHandler<CheckOutVisitorCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckOutVisitorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(
        CheckOutVisitorCommand request,
        CancellationToken cancellationToken)
    {
        var checkInOut = await _unitOfWork.Repository<CheckInOut>()
            .GetByIdAsync(request.CheckInOutId, cancellationToken);

        if (checkInOut is null)
            return ApiResponse<bool>.Failure("Check-in record not found.");

        if (checkInOut.CheckOutTime is not null)
            return ApiResponse<bool>.Failure("Visitor has already been checked out.");

        var now = DateTime.UtcNow;
        checkInOut.CheckOutTime = now;
        checkInOut.CheckOutAccessPointId = request.AccessPointId;
        checkInOut.CheckOutPersonnelId = request.SecurityPersonnelId;

        var gatepass = await _unitOfWork.Gatepasses
            .GetByIdAsync(checkInOut.GatePassId, cancellationToken);

        if (gatepass is not null && now > gatepass.ValidUntil)
        {
            checkInOut.IsOverstay = true;
        }

        _unitOfWork.Repository<CheckInOut>().Update(checkInOut);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true, "Visitor checked out successfully.");
    }
}

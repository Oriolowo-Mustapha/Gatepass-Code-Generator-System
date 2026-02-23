using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enum;
using MediatR;

namespace Application.Features.GatepassRequests.Commands.CreateGatepassRequest;

public class CreateGatepassRequestCommandHandler
    : IRequestHandler<CreateGatepassRequestCommand, ApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateGatepassRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<Guid>> Handle(
        CreateGatepassRequestCommand request,
        CancellationToken cancellationToken)
    {
        var visitorExists = await _unitOfWork.Repository<Visitor>()
            .ExistsAsync(request.VisitorId, cancellationToken);

        if (!visitorExists)
            return ApiResponse<Guid>.Failure("Visitor not found.");

        var hostExists = await _unitOfWork.Users
            .ExistsAsync(request.HostUserId, cancellationToken);

        if (!hostExists)
            return ApiResponse<Guid>.Failure("Host user not found.");

        var gatepassRequest = new GatepassRequest
        {
            VisitorsId = request.VisitorId,
            HostUserId = request.HostUserId,
            DestinationDepartmentId = request.DestinationDepartmentId,
            VisitPurpose = request.VisitPurpose,
            RequestedDate = request.ValidFrom,
            RequestedDuration = request.ValidUntil,
            ApprovalStatus = ApprovalStatus.Pending,
            LastModifiedDate = DateTime.UtcNow
        };

        if (!string.IsNullOrWhiteSpace(request.PlateNumber))
        {
            gatepassRequest.VehicleDetails = new VehicleDetails
            {
                GatePassRequestID = gatepassRequest.Id,
                PlateNumber = request.PlateNumber,
                VehicleType = (VehicleType)(request.VehicleType ?? 0),
                VehicleColor = request.VehicleColor ?? string.Empty,
                VehicleModel = request.VehicleModel ?? string.Empty,
                DriverName = request.DriverName ?? string.Empty
            };
            gatepassRequest.GatepassType = GatepassType.Vehicle;
        }
        else
        {
            gatepassRequest.GatepassType = GatepassType.Visitor;
        }

        await _unitOfWork.GatepassRequests.AddAsync(gatepassRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.Success(gatepassRequest.Id, "Gatepass request created successfully.");
    }
}

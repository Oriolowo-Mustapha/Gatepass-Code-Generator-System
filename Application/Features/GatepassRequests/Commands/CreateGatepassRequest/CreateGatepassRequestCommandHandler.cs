using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enum;
using MediatR;

namespace Application.Features.GatepassRequests.Commands.CreateGatepassRequest;

public class CreateGatepassRequestCommandHandler
    : IRequestHandler<CreateGatepassRequestCommand, ApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateGatepassRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<Guid>> Handle(
        CreateGatepassRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
            return ApiResponse<Guid>.Failure("You must be logged in as a host to create a gatepass request.");

        var hostId = _currentUserService.UserId.Value;

        var hostExists = await _unitOfWork.Users
            .ExistsAsync(hostId, cancellationToken);

        if (!hostExists)
            return ApiResponse<Guid>.Failure("Host user not found.");

        // Find or create Visitor
        var visitor = await _unitOfWork.Repository<Visitor>().FindAsync(
            v => v.FirstName == request.VisitorFirstName &&
                 v.LastName == request.VisitorLastName &&
                 v.ContactNumber == request.VisitorContactNumber &&
                 v.Email == request.VisitorEmail,
            cancellationToken
        );
        
        Visitor? existingVisitor = null;
        if(visitor != null && visitor.Count > 0){
             existingVisitor = visitor.FirstOrDefault();
        }

        if (existingVisitor == null)
        {
            existingVisitor = new Visitor
            {
                FirstName = request.VisitorFirstName,
                LastName = request.VisitorLastName,
                ContactNumber = request.VisitorContactNumber,
                Email = request.VisitorEmail,
                RegistrationDate = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Visitor>().AddAsync(existingVisitor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var gatepassRequest = new GatepassRequest
        {
            VisitorsId = existingVisitor.Id,
            HostUserId = hostId,
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

using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Organization.Commands.CreateAccessPoint;

public class CreateAccessPointCommandHandler
    : IRequestHandler<CreateAccessPointCommand, ApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccessPointCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<Guid>> Handle(
        CreateAccessPointCommand request,
        CancellationToken cancellationToken)
    {
        var accessPoint = new AccessPoint
        {
            Name = request.Name,
            LocationDescription = request.LocationDescription,
            IsActive = true
        };

        await _unitOfWork.Repository<AccessPoint>().AddAsync(accessPoint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.Success(accessPoint.Id, "Access point created successfully.");
    }
}

using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Organization.Queries.GetAccessPoints;

public class GetAccessPointsQueryHandler
    : IRequestHandler<GetAccessPointsQuery, ApiResponse<List<AccessPointDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAccessPointsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<AccessPointDto>>> Handle(
        GetAccessPointsQuery request,
        CancellationToken cancellationToken)
    {
        var accessPoints = await _unitOfWork.Repository<AccessPoint>()
            .GetAllAsync(cancellationToken);

        var result = accessPoints.Select(ap => new AccessPointDto
        {
            Id = ap.Id,
            Name = ap.Name,
            LocationDescription = ap.LocationDescription,
            IsActive = ap.IsActive
        }).ToList();

        return ApiResponse<List<AccessPointDto>>.Success(result);
    }
}

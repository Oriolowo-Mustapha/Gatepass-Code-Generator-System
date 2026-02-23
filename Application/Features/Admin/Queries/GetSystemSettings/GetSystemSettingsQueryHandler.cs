using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Admin.Queries.GetSystemSettings;

public class GetSystemSettingsQueryHandler
    : IRequestHandler<GetSystemSettingsQuery, ApiResponse<List<SystemConfigurationDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSystemSettingsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<SystemConfigurationDto>>> Handle(
        GetSystemSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await _unitOfWork.Repository<SystemConfiguration>()
            .GetAllAsync(cancellationToken);

        var result = settings.Select(s => new SystemConfigurationDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.Value,
            Description = s.Description,
            LastModified = s.LastModified
        }).ToList();

        return ApiResponse<List<SystemConfigurationDto>>.Success(result);
    }
}

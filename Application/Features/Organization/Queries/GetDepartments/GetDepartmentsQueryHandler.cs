using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Organization.Queries.GetDepartments;

public class GetDepartmentsQueryHandler
    : IRequestHandler<GetDepartmentsQuery, ApiResponse<List<DepartmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDepartmentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<DepartmentDto>>> Handle(
        GetDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        var departments = await _unitOfWork.Repository<Department>()
            .GetAllAsync(cancellationToken);

        var result = departments.Select(d => new DepartmentDto
        {
            Id = d.Id,
            DepartmentName = d.DepartmentName,
            DepartmentCode = d.DepartmentCode,
            HeadOfDepartment = d.HeadOfDepartment
        }).ToList();

        return ApiResponse<List<DepartmentDto>>.Success(result);
    }
}

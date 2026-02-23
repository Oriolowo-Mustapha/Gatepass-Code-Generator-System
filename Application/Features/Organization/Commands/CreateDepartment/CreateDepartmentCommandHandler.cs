using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Organization.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler
    : IRequestHandler<CreateDepartmentCommand, ApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<Guid>> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department = new Department
        {
            DepartmentName = request.DepartmentName,
            DepartmentCode = request.DepartmentCode,
            HeadOfDepartment = request.HeadOfDepartment
        };

        await _unitOfWork.Repository<Department>().AddAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.Success(department.Id, "Department created successfully.");
    }
}

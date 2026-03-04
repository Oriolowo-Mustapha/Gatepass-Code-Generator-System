using Application.DTOS;
using Application.Exceptions;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Admin.Queries.GetStaffUserById;

public class GetStaffUserByIdQueryHandler : IRequestHandler<GetStaffUserByIdQuery, ApiResponse<StaffDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStaffUserByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<StaffDto>> Handle(GetStaffUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdWithDetailsAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return ApiResponse<StaffDto>.Failure("User not found");
        }

        var result = new StaffDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            RoleName = user.Role?.RoleName ?? string.Empty,
            DepartmentName = user.Department?.DepartmentName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginDate = user.LastLoginDate
        };

        return ApiResponse<StaffDto>.Success(result);
    }
}

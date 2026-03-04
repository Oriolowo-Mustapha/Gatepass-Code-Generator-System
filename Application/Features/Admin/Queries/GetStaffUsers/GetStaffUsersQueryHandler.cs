using Application.DTOS;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Admin.Queries.GetStaffUsers;

public class GetStaffUsersQueryHandler : IRequestHandler<GetStaffUsersQuery, ApiResponse<List<StaffDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    private static readonly string[] StaffRoles = ["Host", "Security"];

    public GetStaffUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<StaffDto>>> Handle(GetStaffUsersQuery request, CancellationToken cancellationToken)
    {
        var roleFilter = !string.IsNullOrWhiteSpace(request.RoleName)
            ? [request.RoleName]
            : StaffRoles;

        var users = await _unitOfWork.Users.GetByRoleNamesAsync(roleFilter, cancellationToken);

        var result = users.Select(u => new StaffDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhoneNumber = u.PhoneNumber,
            RoleName = u.Role?.RoleName ?? string.Empty,
            DepartmentName = u.Department?.DepartmentName,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            LastLoginDate = u.LastLoginDate
        }).ToList();

        return ApiResponse<List<StaffDto>>.Success(result);
    }
}

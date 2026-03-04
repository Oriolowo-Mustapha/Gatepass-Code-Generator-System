using Application.DTOS;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Commands.UpdateStaffUser;

public class UpdateStaffUserCommandHandler : IRequestHandler<UpdateStaffUserCommand, ApiResponse<StaffDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateStaffUserCommandHandler> _logger;

    public UpdateStaffUserCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateStaffUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<StaffDto>> Handle(UpdateStaffUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdWithDetailsAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return ApiResponse<StaffDto>.Failure("User not found");
        }

        if (request.FirstName is not null)
            user.FirstName = request.FirstName;

        if (request.LastName is not null)
            user.LastName = request.LastName;

        if (request.PhoneNumber is not null)
            user.PhoneNumber = request.PhoneNumber;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        if (request.DepartmentCode is not null)
        {
            var deptId = await _unitOfWork.Departements.GetDeptIdBydeptCodeAsync(request.DepartmentCode);
            user.DepartmentId = deptId;
        }

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Admin updated user {UserId} ({Email})", user.Id, user.Email);

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

        return ApiResponse<StaffDto>.Success(result, "User Updated Successfully");
    }
}

using Application.DTOS;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Commands.DeleteStaffUser;

public class DeleteStaffUserCommandHandler : IRequestHandler<DeleteStaffUserCommand, ApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteStaffUserCommandHandler> _logger;

    private static readonly string[] StaffRoles = ["Host", "Security"];

    public DeleteStaffUserCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteStaffUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<Guid>> Handle(DeleteStaffUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdWithRoleAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return ApiResponse<Guid>.Failure("User not found");
        }

        var roleName = user.Role?.RoleName ?? string.Empty;
        if (!StaffRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
        {
            return ApiResponse<Guid>.Failure("Only Host and Security users can be deleted through this endpoint");
        }

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Admin deleted user {UserId} ({Email}, Role: {Role})", user.Id, user.Email, roleName);

        return ApiResponse<Guid>.Success(user.Id, "User Deleted Successfully");
    }
}

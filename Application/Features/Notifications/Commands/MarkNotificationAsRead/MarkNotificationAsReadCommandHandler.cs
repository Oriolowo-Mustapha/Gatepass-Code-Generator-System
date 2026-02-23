using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler
    : IRequestHandler<MarkNotificationAsReadCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsReadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Repository<Notification>()
            .GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification is null)
            return ApiResponse<bool>.Failure("Notification not found.");

        notification.IsRead = true;
        _unitOfWork.Repository<Notification>().Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true, "Notification marked as read.");
    }
}

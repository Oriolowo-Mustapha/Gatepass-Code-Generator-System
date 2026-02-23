using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Notifications.Queries.GetUserNotifications;

public class GetUserNotificationsQueryHandler
    : IRequestHandler<GetUserNotificationsQuery, ApiResponse<List<NotificationDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserNotificationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<NotificationDto>>> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await _unitOfWork.Repository<Notification>()
            .FindAsync(n => n.UserId == request.UserId && !n.IsRead, cancellationToken);

        var result = notifications
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReferenceLink = n.ReferenceLink
            }).ToList();

        return ApiResponse<List<NotificationDto>>.Success(result);
    }
}

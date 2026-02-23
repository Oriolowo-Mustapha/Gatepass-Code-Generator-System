using Application.DTOS;
using MediatR;

namespace Application.Features.Notifications.Queries.GetUserNotifications;

public record GetUserNotificationsQuery : IRequest<ApiResponse<List<NotificationDto>>>
{
    public Guid UserId { get; init; }
}

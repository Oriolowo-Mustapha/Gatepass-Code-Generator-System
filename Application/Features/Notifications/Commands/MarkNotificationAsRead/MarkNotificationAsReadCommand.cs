using Application.DTOS;
using MediatR;

namespace Application.Features.Notifications.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand : IRequest<ApiResponse<bool>>
{
    public Guid NotificationId { get; init; }
}

using Application.DTOS;
using MediatR;

namespace Application.Features.Admin.Commands.UpdateSystemSetting;

public record UpdateSystemSettingCommand : IRequest<ApiResponse<bool>>
{
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public Guid? UpdatedByUserId { get; init; }
}

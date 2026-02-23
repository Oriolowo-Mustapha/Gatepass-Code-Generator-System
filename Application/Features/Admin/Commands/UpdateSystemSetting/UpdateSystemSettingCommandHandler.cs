using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Admin.Commands.UpdateSystemSetting;

public class UpdateSystemSettingCommandHandler
    : IRequestHandler<UpdateSystemSettingCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSystemSettingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(
        UpdateSystemSettingCommand request,
        CancellationToken cancellationToken)
    {
        var settings = await _unitOfWork.Repository<SystemConfiguration>()
            .FindAsync(s => s.Key == request.Key, cancellationToken);

        var setting = settings.FirstOrDefault();
        if (setting is null)
            return ApiResponse<bool>.Failure($"System setting with key '{request.Key}' not found.");

        var oldValue = setting.Value;
        setting.Value = request.Value;
        setting.LastModified = DateTime.UtcNow;
        _unitOfWork.Repository<SystemConfiguration>().Update(setting);

        var auditLog = new AuditLog
        {
            Timestamp = DateTime.UtcNow,
            UserID = request.UpdatedByUserId,
            Action = "UpdateSystemSetting",
            EntityType = nameof(SystemConfiguration),
            EntityID = setting.Id,
            OldValue = oldValue,
            NewValue = request.Value,
            Description = $"Updated system setting '{request.Key}' from '{oldValue}' to '{request.Value}'."
        };

        await _unitOfWork.Repository<AuditLog>().AddAsync(auditLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true, "System setting updated successfully.");
    }
}

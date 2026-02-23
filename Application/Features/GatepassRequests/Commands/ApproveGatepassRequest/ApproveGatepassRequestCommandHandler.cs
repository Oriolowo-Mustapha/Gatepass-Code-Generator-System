using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enum;
using MediatR;

namespace Application.Features.GatepassRequests.Commands.ApproveGatepassRequest;

public class ApproveGatepassRequestCommandHandler
    : IRequestHandler<ApproveGatepassRequestCommand, ApiResponse<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUniqueCodeGenerator _codeGenerator;
    private readonly IQRCodeGenerator _qrCodeGenerator;

    public ApproveGatepassRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IUniqueCodeGenerator codeGenerator,
        IQRCodeGenerator qrCodeGenerator)
    {
        _unitOfWork = unitOfWork;
        _codeGenerator = codeGenerator;
        _qrCodeGenerator = qrCodeGenerator;
    }

    public async Task<ApiResponse<string>> Handle(
        ApproveGatepassRequestCommand request,
        CancellationToken cancellationToken)
    {
        var gatepassRequest = await _unitOfWork.GatepassRequests
            .GetByIdAsync(request.RequestId, cancellationToken);

        if (gatepassRequest is null)
            return ApiResponse<string>.Failure("Gatepass request not found.");

        if (gatepassRequest.ApprovalStatus != ApprovalStatus.Pending)
            return ApiResponse<string>.Failure("Only pending requests can be approved.");

        gatepassRequest.ApprovalStatus = ApprovalStatus.Approved;
        gatepassRequest.ApproverId = request.ApproverId;
        gatepassRequest.ApprovalDate = DateTime.UtcNow;
        gatepassRequest.LastModifiedDate = DateTime.UtcNow;
        _unitOfWork.GatepassRequests.Update(gatepassRequest);

        var uniqueCode = await _codeGenerator.GenerateCodeAsync(cancellationToken);
        var qrCodeBase64 = _qrCodeGenerator.GenerateQRCodeBase64(uniqueCode);

        var gatepass = new Gatepass
        {
            GatePassRequestId = gatepassRequest.Id,
            UniqueCode = uniqueCode,
            QRCodeImage = qrCodeBase64,
            IssueDate = DateTime.UtcNow,
            ValidFrom = gatepassRequest.RequestedDate,
            ValidUntil = gatepassRequest.RequestedDuration,
            IsActive = true,
            IsRevoked = false,
            UsageCount = 0
        };

        await _unitOfWork.Gatepasses.AddAsync(gatepass, cancellationToken);

        var auditLog = new AuditLog
        {
            Timestamp = DateTime.UtcNow,
            UserID = request.ApproverId,
            Action = "ApproveGatepassRequest",
            EntityType = nameof(GatepassRequest),
            EntityID = gatepassRequest.Id,
            Description = $"Approved gatepass request. Generated code: {uniqueCode}"
        };

        await _unitOfWork.Repository<AuditLog>().AddAsync(auditLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<string>.Success(qrCodeBase64, "Gatepass approved and QR code generated.");
    }
}

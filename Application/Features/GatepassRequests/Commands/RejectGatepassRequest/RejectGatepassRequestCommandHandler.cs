using Application.DTOS;
using Application.Interfaces;
using Domain.Enum;
using MediatR;

namespace Application.Features.GatepassRequests.Commands.RejectGatepassRequest;

public class RejectGatepassRequestCommandHandler
    : IRequestHandler<RejectGatepassRequestCommand, ApiResponse<string>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RejectGatepassRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<string>> Handle(
        RejectGatepassRequestCommand request,
        CancellationToken cancellationToken)
    {
        var gatepassRequest = await _unitOfWork.GatepassRequests
            .GetByIdAsync(request.RequestId, cancellationToken);

        if (gatepassRequest is null)
            return ApiResponse<string>.Failure("Gatepass request not found.");

        if (gatepassRequest.ApprovalStatus != ApprovalStatus.Pending)
            return ApiResponse<string>.Failure("Only pending requests can be rejected.");

        gatepassRequest.ApprovalStatus = ApprovalStatus.Rejected;
        gatepassRequest.ApproverId = request.ApproverId;
        gatepassRequest.ApprovalDate = DateTime.UtcNow;
        gatepassRequest.RejectionReason = request.RejectionReason;
        gatepassRequest.LastModifiedDate = DateTime.UtcNow;

        _unitOfWork.GatepassRequests.Update(gatepassRequest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<string>.Success("Rejected", "Gatepass request rejected successfully.");
    }
}

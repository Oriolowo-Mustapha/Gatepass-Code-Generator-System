using Application.DTOS;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.GatepassRequests.Commands.CreateGatepassRequest;

public class CreateGatepassRequestCommandHandler
    : IRequestHandler<CreateGatepassRequestCommand, ApiResponse<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUniqueCodeGenerator _uniqueCodeGenerator;
    private readonly IQRCodeGenerator _qrCodeGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<CreateGatepassRequestCommandHandler> _logger;

    public CreateGatepassRequestCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IUniqueCodeGenerator uniqueCodeGenerator,
        IQRCodeGenerator qrCodeGenerator,
        IEmailService emailService,
        ILogger<CreateGatepassRequestCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _uniqueCodeGenerator = uniqueCodeGenerator;
        _qrCodeGenerator = qrCodeGenerator;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ApiResponse<string>> Handle(
        CreateGatepassRequestCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating gatepass request for visitor {Email}", request.VisitorEmail);

        if (!_currentUserService.UserId.HasValue)
            throw new UnauthorizedException("You must be logged in to create a gatepass request.");

        var hostId = _currentUserService.UserId.Value;
        _logger.LogDebug("Host user ID: {HostId}", hostId);

        var visitors = await _unitOfWork.Repository<Visitor>().FindAsync(
            v => v.Email.ToLower() == request.VisitorEmail.ToLower(),
            cancellationToken
        );

        var visitor = visitors.FirstOrDefault();

        if (visitor == null)
        {
            _logger.LogInformation("Visitor not found, creating new visitor record for {Email}", request.VisitorEmail);
            visitor = new Visitor
            {
                FirstName = request.VisitorFirstName,
                LastName = request.VisitorLastName,
                ContactNumber = request.VisitorPhoneNumber,
                Email = request.VisitorEmail,
                RegistrationDate = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Visitor>().AddAsync(visitor, cancellationToken);
        }
        else
        {
            _logger.LogDebug("Existing visitor found with ID {VisitorId}", visitor.Id);
        }

        var deptId = await _unitOfWork.Departements.GetDeptIdBydeptCodeAsync(request.DestinationDepartmentCode);
        var gatepassRequest = new GatepassRequest
        {
            Visitor = visitor,
            HostUserId = hostId,
            DestinationDepartmentId = deptId,
            VisitPurpose = request.VisitPurpose,
            RequestedDate = request.ValidFrom,
            RequestedDuration = request.ValidUntil,
            ApprovalStatus = ApprovalStatus.Approved,
            ApprovalDate = DateTime.UtcNow,
            ApproverId = hostId, 
            LastModifiedDate = DateTime.UtcNow,
            RequestDate = DateTime.UtcNow
        };

        if (request.Vehicle != null)
        {
            gatepassRequest.VehicleDetails = new VehicleDetails
            {
                PlateNumber = request.Vehicle.PlateNumber,
            };
            gatepassRequest.GatepassType = GatepassType.Vehicle;
        }
        else
        {
            gatepassRequest.GatepassType = GatepassType.Visitor;
        }

        await _unitOfWork.GatepassRequests.AddAsync(gatepassRequest, cancellationToken);

        var uniqueCode = await _uniqueCodeGenerator.GenerateCodeAsync(cancellationToken);
        var qrCodeBase64 = _qrCodeGenerator.GenerateQRCodeBase64(uniqueCode);
        _logger.LogDebug("Generated gatepass code {Code} with QR code ({Length} chars)", uniqueCode, qrCodeBase64.Length);

        var gatepass = new Gatepass
        {
            GatepassRequest = gatepassRequest,
            UniqueCode = uniqueCode,
            QRCodeImage = qrCodeBase64,
            IsActive = true,
            ValidFrom = DateTime.UtcNow,
            ValidUntil = request.ValidUntil
        };

        await _unitOfWork.Gatepasses.AddAsync(gatepass, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Gatepass {GatepassId} created with code {Code}", gatepass.Id, uniqueCode);

        var qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
        var emailBody = $"""
            <h2>Your Gatepass Has Been Issued</h2>
            <p>Dear {visitor.FirstName} {visitor.LastName},</p>
            <p>A gatepass has been generated for your upcoming visit.</p>
            <p><strong>Gatepass Code:</strong> {uniqueCode}</p>
            <p><strong>Valid From:</strong> {gatepass.ValidFrom:g}</p>
            <p><strong>Valid Until:</strong> {gatepass.ValidUntil:g}</p>
            <p>Please find your QR code attached. Present it at the security checkpoint for check-in.</p>
            <br/>
            <p>Regards,<br/>Gatepass Code Generator System</p>
            """;

        try
        {
            await _emailService.SendEmailWithAttachmentAsync(
                visitor.Email,
                "Your Gatepass QR Code",
                emailBody,
                "gatepass-qrcode.png",
                qrCodeBytes,
                "image/png",
                cancellationToken);

            _logger.LogInformation("QR code email sent successfully to {Email} for gatepass {Code}", visitor.Email, uniqueCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send QR code email to {Email} for gatepass {Code}. Gatepass was created but email delivery failed.", visitor.Email, uniqueCode);
        }

        return ApiResponse<string>.Success(uniqueCode, "Gatepass created successfully. The QR code has been sent to the visitor's email.");
    }
}

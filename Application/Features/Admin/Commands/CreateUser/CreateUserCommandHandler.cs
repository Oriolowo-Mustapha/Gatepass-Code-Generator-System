using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Application.Features.Admin.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    private static readonly string[] AllowedRoles = ["Host", "Security"];

    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IEmailService emailService,
        ILogger<CreateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin creating user {Email} with role {Role}", request.Email, request.RoleName);

        if (!AllowedRoles.Contains(request.RoleName, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException($"Only the following roles can be created: {string.Join(", ", AllowedRoles)}");
        }

        var isEmailUnique = await _unitOfWork.Users.IsEmailUniqueAsync(request.Email);
        if (!isEmailUnique)
        {
            _logger.LogWarning("User creation failed: email {Email} already in use", request.Email);
            throw new ValidationException("Email Already In Use");
        }

        var rawPassword = GenerateRandomPassword(12);
        var hashedPassword = _passwordHasher.HashPassword(rawPassword);
        var roleId = await _unitOfWork.Roles.GetIdByRoleNameAsync(request.RoleName);

        Guid? deptId = null;
        if (request.DepartmentCode != null)
        {
            deptId = await _unitOfWork.Departements.GetDeptIdBydeptCodeAsync(request.DepartmentCode);
        }

        var refreshToken = _tokenService.GenerateRefreshToken();

        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(roleId);
        var permissions = role?.Permissions ?? string.Empty;

        var user = new User
        {
            UserName = request.UserName,
            PasswordHash = hashedPassword,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RoleId = roleId,
            DepartmentId = deptId,
            PhoneNumber = request.PhoneNumber,
            RefreshToken = refreshToken,
            IsActive = true,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(24)
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User created successfully: {UserId} ({Email})", user.Id, user.Email);

        try
        {
            var emailBody = BuildWelcomeEmail(request.FirstName, request.LastName, request.Email, rawPassword, request.RoleName, permissions);
            await _emailService.SendEmailAsync(request.Email, "Your Gatepass System Account Has Been Created", emailBody, cancellationToken);
            _logger.LogInformation("Credentials email sent to {Email}", request.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}. User was created successfully but email delivery failed.", request.Email);
        }

        return ApiResponse<Guid>.Success(user.Id, "User Created Successfully");
    }

    private static string GenerateRandomPassword(int length)
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%&*";
        const string all = upper + lower + digits + special;

        var password = new char[length];
        password[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        password[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        password[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        password[3] = special[RandomNumberGenerator.GetInt32(special.Length)];

        for (int i = 4; i < length; i++)
        {
            password[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }

        RandomNumberGenerator.Shuffle<char>(password);
        return new string(password);
    }

    private static string BuildWelcomeEmail(string firstName, string lastName, string email, string password, string roleName, string permissions)
    {
        var permissionsList = string.Join("", permissions.Split(',').Select(p => $"<li>{p.Trim()}</li>"));

        return $"""
            <html>
            <body style="font-family: Arial, sans-serif; line-height: 1.6; color: #333;">
                <div style="max-width: 600px; margin: 0 auto; padding: 20px;">
                    <h2 style="color: #2c3e50;">Welcome to the Gatepass Code Generator System</h2>
                    <p>Hello <strong>{firstName} {lastName}</strong>,</p>
                    <p>An account has been created for you by the system administrator with the following details:</p>
                    <table style="width: 100%; border-collapse: collapse; margin: 15px 0;">
                        <tr>
                            <td style="padding: 8px; border: 1px solid #ddd; font-weight: bold;">Email</td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{email}</td>
                        </tr>
                        <tr>
                            <td style="padding: 8px; border: 1px solid #ddd; font-weight: bold;">Password</td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{password}</td>
                        </tr>
                        <tr>
                            <td style="padding: 8px; border: 1px solid #ddd; font-weight: bold;">Role</td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{roleName}</td>
                        </tr>
                    </table>
                    <p><strong>Permissions:</strong></p>
                    <ul>{permissionsList}</ul>
                    <p style="color: #e74c3c;"><strong>Important:</strong> Please change your password after your first login for security purposes.</p>
                    <hr style="border: none; border-top: 1px solid #ddd; margin: 20px 0;" />
                    <p style="font-size: 12px; color: #999;">This is an automated message from the Gatepass Code Generator System. Please do not reply to this email.</p>
                </div>
            </body>
            </html>
            """;
    }
}

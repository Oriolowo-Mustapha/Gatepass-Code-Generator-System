using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService, ILogger<RegisterUserCommandHandler> logger) 
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
        }
        async Task<ApiResponse<Guid>> IRequestHandler<RegisterUserCommand, ApiResponse<Guid>>.Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registration attempt for {Email} with role {Role}", request.Email, request.RoleName);

            var checkEmail = await _unitOfWork.Users.IsEmailUniqueAsync(request.Email);
            if(checkEmail == false)
            {
                _logger.LogWarning("Registration failed: email {Email} already in use", request.Email);
                throw new ValidationException("Email Already In Use");
            }

            var password =  _passwordHasher.HashPassword(request.Password);
            var roleId = await _unitOfWork.Roles.GetIdByRoleNameAsync(request.RoleName);

            Guid? deptId = null;
            if (request.DepartmentCode != null)
            {
                deptId = await _unitOfWork.Departements.GetDeptIdBydeptCodeAsync(request.DepartmentCode);
            }

            var RefreshToken = _tokenService.GenerateRefreshToken();

            var user = new User
            {
                UserName = request.UserName,
                PasswordHash = password,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = roleId,
                DepartmentId = deptId,
                PhoneNumber = request.PhoneNumber,
                RefreshToken = RefreshToken,
                IsActive = true,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(24)
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User registered successfully: {UserId} ({Email})", user.Id, user.Email);

            return ApiResponse<Guid>.Success(user.Id, "User Registered Successfully");
        }
    }
}

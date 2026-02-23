using Application.DTOS;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher; 
        public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService) 
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }
        async Task<ApiResponse<Guid>> IRequestHandler<RegisterUserCommand, ApiResponse<Guid>>.Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var checkEmail = await _unitOfWork.Users.IsEmailUniqueAsync(request.Email);
            if(checkEmail == false)
            {
                throw new ValidationException("Email Already In Use");
            }

            var password =  _passwordHasher.HashPassword(request.Password);
            var roleId = await _unitOfWork.Roles.GetIdByRoleNameAsync(request.RoleName);
            
            Guid? deptId = null;
            if (request.DepartmentName != null)
            {
                deptId = await _unitOfWork.Departements.GetDeptIdBydeptNameAsync(request.DepartmentName);
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

            return ApiResponse<Guid>.Success(user.Id, "User Registered Successfully");
        }
    }
}

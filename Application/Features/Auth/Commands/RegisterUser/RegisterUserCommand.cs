using Application.DTOS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands.RegisterUser
{
    public record RegisterUserCommand : IRequest<ApiResponse<Guid>>
    {
        public string UserName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init;  } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string RoleName { get; init;  } = string.Empty;
        public string? DepartmentName { get; init; } = string.Empty;
    }
}

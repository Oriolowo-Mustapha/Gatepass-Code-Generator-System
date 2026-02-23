using Application.DTOS;
using MediatR;

namespace Application.Features.Organization.Queries.GetDepartments;

public record GetDepartmentsQuery : IRequest<ApiResponse<List<DepartmentDto>>>;

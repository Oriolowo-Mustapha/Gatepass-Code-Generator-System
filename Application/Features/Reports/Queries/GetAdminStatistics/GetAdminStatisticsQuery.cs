using Application.DTOS;
using MediatR;

namespace Application.Features.Reports.Queries.GetAdminStatistics;

public record GetAdminStatisticsQuery : IRequest<ApiResponse<AdminStatisticsDto>>;

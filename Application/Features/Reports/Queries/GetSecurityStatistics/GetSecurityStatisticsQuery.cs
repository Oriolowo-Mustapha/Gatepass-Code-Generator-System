using Application.DTOS;
using MediatR;

namespace Application.Features.Reports.Queries.GetSecurityStatistics;

public record GetSecurityStatisticsQuery : IRequest<ApiResponse<SecurityStatisticsDto>>;

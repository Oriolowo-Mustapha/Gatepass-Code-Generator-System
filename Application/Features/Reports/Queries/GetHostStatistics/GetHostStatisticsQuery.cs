using Application.DTOS;
using MediatR;

namespace Application.Features.Reports.Queries.GetHostStatistics;

public record GetHostStatisticsQuery : IRequest<ApiResponse<HostStatisticsDto>>;

using Application.DTOS;
using MediatR;

namespace Application.Features.Reports.Queries.GetGatepassStatistics;

public record GetGatepassStatisticsQuery : IRequest<ApiResponse<GatepassStatisticsDto>>;

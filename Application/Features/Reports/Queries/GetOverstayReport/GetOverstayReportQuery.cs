using Application.DTOS;
using MediatR;

namespace Application.Features.Reports.Queries.GetOverstayReport;

public record GetOverstayReportQuery : IRequest<ApiResponse<List<OverstayReportDto>>>;

using Application.DTOS;
using MediatR;

namespace Application.Features.Organization.Queries.GetAccessPoints;

public record GetAccessPointsQuery : IRequest<ApiResponse<List<AccessPointDto>>>;

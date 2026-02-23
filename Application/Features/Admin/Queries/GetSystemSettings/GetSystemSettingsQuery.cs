using Application.DTOS;
using MediatR;

namespace Application.Features.Admin.Queries.GetSystemSettings;

public record GetSystemSettingsQuery : IRequest<ApiResponse<List<SystemConfigurationDto>>>;

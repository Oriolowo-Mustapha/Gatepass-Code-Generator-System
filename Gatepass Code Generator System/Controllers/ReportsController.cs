using Application.Features.Reports.Queries.GetAdminStatistics;
using Application.Features.Reports.Queries.GetDailyVisitorLog;
using Application.Features.Reports.Queries.GetHostStatistics;
using Application.Features.Reports.Queries.GetOverstayReport;
using Application.Features.Reports.Queries.GetSecurityStatistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gatepass_Code_Generator_System.Controllers;

[Route("api/reports")]
[Authorize]
public class ReportsController : BaseApiController
{
    [HttpGet("daily-log")]
    public async Task<IActionResult> GetDailyLog([FromQuery] DateTime date)
    {
        var response = await Mediator.Send(new GetDailyVisitorLogQuery { Date = date });
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("statistics/admin")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAdminStatistics()
    {
        var response = await Mediator.Send(new GetAdminStatisticsQuery());
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("statistics/security")]
    [Authorize(Roles = "Security")]
    public async Task<IActionResult> GetSecurityStatistics()
    {
        var response = await Mediator.Send(new GetSecurityStatisticsQuery());
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("statistics/host")]
    [Authorize(Roles = "Host")]
    public async Task<IActionResult> GetHostStatistics()
    {
        var response = await Mediator.Send(new GetHostStatisticsQuery());
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("overstays")]
    public async Task<IActionResult> GetOverstays()
    {
        var response = await Mediator.Send(new GetOverstayReportQuery());
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }
}

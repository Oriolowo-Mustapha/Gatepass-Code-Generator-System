using Application.Features.Reports.Queries.GetDailyVisitorLog;
using Application.Features.Reports.Queries.GetGatepassStatistics;
using Application.Features.Reports.Queries.GetOverstayReport;
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

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var response = await Mediator.Send(new GetGatepassStatisticsQuery());
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("overstays")]
    public async Task<IActionResult> GetOverstays()
    {
        var response = await Mediator.Send(new GetOverstayReportQuery());
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }
}

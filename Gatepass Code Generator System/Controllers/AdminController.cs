using Application.Features.Admin.Commands.UpdateSystemSetting;
using Application.Features.Admin.Queries.GetAuditLogs;
using Application.Features.Admin.Queries.GetSystemSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gatepass_Code_Generator_System.Controllers;

[Route("api/admin")]
[Authorize(Roles = "Administrator")]
public class AdminController : BaseApiController
{
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var response = await Mediator.Send(new GetSystemSettingsQuery());
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSetting([FromBody] UpdateSystemSettingCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("auditlogs")]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var response = await Mediator.Send(new GetAuditLogsQuery
        {
            StartDate = startDate,
            EndDate = endDate
        });
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }
}

using Application.Features.Admin.Commands.CreateUser;
using Application.Features.Admin.Commands.DeleteStaffUser;
using Application.Features.Admin.Commands.UpdateStaffUser;
using Application.Features.Admin.Commands.UpdateSystemSetting;
using Application.Features.Admin.Queries.GetAuditLogs;
using Application.Features.Admin.Queries.GetStaffUserById;
using Application.Features.Admin.Queries.GetStaffUsers;
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

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetStaffUsers([FromQuery] string? role)
    {
        var response = await Mediator.Send(new GetStaffUsersQuery { RoleName = role });
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetStaffUser(Guid id)
    {
        var response = await Mediator.Send(new GetStaffUserByIdQuery { UserId = id });
        return !response.Succeeded ? NotFound(response) : Ok(response);
    }

    [HttpPut("users/{id:guid}")]
    public async Task<IActionResult> UpdateStaffUser(Guid id, [FromBody] UpdateStaffUserCommand command)
    {
        var updated = command with { UserId = id };
        var response = await Mediator.Send(updated);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteStaffUser(Guid id)
    {
        var response = await Mediator.Send(new DeleteStaffUserCommand { UserId = id });
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

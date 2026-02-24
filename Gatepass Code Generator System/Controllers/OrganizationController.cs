using Application.Features.Organization.Commands.CreateAccessPoint;
using Application.Features.Organization.Commands.CreateDepartment;
using Application.Features.Organization.Queries.GetAccessPoints;
using Application.Features.Organization.Queries.GetDepartments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gatepass_Code_Generator_System.Controllers;

[Route("api/organization")]
[Authorize(Roles = "Administrator")]
public class OrganizationController : BaseApiController
{
    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments()
    {
        var response = await Mediator.Send(new GetDepartmentsQuery());
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpPost("departments")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("accesspoints")]
    public async Task<IActionResult> GetAccessPoints()
    {
        var response = await Mediator.Send(new GetAccessPointsQuery());
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpPost("accesspoints")]
    public async Task<IActionResult> CreateAccessPoint([FromBody] CreateAccessPointCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }
}

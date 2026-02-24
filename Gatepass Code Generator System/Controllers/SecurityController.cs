using Application.Features.Security.Commands.CheckInVisitor;
using Application.Features.Security.Commands.CheckOutVisitor;
using Application.Features.Security.Queries.VerifyGatepass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gatepass_Code_Generator_System.Controllers;

[Route("api/security")]
[Authorize(Roles = "Security")]
public class SecurityController : BaseApiController
{
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyGatepassQuery query)
    {
        var response = await Mediator.Send(query);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInVisitorCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutVisitorCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }
}

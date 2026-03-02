using Application.Features.GatepassRequests.Commands.CreateGatepassRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gatepass_Code_Generator_System.Controllers;

[Route("api/gatepassrequests")]
[Authorize]
public class GatepassRequestsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGatepassRequestCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }
}

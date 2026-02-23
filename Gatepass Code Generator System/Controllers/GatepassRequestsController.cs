using Application.Features.GatepassRequests.Commands.ApproveGatepassRequest;
using Application.Features.GatepassRequests.Commands.CreateGatepassRequest;
using Application.Features.GatepassRequests.Commands.RejectGatepassRequest;
using Application.Features.GatepassRequests.Queries.GetPendingRequests;
using Microsoft.AspNetCore.Mvc;

namespace Gatepass_Code_Generator_System.Controllers;

[Route("api/gatepassrequests")]
public class GatepassRequestsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGatepassRequestCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpGet("pending/{hostId:guid}")]
    public async Task<IActionResult> GetPending([FromRoute] Guid hostId)
    {
        var response = await Mediator.Send(new GetPendingRequestsQuery { HostId = hostId });
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        [FromRoute] Guid id,
        [FromBody] ApproveGatepassRequestCommand command)
    {
        var response = await Mediator.Send(command with { RequestId = id });
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        [FromRoute] Guid id,
        [FromBody] RejectGatepassRequestCommand command)
    {
        var response = await Mediator.Send(command with { RequestId = id });
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }
}

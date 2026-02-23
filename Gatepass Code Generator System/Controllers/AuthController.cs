using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.RegisterUser;
using Microsoft.AspNetCore.Mvc;

namespace Gatepass_Code_Generator_System.Controllers;

[Route("api/auth")]
public class AuthController : BaseApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var response = await Mediator.Send(command);
        return !response.Succeeded ? BadRequest(response) : Ok(response);
    }
}

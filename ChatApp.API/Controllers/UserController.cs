using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/user/")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        await _userService.Register(request.UserName, request.PhoneNumber, request.Password);

        return Ok(new {message = string.Format(InfoMessages.SuccessfulRegistration)});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var token = await _userService.Login(request.PhoneNumber, request.Password);
        
        Response.Cookies.Append("token", token);
        
        return Ok(new {message = string.Format(InfoMessages.SuccessfulLogin)});
    }
}
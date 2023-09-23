using Microsoft.AspNetCore.Mvc;

using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);

        if (response == null)
            return BadRequest(new { message = "Username or password is incorrect" });

        return Ok(response);
    }

    [Authorize(Role.User)]
    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    [Authorize(Role.Admin)]
    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var user = _userService.GetById(id);

        return Ok(user);
    }
}

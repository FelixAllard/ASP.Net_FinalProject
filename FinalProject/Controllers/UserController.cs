using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;

[Authorize(Roles = "User")]
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet("/user")]
    public IActionResult Get()
    {
        return Ok("You Have Accessed the user Controller");
    }
    
}
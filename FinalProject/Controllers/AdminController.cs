using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;

[Authorize (Roles = "Admin")]
[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    [HttpGet("/admin")]
    public IActionResult Get()
    {
        return Ok("You Have Accessed the admin Controller");
    }
    
}
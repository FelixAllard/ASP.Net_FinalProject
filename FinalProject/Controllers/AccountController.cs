using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FinalProject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody] Register model)
    {
        var user = new ApplicationUser { UserName = model.Username };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            return Ok(new {message = "Registration successful"});
        }
        return BadRequest(result.Errors);
    }
    [HttpPost("register/artist")]
    public async Task<IActionResult> RegisterArtist([FromBody] RegisterArtist model)
    {
        // Create a new ApplicationUser (IdentityUser) instance
        var user = new ApplicationUser 
        { 
            UserName = model.Username, 
            Email = model.Email,
            ArtistName = model.ArtistName,
            Bio = model.Bio
        };

        // Create the user with the provided password
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Assign the User and Artist roles to the new user
            await _userManager.AddToRoleAsync(user, "User");
            await _userManager.AddToRoleAsync(user, "Artist");

            return Ok(new { message = "Registration successful, Artist role assigned." });
        }

        // If creation failed, return the errors
        return BadRequest(result.Errors);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var roles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                claims : authClaims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            SecurityAlgorithms.HmacSha256
                )
            );
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            
        } 
        return Unauthorized();
    }

    /*
    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(role));
            if (result.Succeeded)
            {
                return Ok(new { message = "Role added successfully!"});
            }
            return BadRequest(result.Errors);
        }
        return BadRequest("Role already exists!");
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] UserRole model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return BadRequest(new { message = "User Not Found" });
        }
        var result = await _userManager.AddToRoleAsync(user, model.Role);
        if (result.Succeeded)
        {
            return Ok(new { message = "Role assigned successfully!"});
        }
        return BadRequest(result.Errors);
    }*/
}
using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
public class ExhibitionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExhibitionController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/Exhibition
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<Exhibition>>> GetExhibitionsForArtist()
    {
        // Get the current logged-in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized("User not found");
        }

        // Return exhibitions for the current logged-in artist
        var exhibitions = await _context.Exhibitions
                                        .Where(e => e.ArtistId == user.Id)  // Filter by ArtistId (ApplicationUser)
                                        .ToListAsync();

        if (exhibitions == null || !exhibitions.Any())
        {
            return NotFound("No exhibitions found for this artist.");
        }

        return Ok(exhibitions);
    }

    // GET: api/Exhibition/{name}
    [HttpGet("{name}")]
    public async Task<ActionResult<Exhibition>> GetExhibitionByName(string name)
    {
        var exhibition = await _context.Exhibitions
                                       .Where(e => e.Title.Contains(name))  // Search by exhibition name
                                       .FirstOrDefaultAsync();

        if (exhibition == null)
        {
            return NotFound("Exhibition not found.");
        }

        return Ok(exhibition);
    }

    // GET: api/Exhibition/artist/{artistId}
    [HttpGet("artist/{artistId}")]
    public async Task<ActionResult<IEnumerable<Exhibition>>> GetExhibitionsByArtist(string artistId)
    {
        var exhibitions = await _context.Exhibitions
                                        .Where(e => e.ArtistId == artistId)  // Filter by artistId
                                        .ToListAsync();

        if (exhibitions == null || !exhibitions.Any())
        {
            return NotFound("No exhibitions found for this artist.");
        }

        return Ok(exhibitions);
    }


    [Authorize(Roles = "Artist")]
    [HttpPost("")]
    public async Task<ActionResult<Exhibition>> CreateExhibition(Exhibition exhibition)
    {
        // Get the current logged-in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized("User not found");
        }

        // Set the artist (user) as the creator of the exhibition
        exhibition.ArtistId = user.Id;

        _context.Exhibitions.Add(exhibition);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExhibitionByName), new { name = exhibition.Title }, exhibition);
    }

    [Authorize(Roles = "Artist")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExhibition(int id)
    {
        var exhibition = await _context.Exhibitions.FindAsync(id);
        if (exhibition == null)
        {
            return NotFound("Exhibition not found.");
        }

        // Ensure the logged-in user is the artist for this exhibition
        var user = await _userManager.GetUserAsync(User);
        if (user == null || exhibition.ArtistId != user.Id)
        {
            return Unauthorized("You are not authorized to delete this exhibition.");
        }

        _context.Exhibitions.Remove(exhibition);
        await _context.SaveChangesAsync();

        return NoContent();  // Return 204 No Content on successful delete
    }
}
using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Microsoft.AspNetCore.Components.Route("api/artists")]
[ApiController]
public class ArtistController : ControllerBase
{
    [HttpGet("paintings")]
    public async Task<IActionResult> GetAllPaintings([FromServices] AppDbContext dbContext)
    {
        var paintings = await dbContext.Paintings.ToListAsync();
        return Ok(paintings);
    }

    [HttpGet("user/{userId}/paintings")]
    public async Task<IActionResult> GetUserPaintings(string userId, [FromServices] AppDbContext dbContext)
    {
        var paintings = await dbContext.Paintings
            .Where(p => p.UserId == userId)
            .ToListAsync();

        return Ok(paintings);
    }

    [HttpGet("users-with-paintings")]
    public async Task<IActionResult> GetUsersWithPaintings([FromServices] AppDbContext dbContext)
    {
        var usersWithPaintings = await dbContext.Users
            .Include(u => u.Paintings)
            .ToListAsync();

        return Ok(usersWithPaintings);
    }
    [HttpPost("create-painting")]
    [Authorize(Roles = "Artist")]  // Ensure that only users with the "Artist" role can create a painting
    public async Task<IActionResult> CreatePainting([FromBody] Painting model, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] AppDbContext dbContext)
    {
        // Validate if the required fields are provided (model validation)
        if (!ModelState.IsValid)
        {
            return BadRequest("All fields are required.");
        }

        // Get the current authenticated user's ID (the artist)
        var userId = userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        // Check if the exhibition exists and belongs to the currently authenticated user
        var exhibition = await dbContext.Exhibitions
            .FirstOrDefaultAsync(e => e.ExhibitionId == model.ExhibitId && e.ArtistId == userId);

        if (exhibition == null)
        {
            return BadRequest("Exhibition does not belong to the current user or does not exist.");
        }

        // Create a new painting object with all required properties
        var painting = new Painting
        {
            Title = model.Title,
            Medium = model.Medium,
            Dimensions = model.Dimensions,
            Price = model.Price,
            Story = model.Story,
            ImageUrl = model.ImageUrl,
            ExhibitId = model.ExhibitId,
            UserId = userId // Associate the painting with the artist
        };

        // Save the painting to the database
        dbContext.Paintings.Add(painting);
        await dbContext.SaveChangesAsync();

        return Ok(new { message = "Painting created successfully!" });
    }
    
    [HttpDelete("delete-painting/{paintingId}")]
    [Authorize(Roles = "Artist")]  // Ensure that only users with the "Artist" role can delete a painting
    public async Task<IActionResult> DeletePainting(int paintingId, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] AppDbContext dbContext)
    {
        // Get the current authenticated user's ID (the artist)
        var userId = userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        // Find the painting in the database
        var painting = await dbContext.Paintings
            .FirstOrDefaultAsync(p => p.PaintingId == paintingId && p.UserId == userId);

        // If the painting doesn't exist or doesn't belong to the current user, return a not found or bad request response
        if (painting == null)
        {
            return NotFound("Painting not found or you are not authorized to delete this painting.");
        }

        // Remove the painting from the database
        dbContext.Paintings.Remove(painting);
        await dbContext.SaveChangesAsync();

        return Ok(new { message = "Painting deleted successfully." });
    }
    [HttpPut("edit-painting/{paintingId}")]
    [Authorize(Roles = "Artist")]  // Ensure that only users with the "Artist" role can edit a painting
    public async Task<IActionResult> EditPainting(int paintingId, [FromBody] Painting model, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] AppDbContext dbContext)
    {
        // Get the current authenticated user's ID (the artist)
        var userId = userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        // Find the painting in the database
        var painting = await dbContext.Paintings
            .FirstOrDefaultAsync(p => p.PaintingId == paintingId && p.UserId == userId);

        // If the painting doesn't exist or doesn't belong to the current user, return a not found or bad request response
        if (painting == null)
        {
            return NotFound("Painting not found or you are not authorized to edit this painting.");
        }

        // Update the painting properties with the new values from the model
        painting.Title = model.Title ?? painting.Title;
        painting.Story = model.Story ?? painting.Story;
        painting.Medium = model.Medium ?? painting.Medium;
        painting.Dimensions = model.Dimensions ?? painting.Dimensions;
        painting.Price = model.Price != 0 ? model.Price : painting.Price;  // Only update if the new value is not the default
        painting.ImageUrl = model.ImageUrl ?? painting.ImageUrl;

        // Save the changes to the database
        dbContext.Paintings.Update(painting);
        await dbContext.SaveChangesAsync();

        return Ok(new { message = "Painting updated successfully." });
    }
    

}
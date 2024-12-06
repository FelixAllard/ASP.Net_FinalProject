using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Views.Painting;

[Authorize(Roles = "Artist")]
public class CreatePaintingModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreatePaintingModel(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Models.Painting Painting { get; set; } = new();

    public List<Exhibition> Exhibitions { get; set; } = new();

    [BindProperty]
    public int ExhibitionId { get; set; } // Selected Exhibition ID from dropdown

    public async Task<IActionResult> OnGetAsync()
    {
        // Fetch the exhibitions for the current artist
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        Exhibitions = await _context.Exhibitions
            .Where(e => e.ArtistId == user.Id)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var exhibition = await _context.Exhibitions
            .FirstOrDefaultAsync(e => e.ExhibitionId == ExhibitionId && e.ArtistId == user.Id);

        if (exhibition == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid exhibition selected.");
            return Page();
        }

        // Associate painting with exhibition
        Painting.ExhibitId = ExhibitionId;
        Painting.UserId = user.Id;

        _context.Paintings.Add(Painting);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index"); // Redirect to a paintings list or another page
    }
}

using Microsoft.AspNetCore.Identity;

namespace FinalProject.Models;

public class ApplicationUser : IdentityUser
{
    // ICollection to store all the paintings for the artist
    public ICollection<Painting> Paintings { get; set; } = new List<Painting>();

    // ICollection to store all the exhibitions the artist is involved in
    public ICollection<Exhibition> Exhibitions { get; set; } = new List<Exhibition>();

    // Additional fields related to the artist
    public string ArtistName { get; set; } = string.Empty;  // Artist's name
    public string? Bio { get; set; }  // Optional biography field
}



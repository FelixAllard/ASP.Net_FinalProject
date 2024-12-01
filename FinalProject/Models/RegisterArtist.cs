namespace FinalProject.Models;

public class RegisterArtist
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty; 
    public string? Bio { get; set; }
}
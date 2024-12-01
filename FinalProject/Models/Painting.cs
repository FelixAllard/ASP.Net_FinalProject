using System.ComponentModel.DataAnnotations;
using FinalProject.Data;

namespace FinalProject.Models;

public class Painting
{
    [Key]
    public int PaintingId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    public string Medium { get; set; }

    [Required]
    public string Dimensions { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    public string Story { get; set; }  // Optional story for the painting

    [Required]
    [StringLength(255)]
    public string ImageUrl { get; set; }
    
    public int ExhibitId { get; set; }

    // Foreign Key to the ApplicationUser (the artist)
    public string UserId { get; set; } = string.Empty;  // The artist's user id
    
    // Navigation property to the ApplicationUser (Artist)
    public ApplicationUser User { get; set; } = null!;
}

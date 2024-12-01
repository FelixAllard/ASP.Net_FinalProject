using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models;

public class Exhibition
{
    [Key]
    public int ExhibitionId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    // Foreign Key to the ApplicationUser (Artist)
    public string ArtistId { get; set; }

    // Navigation property to the ApplicationUser (Artist)
    public ApplicationUser Artist { get; set; }

    // Navigation property to Paintings (One exhibition can have many paintings)
    public ICollection<Painting> Paintings { get; set; } = new List<Painting>();
}


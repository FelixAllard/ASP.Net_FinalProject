using System.ComponentModel.DataAnnotations;

namespace FinalProject.Data;

public class Exhibition
{
    [Key]
    public int exhibitionId { get; set; } 

    [Required]
    [StringLength(100)]
    public string title { get; set; } 

    [Required]
    public string description { get; set; } 

    [Required]
    public int artistId { get; set; }

    public Artist artist { get; set; } 

    public ICollection<Painting> paintings { get; set; } 
}
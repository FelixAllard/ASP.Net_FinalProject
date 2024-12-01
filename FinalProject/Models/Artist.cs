using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinalProject.Models;

namespace FinalProject.Data;

public class Artist
{
    [Key]
    public int ArtistId { get; set; } 

    [Required]
    [ForeignKey("UserRole")]
    public string Username { get; set; } 

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public string Biography { get; set; } 

    public ICollection<Exhibition> Exhibitions { get; set; } 

    // Navigation property
    public UserRole UserRole { get; set; } 
}